using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyResultModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Repository.Entities.AiMediaEntities;
using TabbyCat.Service.RunningHubServices;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TuDog.Bootstrap;
using TuDog.Extensions;

namespace TabbyCat.ViewModels.Bases;

public abstract partial class AiMediaViewModelBase : ViewModelBase
{
    [ObservableProperty] private bool _isBackgroundTaskRunning = false;

    [ObservableProperty] private int _workingTaskCount;

    [ObservableProperty] private bool _showPanel = false;

    protected ILogger<AiMediaViewModelBase> Logger { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<AiMediaViewModelBase>();

    protected INavigationMenuItemService NavigationMenuItemService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<INavigationMenuItemService>();

    protected Task<ResultBase<string>> UploadImageAsync(string fileName)
    {
        return _remoteServerService.UploadImageAsync(fileName);
    }


    protected async Task<string> GetUploadImageUrlAsync(string fileName)
    {
        var result = await UploadImageAsync(fileName);
        return result.Ok ? result.Data : throw new(result.ErrorMsg);
    }


    /// <summary>
    /// 最后一次生成的多媒体文件
    /// </summary>
    [ObservableProperty] private ObservableCollection<string> _lastBuildResultMedia = [];


    [ObservableProperty] private ObservableCollection<AiMediaResultEntity> _imageCollectionViewSource = [];

    [ObservableProperty] private ObservableCollection<AiMediaResultEntity> _videoCollectionViewSource = [];

    [ObservableProperty] private IList selectedMediaFiles;
    
    [ObservableProperty]
    private IList _selectedResultMediaEntities;

    protected abstract AiMediaWorkType RunningHubWorkType { get; }


    protected IAiMediaService AiMediaService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiMediaService>();

    protected IRunningStateService RunningStateService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IRunningStateService>();

    protected IAiMediaResourceService AiMediaResourceService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiMediaResourceService>();

    protected IAiMediaRunningStateManager RunningHubStateManager { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiMediaRunningStateManager>();

    protected AiMediaResultEntity? AiMediaResultEntity { get; set; }

    protected IAiMediaResultService AiMediaResultService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiMediaResultService>();

    protected HttpClient HttpClient { get; private set; }

    protected IRemoteServerService _remoteServerService =
        TuDogApplication.ServiceProvider.GetRequiredService<IRemoteServerService>();

    public AiMediaViewModelBase()
    {
        var httpFactory = TuDogApplication.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        HttpClient = httpFactory.CreateClient();
    }

    protected abstract Task ConfirmAsync();

    /// <summary>
    /// 获得正在进行的任务数量
    /// </summary>
    protected async Task QueryIsRunningTaskCountAsync()
    {
        var count = await RunningStateService.CountAsync(x =>
            x.TaskStatus == TaskState.Running && x.WorkType == RunningHubWorkType);
        WorkingTaskCount = count;
    }

    [RelayCommand]
    private Task Confirm()
    {
        return ConfirmAsync();
    }


    [RelayCommand]
    private Task ClosePortfolioPanel()
    {
        ShowPanel = false;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OpenImageByDefaultProgram(AiMediaResultEntity selected)
    {
        return App.TopLevel.Launcher.LaunchUriAsync(new(selected.SavePath));
    }

    [RelayCommand]
    private async Task SaveFileToLocal(AiMediaResultEntity selected)
    {
        var fileName = Path.GetFileName(selected.SavePath);

        var saveLocation = await App.TopLevel.StorageProvider.SaveFilePickerAsync(new()
            { SuggestedFileName = fileName, ShowOverwritePrompt = true, DefaultExtension = selected.FileType });
        if (saveLocation is null)
            return;
        File.Copy(selected.SavePath, saveLocation.Path.LocalPath, true);
        await DialogServer.ShowMessageDialogAsync(AppResources.ExportedSuccessfully);
    }

    [RelayCommand]
    private async Task ClearTasks()
    {
        if (!await DialogServer.ShowConfirmDialogAsync(AppResources.DoYouWantToClearRunningTasks, AppResources.Warning,
                AppResources.Ok))
            return;
        await RunningHubStateManager.ClearTasksAsync();
        await QueryIsRunningTaskCountAsync();
    }


    /// <summary>
    /// 删除媒体
    /// </summary>
    /// <param name="selected"></param>
    [RelayCommand]
    private async Task DeleteMedia(AiMediaResultEntity selected)
    {
        var deleteConfirm = await DialogServer.ShowConfirmDialogAsync(AppResources.ConfirmDeleteItem);
        if (!deleteConfirm)
            return;

        if (await AiMediaResultService.DeleteAsync(x => x.Key == selected.Key) is not null)
        {
            if (selected.FileType == ".png")
            {
                if (File.Exists(selected.SavePath)) File.Delete(selected.SavePath);
                ImageCollectionViewSource.Remove(selected);
            }
            else if (selected.FileType == ".mp4")
            {
                if (File.Exists(selected.SavePath)) File.Delete(selected.SavePath);
                if (File.Exists(selected.ThumbnailPath)) File.Delete(selected.ThumbnailPath);
                VideoCollectionViewSource.Remove(selected);
            }
        }
    }

    [RelayCommand]
    private Task SendTo(AiMediaWorkType workType)
    {
        List<string> files = new();
        foreach (var file in SelectedMediaFiles)
        {
            if (file is null)
                continue;
            files.Add(file.ToString());
        }

        return this.NavigationMenuItemService.NavigationAsync(workType, files.AsReadOnly());
    }


    [RelayCommand]
    private Task ListSendTo(AiMediaWorkType workType)
    {
        List<string> files = new();
        foreach (var file in SelectedResultMediaEntities)
        {
            if (file is null)
                continue;
            var f = (AiMediaResultEntity)file;
            files.Add(f.SavePath);
        }

        return this.NavigationMenuItemService.NavigationAsync(workType, files.AsReadOnly());
    }

    /// <summary>
    /// 获得结果集
    /// </summary>
    protected async Task<IEnumerable<AiMediaResultEntity>> ResetResultsAsync()
    {
        var list = (await AiMediaResultService.QueryAsync())
            .OrderByDescending(x => x.UpdateTime);
        ObservableCollectionExtension.Reset(VideoCollectionViewSource, list.Where(x => x.FileType == ".mp4"));
        ObservableCollectionExtension.Reset(ImageCollectionViewSource, list.Where(x => x.FileType == ".png"));
        return list;
    }


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

    [RelayCommand]
    private async Task Look()
    {
        foreach (var item in SelectedMediaFiles)
        {
            var file = item.ToString();
            if (!File.Exists(file))
                continue;
            
            await App.TopLevel.Launcher.LaunchUriAsync(new Uri(item.ToString()));
        }
    }
}

public abstract partial class AiMediaViewModelBase<TPublishModel, TInput, TParameters> : AiMediaViewModelBase
    where TPublishModel : AiMediaRequestModelBase<TInput, TParameters>
{
    protected override Task OnUnLoaded()
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        RunningHubStateManager.OnSuccess -= TaskOkAsync;
        RunningHubStateManager.OnFailure -= TaskFailAsync;
        RunningHubStateManager.OnBackgroundTaskCount -= RunningTaskCountChangedAsync;
#pragma warning restore CS8601 // Possible null reference assignment.
        return Task.CompletedTask;
    }

    private async Task TaskOkAsync(string key, AiMediaWorkType workType)
    {
        await QueryIsRunningTaskCountAsync();
        var list = await ResetResultsAsync();
        var temps = list.Where(x => x.WorkType == workType && x.TaskId == key)
            .Select(x => x.SavePath);
        ObservableCollectionExtension.Reset(LastBuildResultMedia, temps);
    }


    private Task RunningTaskCountChangedAsync(int count)
    {
        IsBackgroundTaskRunning = count > 0;
        return Task.CompletedTask;
    }

    private Task TaskFailAsync(string key)
    {
        return Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.TaskErrorResetPlease, AppResources.Warning,
                AppResources.Ok);
            await QueryIsRunningTaskCountAsync();
        });
    }


    protected override async Task ConfirmAsync()
    {

        if (_apiKey is null)
        {
            var queryKey = await _remoteServerService.GetAiKeyAsync();
            if (!queryKey.Ok)
            {
                await DialogServer.ShowMessageDialogAsync(queryKey.ErrorMsg, AppResources.Warning, AppResources.Ok);
                return;
            }

            _apiKey = queryKey.Data;
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

    protected abstract string DownloadFileExtension { get; }


    protected virtual Dictionary<string, string> AddHttpHeaders()
    {
        return new Dictionary<string, string>();
    }

    private string _apiKey = string.Empty;

    protected override async Task OnLoaded()
    {
        var keyResult = await _remoteServerService.GetAiKeyAsync();
        if (!keyResult.Ok)
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.GetKeyError, AppResources.Message, AppResources.Ok);
            return;
        }

        _apiKey = keyResult.Data;

        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {keyResult.Data}");
        HttpClient.DefaultRequestHeaders.Add("X-DashScope-Async", "enable");

        foreach (var item in AddHttpHeaders())
        {
            HttpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
        }


        RunningHubStateManager.OnSuccess += TaskOkAsync;
        RunningHubStateManager.OnFailure += TaskFailAsync;
        RunningHubStateManager.OnBackgroundTaskCount += RunningTaskCountChangedAsync;

        await QueryIsRunningTaskCountAsync();

        var find = await this.AiMediaService.QueryAsync();

        AiMediaResultEntity = find.FirstOrDefault();
        var lastRunTask =
            (await RunningStateService.QueryAsync(x =>
             x.WorkType == RunningHubWorkType && x.TaskStatus == TaskState.Success))
            .OrderByDescending(x => x.UpdateTime).FirstOrDefault();
        if (lastRunTask != null)
            ObservableCollectionExtension.Reset(LastBuildResultMedia,
                (await AiMediaResultService.QueryAsync(x => x.TaskId == lastRunTask.TaskId)).Select(x => x.SavePath));
    }

    protected string ErrorMessage { get; set; } = string.Empty;

    private string SuccessMessage { get; set; } = AppResources.ExecutedSuccessfully;

    protected virtual Task<bool> ValidateConfirmAsync()
    {
        return Task.FromResult(true);
    }


    protected abstract string CreateTaskUrl { get; }

    /// <summary>
    /// 发布任务
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected async Task<bool> PublishTaskAsync(TPublishModel data)
    {
        // 序列化 JSON
        var json = JsonConvert.SerializeObject(data);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        Logger.LogDebug("发布任务请求json:{json}", json);

        // 发送 POST 请求
        var response = await HttpClient.PostAsync(CreateTaskUrl, content);
        var result = await response.Content.ReadAsStringAsync();

        var model = JsonConvert.DeserializeObject<CreateAiMediaTaskResponseModel>(result);

        if (model is null)
            throw new NullReferenceException();

        Logger.LogDebug("获得返回任务json:{json}", result);

        if (model.Output.TaskStatus is AiMediaRunningStatus.Failed or AiMediaRunningStatus.Unknown ||
            string.IsNullOrEmpty(model.Output.TaskId))
        {
            ErrorMessage = string.Format(AppResources.AnErrorOccurred, AppResources.AnErrorOccurred);
            return false;
        }

        return await RunningHubStateManager.AddTaskAsync(
            AiMediaRunningStateEntity.Create(model.Output.TaskId, _apiKey, RunningHubWorkType, string.Empty,
                DownloadFileExtension));
    }

    protected virtual async Task<bool> OnConfirmAsync()
    {
        try
        {
            var model = await CreatePublishModelAsync();
            return await PublishTaskAsync(model);
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            return false;
        }
    }

    protected abstract Task<TPublishModel> CreatePublishModelAsync();
}