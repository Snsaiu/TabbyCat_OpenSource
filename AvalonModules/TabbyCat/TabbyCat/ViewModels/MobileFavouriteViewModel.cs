using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Service.AiServices;
using TabbyCat.ViewModels.Bases;
using TuDog.Extensions;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;


[Register]
public partial class MobileFavouriteViewModel( IAiChatMessageRecordService aiChatMessageRecordService,
    ILogger<MobileFavouriteViewModel> logger,
    IUseMarkdownService markdownService) : ViewModelBase
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
            var items = item.Select(x => new MessagesItem()
            {
                Key = x.Key, Content = x.Content, IsFavourite = x.IsFavourite,
                Role = x.Role, ShowMarkdownMode = showMarkdown
            });
            var model = new ChatMessageDateGroupModel(item.Key, items);
            list.Add(model);

        }

        Chats.Reset(list);
    }

}