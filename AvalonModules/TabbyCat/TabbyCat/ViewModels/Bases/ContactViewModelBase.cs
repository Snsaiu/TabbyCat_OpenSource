using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TuDog.Bootstrap;
using TuDog.Extensions;
using TuDog.Interfaces.Navigations;

namespace TabbyCat.ViewModels.Bases;

/// <summary>
/// 联系人基类
/// </summary>
public abstract partial class ContactViewModelBase : AiViewModelBase
{
    protected INavigationService NavigationService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<INavigationService>();

    protected ICustomOccupationSyncService CustomOccupationSyncService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<ICustomOccupationSyncService>();

    protected ICustomOccupationHubSyncManager CustomOccupationSyncManager { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<ICustomOccupationHubSyncManager>();

    protected IOccupationService OccupationService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IOccupationService>();

    [ObservableProperty] private ObservableCollection<OccupationType> occupations = [];

    [ObservableProperty] private OccupationType? selectedOccupation = null;

    [ObservableProperty] private ObservableCollection<OccupationType> _source = [];

    protected override Task OnLoaded()
    {
        CustomOccupationSyncManager.UpdatedCallBack += OnUpdatedLocalDbCallback;
        return ResetOccupationsAsync();
    }

    protected override Task OnUnLoaded()
    {
        CustomOccupationSyncManager.UpdatedCallBack -= OnUpdatedLocalDbCallback;
        return base.OnUnLoaded();
    }

    private Task OnUpdatedLocalDbCallback(IEnumerable<CustomAssistantOccupationEntity> models)
    {
        return ResetOccupationsAsync();
    }

    /// <summary>
    /// 重新读取所有的角色
    /// </summary>
    protected async Task ResetOccupationsAsync()
    {
        Occupations.Reset(await OccupationService.GetAllOccupationsAsync());
        Source.Reset(Occupations);
        SelectedOccupation = Source.FirstOrDefault();
    }

    partial void OnSelectedOccupationChanged(OccupationType? value)
    {
        OnOccupationSelectedChanged(value);
    }

    [RelayCommand]
    private Task Search()
    {
        return OnSearchAsync();
    }

    [RelayCommand]
    private Task AddNewOccupation()
    {
        return OnAddNewOccupationAsync();
    }

    [RelayCommand]
    private async Task DeleteContact()
    {
        if (SelectedOccupation == null)
            return;

        var sessions = await ChatSessionService.QueryAsync(x =>
            x.Occupation == AssistantOccupation.Custom && x.CustomOccupationName == SelectedOccupation.OccupationName &&
            x.Email == CurrentUser.Email);

        if (sessions.Any())
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.HasChatHistoryCannotDeleteContact,
                AppResources.Warning,
                AppResources.Ok);
            return;
        }

        if (!await DialogServer.ShowConfirmDialogAsync(
                string.Format(AppResources.ConfirmDelete, SelectedOccupation.OccupationName), AppResources.Warning,
                AppResources.Ok, AppResources.Cancel))
            return;

        var finds = (await CustomAssistantOccupationService.QueryAsync(x =>
            x.Name == SelectedOccupation.OccupationName && x.Email == CurrentUser.Email && !x.IsDeleted)).ToArray();

        if (!finds.Any())
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.AnErrorOccurred, AppResources.Warning,
                AppResources.Ok);
            return;
        }

        var selected = finds.First();
        selected.IsDeleted = true;
        await CustomAssistantOccupationService.UpdateAsync(selected);

        // 同步
        if (CurrentUser.LoginSuccess())
        {
            //查询最新的版本
            var lastestVersionResult = await CustomOccupationSyncService.QueryLatestVersionAsync();
            if (lastestVersionResult is not { Ok: true, Data: var version })
            {
                await DialogServer.ShowMessageDialogAsync(lastestVersionResult.ErrorMsg, AppResources.Warning,
                    AppResources.Ok);
                goto LocalDelete;
            }

            var latestVersion = version + 1;
            var updateTime = DateTime.Now;
            var occupations = (await CustomAssistantOccupationService.QueryAsync(x => x.Email == CurrentUser.Email))
                .ToArray();

            foreach (var item in occupations)
            {
                item.LastUpdateTime = updateTime;
                item.Version = latestVersion;
            }

            await CustomOccupationSyncService.UploadRemoteAsync(occupations);
            await CustomAssistantOccupationService.UpdateRangeAsync(occupations);
        }

        LocalDelete:
        Occupations.Reset(await OccupationService.GetAllOccupationsAsync());
        SelectedOccupation = Occupations.FirstOrDefault();
        await OnDeletedContactAsync();
    }

    protected virtual Task OnDeletedContactAsync()
    {
        return Task.CompletedTask;
    }

    protected abstract void OnOccupationSelectedChanged(OccupationType? value);

    protected abstract Task OnSearchAsync();

    protected abstract Task OnAddNewOccupationAsync();
}