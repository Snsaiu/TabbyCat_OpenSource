using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Repository.Entities.RunningHubEntities;
using TabbyCat.Service.RunningHubServices;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels;
using TuDog.IocAttribute;

namespace TabbyCat.Components.ViewModels;

[Register]
public partial class RunningHubSettingViewModel(
    IRunningHubService runningHubService,
    IUser user,
    IRunningHubResourceService runningHubResourceService) : ViewModelBase
{
    private RunningHubEntity? _runningHub;

    [ObservableProperty] private string saveMediaPath = string.Empty;
    protected override async Task OnLoaded()
    {
        var query = await runningHubService.QueryAsync(x=> x.Email==user.Email);
        if (query.Any())
        {
            _runningHub = query.First();
            ApiKey=_runningHub.ApiKey;
        }

        SaveMediaPath = runningHubResourceService.Get();
    }

    [ObservableProperty]
    private string apiKey = string.Empty;


    [RelayCommand]
    private async Task Save()
    {
        if (SaveMediaPath == string.Empty)
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.SavePathCannotBeEmpty,AppResources.Warning,AppResources.Ok);
            return;
        }

        runningHubResourceService.Set(SaveMediaPath);

        //插入
        if (_runningHub == null)
        {
            _runningHub = new RunningHubEntity()
                { ApiKey = ApiKey, CreateTime = DateTime.Now, UpdateTime = DateTime.Now,Email = user.Email};
            if (await runningHubService.AddAsync(_runningHub))
            {
                await DialogServer.ShowMessageDialogAsync(AppResources.SavedSuccessfully, AppResources.Message,AppResources.Ok);
            }
            else
            {
                await DialogServer.ShowMessageDialogAsync(AppResources.SaveFailed,AppResources.Warning,AppResources.Ok);
            }
        }
        else
        {
            var query = await runningHubService.QueryAsync(x=> x.Email==user.Email);
            if (query.Any())
            {
                _runningHub.Key = query.First().Key;
                _runningHub.ApiKey = ApiKey;
                _runningHub.UpdateTime = DateTime.Now;
                if (await runningHubService.UpdateAsync(_runningHub))
                {
                    await DialogServer.ShowMessageDialogAsync(AppResources.UpdatedSuccessfully, AppResources.Message,AppResources.Ok);
                }
                else
                {
                    await DialogServer.ShowMessageDialogAsync(AppResources.UpdateFailed,AppResources.Warning,AppResources.Ok);
                }

            }
            else
            {
                await DialogServer.ShowMessageDialogAsync(AppResources.SaveFailed,AppResources.Warning,AppResources.Ok);
            }
        }
    }
}