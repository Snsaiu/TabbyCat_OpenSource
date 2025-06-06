using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Data;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Service.AiServices;
using TuDog.Bootstrap;
using TuDog.Extensions;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class ChatMessageFavoritesViewModel(
    IAiChatMessageRecordService aiChatMessageRecordService,
    ILogger<ChatMessageFavoritesViewModel> logger,
    IUseMarkdownService markdownService)
    : DialogViewModelBaseAsync<bool,bool>
{
    [ObservableProperty] private ObservableCollection<ChatMessageDateGroupModel> chats = [];

    [ObservableProperty] private string searchText=string.Empty;


    [RelayCommand]
    private async Task SetIsFavouriteState(MessagesItem item)
    {
        var query = await aiChatMessageRecordService.QueryAsync(x => x.Key == item.Key);
        if (!query.Any())
        {
            logger.LogError("根据{0}查不到聊天记录", item.Key);
            return;
        }

        var find = query.First();
        find.UpdateTime = DateTime.Now;
        find.IsFavourite = item.IsFavourite;
        await aiChatMessageRecordService.UpdateAsync(find);
    }


    [RelayCommand]
    private Task Fold()
    {
        foreach (var item in Chats) item.Expanded = false;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task Expand()
    {
        foreach (var item in Chats) item.Expanded = true;
        return Task.CompletedTask;
    }

    partial void OnSearchTextChanged(string value)
    {
        foreach (var item in Chats)
        {
            var filters = item.Filter(value);
            item.Items.Reset(filters);
        }

    }

    protected override async Task OnLoaded()
    {
        var showMarkdown = markdownService.Get();

        var finds = await aiChatMessageRecordService.QueryAsync(x => x.IsFavourite);
        if (!finds.Any())
            return;

        var group = finds.GroupBy(x => DateOnly.FromDateTime(x.CreateTime)).OrderByDescending(x => x.Key);
        var list = new List<ChatMessageDateGroupModel>();
        foreach (var item in group)
        {
            var items = new List<MessagesItem>();
            foreach (var chat in item)
            {
                var message = new MessagesItem()
                {
                    Key = chat.Key, Content = chat.Content, IsFavourite = chat.IsFavourite,
                    Role = chat.Role, ShowMarkdownMode = showMarkdown
                };
                if (!string.IsNullOrEmpty(chat.Appendix))
                    message.Appendixes = JsonConvert.DeserializeObject<IEnumerable<AppendixModel>>(chat.Appendix);
                items.Add(message);
            }

            var model = new ChatMessageDateGroupModel(item.Key, items);
            list.Add(model);

        }

        Chats.Reset(list);
    }


    public override Task<bool> ConfirmAsync()
    {
        return Task.FromResult(true);
    }

    public override Task<bool> CancelAsync()
    {
      return Task.FromResult(false);
    }
}