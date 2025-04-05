using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices;
using TabbyCat.Models.RunningHubs;
using TabbyCat.Repository.Entities.RunningHubEntities;
using TabbyCat.Service.RunningHubServices;
using TabbyCat.Shared.Enums;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IRunningHubStateManager>(ServiceLifetime.Singleton)]
public sealed class RunningHubStateManager(
    IRunningHubStateService runningHubStateService,
    IUser user,
    IRunningHubService runningHubService,ILogger<RunningHubStateManager> logger) : IRunningHubStateManager
{
    /// <summary>
    /// key是taskid
    /// </summary>
    private ConcurrentDictionary<string, RunningHubStateEntity> DoingTasks { get; set; } = new();
    

    private HttpClient client = new();

    private Thread backgroundThread=null!;

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
        var url = "https://www.runninghub.cn/task/openapi/status";

        // 构造请求的 JSON 数据
        var jsonData = @$"{{
            ""taskId"": ""{taskId}"",
            ""apiKey"": ""{apiKey}""
        }}";
        // 创建请求内容
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Host", "www.runninghub.cn");
        // 发送 POST 请求
        var response = await client.PostAsync(url, content);

        response.EnsureSuccessStatusCode();
        if (response.IsSuccessStatusCode)
        {
            // 读取返回的内容
            var responseBody = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonConvert.DeserializeObject<RunningHubResponseModel<string>>(responseBody);
            if (jsonResponse is null)
            {
                logger.LogError("json反序列化失败，{0} 到 {1}类型失败。",responseBody,typeof(RunningHubResponseModel<string>));
            }
            else
            {
                if (jsonResponse.Msg == "success")
                {
                    if (jsonResponse.Data == "SUCCESS")
                    {
                        if (DoingTasks.FirstOrDefault(x => x.Key == taskId) is { } find)
                        {
                            find.Value.TaskStatus = TaskState.Success;
                            find.Value.UpdateTime = DateTime.Now;
                            await runningHubStateService.UpdateAsync(find.Value);
                            DoingTasks.Remove(taskId, out _);
                            await OnSuccess.Invoke(find.Value.Key);
                        }
                    }
                    else if (jsonResponse.Data == "FAILED")
                    {
                        if (DoingTasks.FirstOrDefault(x => x.Key == taskId) is { } find)
                        {
                            find.Value.TaskStatus = TaskState.Failed;
                            find.Value.UpdateTime = DateTime.Now;
                            await runningHubStateService.UpdateAsync(find.Value);
                            DoingTasks.Remove(taskId, out _);
                            await OnFailure.Invoke(find.Value.Key);
                        }
                    }
                }
                else
                {
                    logger.LogError("接口返回错误:{0}",jsonResponse.Code);
                }
            }
        }
        else
        {
           logger.LogError("接口响应失败:{0}",response.ReasonPhrase);
        }
    }

    public async Task<bool> AddTaskAsync(RunningHubStateEntity? entity)
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
        if (cancellationTokenSource is not null)
            await cancellationTokenSource.CancelAsync();

        var apiKeys = await runningHubService.QueryAsync(x=> x.Email==user.Email);
        if (!apiKeys.Any())
        {
          return;
        }
        
        var tasks = await runningHubStateService.QueryAsync(x=>x.TaskStatus== TaskState.Running||x.TaskStatus == TaskState.Queued&& x.Email==user.Email);
       foreach (var runningHubStateEntity in tasks)
       {
           DoingTasks.TryAdd(runningHubStateEntity.TaskId, runningHubStateEntity);
       }

        cancellationTokenSource = new();
        backgroundThread = new(() => RunBackgroundThread(cancellationTokenSource.Token));
        backgroundThread.IsBackground = true;
        backgroundThread.Start();
    }

    public Func<Guid, Task> OnSuccess { get; set; } = null!;
    public Func<Guid, Task> OnFailure { get; set; } = null!;
    public Func<int, Task> OnBackgroundTaskCount { get; set; } = null!;
}