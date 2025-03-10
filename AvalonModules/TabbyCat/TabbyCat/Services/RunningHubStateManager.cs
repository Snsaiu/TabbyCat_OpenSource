using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models.RunningHubs;
using TabbyCat.Repository.Entities.RunningHubEntities;
using TabbyCat.Service.RunningHubServices;
using TabbyCat.Shared.Enums;
using TuDog.IocAttribute;
using ThreadState = System.Threading.ThreadState;

namespace TabbyCat.Services;

[Register<IRunningHubStateManager>(ServiceLifetime.Singleton)]
public sealed class RunningHubStateManager(
    IRunningHubStateService runningHubStateService,
    IRunningHubService runningHubService) : IRunningHubStateManager
{
    /// <summary>
    /// key是taskid
    /// </summary>
    private ConcurrentDictionary<string, RunningHubStateEntity> DoingTasks { get; set; } = new();

    private HttpClient client = new();

    private Thread backgroundThread;

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

    private string apiKey = string.Empty;

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
                //todo:处理转换失败的情况
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
                            OnSuccess?.Invoke(find.Value.Key);
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
                            OnFailure?.Invoke(find.Value.Key);
                        }
                    }
                }
                else
                {
                    //todo:处理返回值错误问题
                }
            }
        }
        else
        {
            //todo:处理响应失败
        }
    }

    [Obsolete("接口返回值错误", true)]
    private async Task HttpQueryRunningTaskCountAsync(string apiKey)
    {
        var url = "https://www.runninghub.cn/uc/openapi/accountStatus";

        // 构造请求的 JSON 数据
        var jsonData = @$"{{
            ""apiKey"": ""{apiKey}""
        }}";
        // 创建请求内容
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Host", "www.runninghub.cn");
        // 发送 POST 请求
        var response = await client.PostAsync(url, content);

        response.EnsureSuccessStatusCode();
        if (!response.IsSuccessStatusCode)
            return;

        var responseContent = await response.Content.ReadAsStringAsync();
        var convert =
            JsonConvert.DeserializeObject<RunningHubResponseModel<RunningHubUserStateResponseModel>>(responseContent);
        if (convert is null)
        {
            //todo:
        }
        else
        {
            if (convert.Msg == "success")
            {
                OnBackgroundTaskCount?.Invoke(convert.Data.CurrentTaskCounts);
            }
            else
            {
                //todo:
            }
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

        var apiKeys = await runningHubService.QueryAsync();
        if (apiKeys.Any())
        {
            apiKey = apiKeys.First().ApiKey;
        }
        
        var tasks = await runningHubStateService.QueryAsync(x=>x.TaskStatus== TaskState.Running||x.TaskStatus == TaskState.Queued);
       foreach (var runningHubStateEntity in tasks)
       {
           DoingTasks.TryAdd(runningHubStateEntity.TaskId, runningHubStateEntity);
       }

        cancellationTokenSource = new();
        backgroundThread = new(() => RunBackgroundThread(cancellationTokenSource.Token));
        backgroundThread.IsBackground = true;
        backgroundThread.Start();
    }

    public Func<Guid, Task> OnSuccess { get; set; }
    public Func<Guid, Task> OnFailure { get; set; }
    public Func<int, Task> OnBackgroundTaskCount { get; set; }
}