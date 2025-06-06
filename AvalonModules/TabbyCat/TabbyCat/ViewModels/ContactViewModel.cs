using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Components.ViewModels;
using TabbyCat.Models;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.Extensions;
using TuDog.Interfaces.MessageBarService;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class ContactViewModel(IMessageBarService messageBarService) : ContactViewModelBase
{
    private OccupationType? _selectedOccupation;

    protected override async void OnOccupationSelectedChanged(OccupationType? value)
    {
        SelectedOccupation = value;
        if(value is null)
            return;
        if (value.Occupation == AssistantOccupation.Custom)
        {
            var finds = await customAssistantOccupationService.QueryAsync(x =>
                x.Name == value.OccupationName );
            if (finds.Any())
                OccupationDescription = finds.FirstOrDefault()?.Description;
        }
        else
        {
           OccupationDescription  =   LocalizationResourceManager.Instance[$"{value.Occupation.ToString()}Prompt"];
        }

    }

    protected override async Task OnLoaded()
    {
        await base.OnLoaded();
        Source.Reset(Occupations);
        SelectedOccupation = Occupations.FirstOrDefault();
    }

    [ObservableProperty] private ObservableCollection<OccupationType> _source = [];

    [ObservableProperty] private string _searchText;

    [ObservableProperty]
    private string _occupationDescription = string.Empty;

    partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
            Source.Reset(Occupations);
        else
            Source.Reset(Occupations.Where(x => x.OccupationName.Contains(value, StringComparison.OrdinalIgnoreCase)));
    }

    protected override Task OnSearchAsync()
    {
        return Task.CompletedTask;
    }

    protected override async Task OnAddNewOccupationAsync()
    {
        var result =
            await DialogServer.ShowDialogAsync<AddNewOccupationViewModel, bool, CustomAssistantOccupationEntity>(
                AppResources.AddContact,
                AppResources.Ok, AppResources.Cancel, false);
        if (result.Ok == false)
            return;
        Occupations.Reset(await OccupationService.GetAllOccupationsAsync());
        Source.Reset(Occupations);
        SelectedOccupation = Source.FirstOrDefault();
    }

    protected override Task OnDeletedContactAsync()
    {
        Source.Reset(Occupations);
        SelectedOccupation = Source.FirstOrDefault();
        messageBarService.ShowSuccess(AppResources.DeleteSuccess, AppResources.Message, true);
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task GoChat()
    {
        if (SelectedOccupation is null)
            return;
        
        // 添加新的会话
        var sessions = await this.chatSessionService.QueryAsync(x => x.IsDefault);
        if (sessions.Any())
        {
            foreach (var item in sessions)
            {
                item.IsDefault = false;
            }

            await this.chatSessionService.UpdateRangeAsync(sessions);
        }
        
        var newSession = SelectedOccupation.Occupation == AssistantOccupation.Custom ? AiChatSessionEntity.CreateCustom(SelectedOccupation.OccupationName) : AiChatSessionEntity.CreateDefault(SelectedOccupation.Occupation);
            
        await chatSessionService.AddAsync(newSession);
        
        NavigationMenuItemService.SelectMenuItem =
            NavigationMenuItemService.MenuItems.FirstOrDefault(x => x.Content == typeof(ChatViewModel));
       
    }

}