using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Data;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TuDog.Bootstrap;
using TuDog.Extensions;

namespace TabbyCat.Models;

public partial class ChatMessageDateGroupModel : ModelBase
{
    [ObservableProperty] private DateOnly date;

    [ObservableProperty] private ObservableCollection<MessagesItem> items = [];

    [ObservableProperty] private bool expanded;

    private CollectionViewSource filter = new();

    public ChatMessageDateGroupModel(DateOnly dateOnly, IEnumerable<MessagesItem> source)
    {
        Date = dateOnly;
        Items.Reset(source);
        filter.Source = source;
    }

    public IEnumerable<MessagesItem> Filter(string input)
    {

        filter.Filter += x =>
        {
            if (string.IsNullOrEmpty(input))
                return true;
            var item = (MessagesItem)x;
            return item.Content.Contains(input);
        };
        return filter.View.OfType<MessagesItem>();
    }
}