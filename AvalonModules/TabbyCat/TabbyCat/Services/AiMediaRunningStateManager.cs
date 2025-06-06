using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Enums;
using TabbyCat.Extensions;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Repository.Entities.AiMediaEntities;
using TabbyCat.Service.RunningHubServices;
using TabbyCat.Shared.Enums;
using TuDog.Extensions;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IAiMediaRunningStateManager>(ServiceLifetime.Singleton)]
public sealed class AiMediaRunningStateManager(
    IRunningStateService runningHubStateService,
    HttpClient client,
    IAiMediaResourceService aiMediaResourceService,
    IAiMediaResultService aiMediaResultService,
    ILogger<AiMediaRunningStateManager> logger) : IAiMediaRunningStateManager
{
    /// <summary>
    /// key是taskid
    /// </summary>
    private ConcurrentDictionary<string, AiMediaRunningStateEntity> DoingTasks { get; set; } = new();

    private Thread backgroundThread = null!;

    private void RunBackgroundThread(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            OnBackgroundTaskCount?.Invoke(DoingTasks.Count);
            foreach (var item in DoingTasks)
            {
                HttpQueryTaskStateAsync(item.Value.ApiKey, item.Value.TaskId).Wait(token);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

        Debug.WriteLine("Background thread stopped");
    }


    private CancellationTokenSource? cancellationTokenSource;


    private async Task HttpQueryTaskStateAsync(string apiKey, string taskId)
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(taskId))
            return;

        var url = $"https://dashscope.aliyuncs.com/api/v1/tasks/{taskId}";

        // 创建请求内容

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        // 发送 POST 请求
        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        if (response.IsSuccessStatusCode)
        {
            // 读取返回的内容
            var responseBody = await response.Content.ReadAsStringAsync();

            logger.LogDebug("检查进度json数据:{json}。", responseBody);

            if (DoingTasks.TryGetValue(taskId, out var currentTask))
            {
                IAiMediaTaskStateResponseModel responseModel = currentTask.WorkType switch
                {
                    AiMediaWorkType.TextToImage => JsonConvert.DeserializeObject<AiMediaImageResponseModel>(
                        responseBody),
                    AiMediaWorkType.TextToVideo => JsonConvert.DeserializeObject<AiMediaVideoResponseModel>(
                        responseBody),
                    AiMediaWorkType.CommandEditImage or AiMediaWorkType.PartialRepaintImage
                        or AiMediaWorkType.ExpandImage or
                        AiMediaWorkType.RemoveWatermark or
                        AiMediaWorkType.ImageColorization or
                        AiMediaWorkType.DoodleImage or
                        AiMediaWorkType.AvatarStylization or
                        AiMediaWorkType.ImageSuperResolution =>
                        JsonConvert.DeserializeObject<GenerateImageEditResponseModel>(
                        responseBody),
                    AiMediaWorkType.GraffitiPainting => JsonConvert.DeserializeObject<AiMediaImageResponseModel>(
                        responseBody),
                    AiMediaWorkType.ImageEraseCompletion => JsonConvert
                        .DeserializeObject<AiMediaOnlyOneImageResultResponseModel>(responseBody),
                    _ => throw new NotImplementedException()
                };

                if (responseModel.TaskStatus == AiMediaRunningStatus.Success)
                {

                    currentTask.TaskStatus = TaskState.Success;
                    currentTask.UpdateTime = DateTime.Now;
                    await runningHubStateService.UpdateAsync(currentTask);
                    DoingTasks.Remove(taskId, out _);
                    if (await SaveToDbAsync(currentTask, responseModel.DownloadUrls) || OnSuccess is not null)
                        await OnSuccess.Invoke(currentTask.TaskId, currentTask.WorkType);
                }
                else if (responseModel.TaskStatus == AiMediaRunningStatus.Failed || responseModel.TaskStatus == AiMediaRunningStatus.Unknown)
                {
                    currentTask.TaskStatus = TaskState.Failed;
                    currentTask.UpdateTime = DateTime.Now;
                    await runningHubStateService.UpdateAsync(currentTask);
                    DoingTasks.Remove(taskId, out _);
                    if (OnFailure is not null)
                        await OnFailure.Invoke(currentTask.TaskId);
                }
            }
        }
        else
        {
            logger.LogError("接口响应失败:{0}", response.ReasonPhrase);
        }
    }

    private async Task<bool> SaveToDbAsync(AiMediaRunningStateEntity entity, IEnumerable<string> downloadUrls)
    {
        var entities = new List<AiMediaResultEntity>();
        var savePath = aiMediaResourceService.Get();

        foreach (var url in downloadUrls)
        {
            var fileName = Path.Combine(savePath, $"{Guid.NewGuid().ToString()}{entity.FileExtension}");

            await client.DownloadFileAsync(url, fileName, null);

            entities.Add(await SaveMediaMetaToDBAsync(fileName, entity.TaskId, entity.FileExtension, entity.WorkType));
        }

        return await aiMediaResultService.AddRangeAsync(entities);

        async Task<AiMediaResultEntity> SaveMediaMetaToDBAsync(string fileName, string taskId, string fileExtension,
            AiMediaWorkType workType)
        {
            var entity = new AiMediaResultEntity()
            {
                FileType = fileExtension, SavePath = fileName, TaskId = taskId,
                CreateTime = DateTime.Now, UpdateTime = DateTime.Now,  WorkType = workType
            };

            if (fileExtension == ".mp4")
            {
                var pngPath = fileName.Replace(".mp4", ".png");
                var pngResult = await Util.GenerateThumbnail(fileName, TimeSpan.FromSeconds(1), pngPath);
                if (!pngResult)
                    throw new("Failed to save thumbnail");
                entity.ThumbnailPath = pngPath;
            }

            return entity;
        }
    }

    public async Task<bool> AddTaskAsync(AiMediaRunningStateEntity? entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));
        if (!await runningHubStateService.AddAsync(entity)) throw new("插入任务到数据库失败");
        DoingTasks.TryAdd(entity.TaskId, entity);
        return true;
    }

    public Task StopWatchAsync()
    {
        return cancellationTokenSource is null ? Task.CompletedTask : cancellationTokenSource.CancelAsync();
    }

    public async Task StartWatchAsync()
    {
        var tasks = await runningHubStateService.QueryAsync(x =>
            x.TaskStatus == TaskState.Running || x.TaskStatus == TaskState.Queued);
        foreach (var runningHubStateEntity in tasks)
        {
            DoingTasks.TryAdd(runningHubStateEntity.TaskId, runningHubStateEntity);
        }

        cancellationTokenSource = new();
        backgroundThread = new(() => RunBackgroundThread(cancellationTokenSource.Token));
        backgroundThread.IsBackground = true;
        backgroundThread.Start();
    }

    public async Task ClearTasksAsync()
    {
        foreach (var item in DoingTasks)
        {
            item.Value.UpdateTime = DateTime.Now;
            item.Value.TaskStatus = TaskState.Cancel;
            await runningHubStateService.UpdateAsync(item.Value);
        }

        DoingTasks.Clear();
    }

    public Func<string, AiMediaWorkType, Task> OnSuccess { get; set; }
    public Func<string, Task>? OnFailure { get; set; }
    public Func<int, Task>? OnBackgroundTaskCount { get; set; }
}