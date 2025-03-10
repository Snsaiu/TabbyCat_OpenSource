using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Markdig;
using TabbyCat.Models.AiReqRes.AiChatRequests;

namespace TabbyCat.Controls;

public partial class ChatItem : UserControl
{
    // 创建依赖属性
    public static readonly StyledProperty<MessagesItem> MessageProperty = AvaloniaProperty.Register<ChatItem, MessagesItem>(nameof(Message));

    private ICommand _favouriteCommand;

    public static readonly DirectProperty<ChatItem, ICommand> FavouriteCommandProperty =
        AvaloniaProperty.RegisterDirect<ChatItem, ICommand>(
            nameof(FavouriteCommand), o => o.FavouriteCommand, (o, v) => o.FavouriteCommand = v);

    public ICommand FavouriteCommand
    {
        get => _favouriteCommand;
        set => SetAndRaise(FavouriteCommandProperty, ref _favouriteCommand, value);
    }


    public MessagesItem Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }


    public ChatItem()
    {
        InitializeComponent();
    }

    [RelayCommand]
    private Task CopyTextToClipboard()
    {
        return App.TopLevel.Clipboard.SetTextAsync(Markdown.ToPlainText(Message.Content));
    }

    private void ToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (FavouriteCommand?.CanExecute(Message) == true)
            FavouriteCommand?.Execute(Message);
    }

}