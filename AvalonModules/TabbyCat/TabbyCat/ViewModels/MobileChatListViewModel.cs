using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.Extensions;
using TuDog.Interfaces.Navigations;
using TuDog.Interfaces.Navigations.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class MobileChatListViewModel(
    INavigationService navigationService,
    IAiChatSessionService aiChatSessionService,
    IStoreChatRecordService storeChatRecordService,
    IAiChatMessageRecordService aiChatMessageRecordService) : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<AiChatSessionEntity> _chatList = [];

    [ObservableProperty] private AiChatSessionEntity? _selectedChat;

    private bool storeChatRecord;

    async partial void OnSelectedChatChanged(AiChatSessionEntity? value)
    {
        if (value == null)
            return;
        
        value.IsDefault = true;

        var defaultSessions =
            await aiChatSessionService.QueryAsync(x => x.IsDefault );

        if (defaultSessions.Any())
        {
            foreach (var item in defaultSessions)
            {
                item.IsDefault = false;
            }

            await aiChatSessionService.UpdateRangeAsync(defaultSessions);
        }

        await aiChatSessionService.UpdateAsync(value);

        var parameter = new NavigationParameter();
        parameter.Add("New", false);
        await navigationService.PushAsync<ChatViewModel>(parameter);
    }

    [RelayCommand]
    private async Task DeleteChatAsync(AiChatSessionEntity? chat)
    {
        if (chat is null)
            return;

        var confirmDelete = await DialogServer.ShowConfirmDialogAsync(AppResources.ConfirmDeleteSelectedSession,AppResources.Message,AppResources.Ok,AppResources.Cancel);
        if (confirmDelete == false)
            return;
        if (await aiChatSessionService.DeleteAsync(x => x.Key == chat.Key) is not null)
        {
            var sessionId = chat.Key;
            ChatList.Remove(chat);
            if (!storeChatRecord) await aiChatMessageRecordService.DeleteRangeAsync(x => x.SessionId == sessionId);
            
        }
        else
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.DeleteFailed,AppResources.Warning,AppResources.Ok);
        }
    }

    protected override async Task OnLoaded()
    {
        this.ChatList.Reset((await aiChatSessionService.QueryAsync()).OrderByDescending(x=>x.UpdateTime));
        storeChatRecord = storeChatRecordService.Get();
    }
}