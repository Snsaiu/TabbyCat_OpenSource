using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models;
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

    protected IOccupationService OccupationService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IOccupationService>();

    [ObservableProperty] private ObservableCollection<OccupationType> occupations = [];

    [ObservableProperty] private OccupationType? selectedOccupation = null;

    protected override async Task OnLoaded()
    {
        Occupations.Reset(await OccupationService.GetAllOccupationsAsync());
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

        var sessions = await this.chatSessionService.QueryAsync(x =>
            x.Occupation == AssistantOccupation.Custom && x.CustomOccupationName == SelectedOccupation.OccupationName);

        if (sessions.Any())
        {
          await  DialogServer.ShowMessageDialogAsync(AppResources.HasChatHistoryCannotDeleteContact, AppResources.Warning,
                AppResources.Ok);
          return;
        }
        

        if (!(await this.DialogServer.ShowConfirmDialogAsync(
                string.Format(AppResources.ConfirmDelete, SelectedOccupation.OccupationName), AppResources.Warning,
                AppResources.Ok, AppResources.Cancel)))
            return;

        await customAssistantOccupationService.DeleteAsync(x =>
            x.Name == SelectedOccupation.OccupationName);
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