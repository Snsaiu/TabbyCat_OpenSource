using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Interactivity;
using Avalonia.Threading;
using TabbyCat.Extensions;
using TabbyCat.ViewModels;

namespace TabbyCat.Views;

public partial class ChatView : UserControl
{
    public ChatView()
    {
        InitializeComponent();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ChatViewModel chatViewModel) throw new NullReferenceException();

        chatViewModel.ChatItemChanged += () => Dispatcher.UIThread.Invoke(() =>
        {
            // if (IsScrolledToBottom())
            // 如果已经滚动到底部，则自动滚动到底部
            if (chatViewModel.ScrollToEnd)
                sv.ScrollToEnd();
        });

        sv.ScrollChanged += (s, e) =>
        {
            if (e.OffsetDelta.Y < 0)
                chatViewModel.ScrollToEnd = false;
        };
    }
}