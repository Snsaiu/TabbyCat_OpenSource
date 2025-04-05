using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.RunningHubs;
using TabbyCat.Repository.Entities.RunningHubEntities;
using TabbyCat.Service.RunningHubServices;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TuDog.Bootstrap;
using TuDog.Extensions;

namespace TabbyCat.ViewModels;

public abstract partial class AiMediaViewModelBase:ViewModelBase
{
    [ObservableProperty] private int workingTaskCount;

    [ObservableProperty] private bool showPanel;

    [ObservableProperty] private ObservableCollection<RunningHubResultEntity> results = [];

    [ObservableProperty] private ObservableCollection<string> lastBuildResultImages = [];

    private ILogger<AiMediaViewModelBase> _logger =
        TuDogApplication.ServiceProvider.GetRequiredService<ILogger<AiMediaViewModelBase>>();

    protected IRunningHubService RunningHubService { get; }=TuDogApplication.ServiceProvider.GetRequiredService<IRunningHubService>();

    protected IRunningHubStateService RunningHubStateService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IRunningHubStateService>();

    protected IRunningHubResourceService RunningHubResourceService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IRunningHubResourceService>();

    protected IRunningHubStateManager RunningHubStateManager { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IRunningHubStateManager>();
    
    protected IUser user = TuDogApplication.ServiceProvider.GetRequiredService<IUser>();

    protected RunningHubEntity? RunningHubEntity { get; private set; }

    protected IRunningHubResultService RunningHubResultService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IRunningHubResultService>();



    protected string BaseAddress { get; } = @"https://www.runninghub.cn";

    protected abstract RunningHubWorkType RunningHubWorkType { get; }

    [RelayCommand]
    private Task ClosePortfolioPanel()
    {
        ShowPanel = false;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OpenImageByDefaultProgram(RunningHubResultEntity selected)
    {
        return App.TopLevel.Launcher.LaunchUriAsync(new(selected.SavePath));
    }

    [RelayCommand]
    private async Task SaveFileToLocal(RunningHubResultEntity selected)
    {
        var fileName = Path.GetFileName(selected.SavePath);

        var saveLocation = await App.TopLevel.StorageProvider.SaveFilePickerAsync(new()
            { SuggestedFileName = fileName, ShowOverwritePrompt = true, DefaultExtension = selected.FileType });
        if (saveLocation is null)
            return;
        File.Copy(selected.SavePath, saveLocation.Path.LocalPath, true);
        await DialogServer.ShowMessageDialogAsync(AppResources.ExportedSuccessfully);
    }

    /// <summary>
    /// 删除媒体
    /// </summary>
    /// <param name="selected"></param>
    [RelayCommand]
    private async Task DeleteMedia(RunningHubResultEntity selected)
    {
        var deleteConfirm = await DialogServer.ShowConfirmDialogAsync(AppResources.ConfirmDeleteItem);
        if (!deleteConfirm)
            return;

        if ((await RunningHubResultService.DeleteAsync(x => x.Key == selected.Key)) is not null)
        {
            if (File.Exists(selected.SavePath))
            {
                File.Delete(selected.SavePath);
            }

            Results.Remove(selected);
        }
    }

    /// <summary>
    /// 获得结果集
    /// </summary>
    private async Task ResetResultsAsync()
    {
        Results.Reset((await RunningHubResultService.QueryAsync(x=>x.Email==user.Email)).OrderByDescending(x => x.UpdateTime));
    }

    protected abstract long WorkFlowId { get; }

    /// <summary>
    /// 打开作品集
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task OpenPortfolio()
    {
        await ResetResultsAsync();
        ShowPanel = true;
    }

    protected HttpClient HttpClient { get; } = new();

    protected async Task<string?> UploadImageAsync(string imagePath)
    {

        if (RunningHubEntity is not { } runningHubEntity)
        {
            _logger.LogError("上传图片时,{0}不能为空。",nameof(RunningHubEntity));
            await DialogServer.ShowMessageDialogAsync(AppResources.AnErrorOccurred, AppResources.Warning, AppResources.Ok);
            
            return null;
        }
        
        var url = $"{BaseAddress}/task/openapi/upload";
        using MultipartFormDataContent form = new MultipartFormDataContent();
        // 添加 apiKey
        form.Add(new StringContent(runningHubEntity.ApiKey), "apiKey");

        // 添加 fileType
        form.Add(new StringContent("image"), "fileType");

        // 添加文件
        await using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
        {
            HttpContent fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // 确保 MIME 类型正确
            form.Add(fileContent, "file", Path.GetFileName(imagePath));

            // 发送请求
            HttpResponseMessage response = await HttpClient.PostAsync(url, form);
            string result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<RunningHubResponseModel<UploadImageResponseModel>>(result);
            if(data is null)
                throw new NullReferenceException();
            if (data.Msg == "success")
                return data.Data?.FileName;
            await DialogServer.ShowMessageDialogAsync(data.Msg);
            return null;
        }
    }

    protected override Task OnUnLoaded()
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        RunningHubStateManager.OnSuccess -= TaskOkAsync;
        RunningHubStateManager.OnFailure -= TaskFailAsync;
        RunningHubStateManager.OnBackgroundTaskCount -= RunningTaskCountChangedAsync;
#pragma warning restore CS8601 // Possible null reference assignment.
        return Task.CompletedTask;
    }

    private async Task TaskOkAsync(Guid key)
    {
        await DownloadResultAsync(key);
        await QueryIsRunningTaskCountAsync();
        await ResetResultsAsync();
    }

    private Task RunningTaskCountChangedAsync(int count)
    {
        IsBackgroundTaskRunning=count>0;
        return Task.CompletedTask;
    }

    private async Task TaskFailAsync(Guid key)
    {
        await QueryIsRunningTaskCountAsync();
    }

    private async Task DownloadResultAsync(Guid key)
    {
        var find = await RunningHubStateService.QueryAsync(x => x.Key == key && x.TaskStatus == TaskState.Success&& x.Email==user.Email);
        if (!find.Any())
        {
            ErrorMessage = AppResources.EntityNotFound;
            return;
        }

        var entity = find.First();

        var jsonData = $@"{{
            ""taskId"": ""{entity.TaskId}"",
            ""apiKey"": ""{entity.ApiKey}""
        }}";

