using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Service.RunningHubServices;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels;
using TuDog.IocAttribute;
using ViewModelBase = TabbyCat.ViewModels.Bases.ViewModelBase;

namespace TabbyCat.Components.ViewModels;

[Register]
public sealed partial class MediaSettingViewModel(
    IAiMediaService aiMediaService,
    IRunningStateService runningStateService,
    ILogger<MediaSettingViewModel> logger,
    IAiMediaResourceService runningHubResourceService) : ViewModelBase

{
    [ObservableProperty] private string saveMediaPath = string.Empty;

    protected override async Task OnLoaded()
    {
        SaveMediaPath = runningHubResourceService.Get();
    }

    [RelayCommand]
    private async Task Save()
    {
        var runningTasks = await runningStateService.QueryAsync(x => x.TaskStatus == TaskState.Running);
        if (runningTasks.Any())
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.TaskRunningNoSet, AppResources.Warning,
                AppResources.Ok);
            return;
        }

        if (SaveMediaPath == string.Empty || !Directory.Exists(saveMediaPath))
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.SavePathCannotBeEmpty, AppResources.Warning,
                AppResources.Ok);
            return;
        }

        var isMigration = await DialogServer.ShowConfirmDialogAsync(AppResources.DoYouWantToMigrateResources,
            AppResources.Warning, AppResources.Ok, AppResources.Cancel);

        if (!isMigration)
        {
            runningHubResourceService.Set(SaveMediaPath);
            return;
        }

        var entities = await aiMediaService.QueryAsync();
        var oldPath = runningHubResourceService.Get();

        foreach (var item in entities)
            try
            {
                var newPath = item.SavePath.Replace(oldPath, SaveMediaPath);
                if (!File.Exists(item.SavePath))
                {
                    await aiMediaService.DeleteAsync(x => x.Key == item.Key);
                    continue;
                }

                File.Move(item.SavePath, newPath);
                item.SavePath = newPath;
                await aiMediaService.UpdateAsync(item);
            }
            catch (Exception e)
            {
                logger.LogError(e, "迁移文件发生错误");
            }
        runningHubResourceService.Set(SaveMediaPath);
        await DialogServer.ShowMessageDialogAsync(AppResources.MigrationSuccessful, AppResources.Message,
            AppResources.Ok);
    }
}