        // 创建 HttpContent
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        HttpClient.DefaultRequestHeaders.Clear();
        // 添加 Headers
        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Apifox/1.0.0 (https://apifox.com)");
        HttpClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");

        var url = $"{BaseAddress}/task/openapi/outputs";
        var response = await HttpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            ErrorMessage = string.Format(AppResources.RequestFileDownloadFailed, response.ReasonPhrase);
            return;
        }

        var responseBody = await response.Content.ReadAsStringAsync();

        var outputs =
            JsonConvert.DeserializeObject<RunningHubResponseModel<List<RunningHubOutputResponseModel>>>(responseBody);

        if (outputs is null)
            throw new NullReferenceException();

        if (outputs.Msg != "success" || outputs.Data is null)
        {
            ErrorMessage = string.Format(AppResources.FailedToObtainDownloadFileLink, outputs.Msg);
            return;
        }
        
        await DownloadAsync(outputs.Data, entity.TaskId);
    }

    private async Task DownloadAsync(IEnumerable<RunningHubOutputResponseModel> data, string taskId)
    {
        var entities = new List<RunningHubResultEntity>();
        var savePath = RunningHubResourceService.Get();


        foreach (var item in data)
        {
            var fileName = Path.Combine(savePath, Path.GetFileNameWithoutExtension(item.FileUrl) +
                                                  Path.GetExtension(item.FileUrl));

            await HttpClient.DownloadFileAsync(item.FileUrl, fileName, null);

            entities.Add(new()
            {
                FileType = Path.GetExtension(item.FileUrl), SavePath = fileName, TaskId = taskId,
                CreateTime = DateTime.Now, UpdateTime = DateTime.Now
            });
        }

        if (await RunningHubResultService.AddRangeAsync(entities))
        {
            LastBuildResultImages.Reset(entities.Select(x => x.SavePath));
            return ;
        }

        ErrorMessage = AppResources.SaveFailed;

    }


    protected override async Task OnLoaded()
    {
        RunningHubStateManager.OnSuccess += TaskOkAsync;
        RunningHubStateManager.OnFailure += TaskFailAsync;
        RunningHubStateManager.OnBackgroundTaskCount += RunningTaskCountChangedAsync;

        await QueryIsRunningTaskCountAsync();

       var find = await this.RunningHubService.QueryAsync(x=> x.Email==user.Email);
       if (!find.Any())
       {
           await DialogServer.ShowMessageDialogAsync(AppResources.AddRunningHubApiKeyInSettings);
          return;
       }
       RunningHubEntity=find.First();


    }

    protected string ErrorMessage { get; set; } = string.Empty;

    private string SuccessMessage { get; set; } = AppResources.ExecutedSuccessfully;

    protected virtual Task<bool> ValidateConfirmAsync()
    {
        return Task.FromResult(true);
    }


    /// <summary>
    /// 发布任务
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected async  Task<bool> PublishTaskAsync(IEnumerable<NodeInfoListItem> data)
    {
        var parameter = new RunningHubTaskPublishResponseModel
        {
            ApiKey = RunningHubEntity!.ApiKey,
            WorkflowId = WorkFlowId,
            NodeInfoList = data
        };

        HttpClient.DefaultRequestHeaders.Clear();
        // 添加请求头
        HttpClient.DefaultRequestHeaders.Add("User-Agent", "Apifox/1.0.0 (https://apifox.com)");
        HttpClient.DefaultRequestHeaders.Add("Accept", "*/*");
        HttpClient.DefaultRequestHeaders.Add("Host", "www.runninghub.cn");
        HttpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

        // 序列化 JSON
        string json = JsonConvert.SerializeObject(parameter);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"{BaseAddress}/task/openapi/create";
        // 发送 POST 请求
        var response = await HttpClient.PostAsync(url, content);
        string result = await response.Content.ReadAsStringAsync();

        var model = JsonConvert.DeserializeObject<RunningHubResponseModel<RunningHubTaskResponseModel>>(result);

        if (model is null)
            throw new NullReferenceException();

        if (model.Msg == "success" && model.Data is not null)
        {
            // 将任务写入数据库并且开始轮询
            return await RunningHubStateManager.AddTaskAsync(RunningHubStateEntity.Create(model.Data.TaskId,
                model.Data.ClientId,
                RunningHubEntity.ApiKey, RunningHubWorkType,user.Email));
        }

        ErrorMessage = string.Format(AppResources.AnErrorOccurred, model.Msg);
        return false;
    }


    /// <summary>
    /// 获得正在进行的任务数量
    /// </summary>
    private async Task QueryIsRunningTaskCountAsync()
    {
        var count = await RunningHubStateService.CountAsync(x =>
            x.TaskStatus == TaskState.Running && x.RunningHubWorkType == RunningHubWorkType);
        WorkingTaskCount = count;
    }


    [ObservableProperty]
    private bool isBackgroundTaskRunning = true;

    [RelayCommand]
    private async Task Confirm()
    {
        if (RunningHubEntity is null)
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.AddRunningHubApiKeyInSettings);
            return;
        }

        if (await ValidateConfirmAsync())
        {
            if (await OnConfirmAsync())
            {
                await QueryIsRunningTaskCountAsync();
                await DialogServer.ShowMessageDialogAsync(SuccessMessage);
            }
            else
            {
                await DialogServer.ShowMessageDialogAsync(ErrorMessage);
            }

        }
        else
        {
            await DialogServer.ShowMessageDialogAsync(ErrorMessage);
        }



    }

    protected abstract Task<bool> OnConfirmAsync();

}