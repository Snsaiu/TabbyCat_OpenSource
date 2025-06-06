using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Interactivity;
using Avalonia.Threading;

using TabbyCat.ViewModels;

namespace TabbyCat.Views;

public partial class ChatView : UserControl
{
    public ChatView()
    {
        InitializeComponent();
       
        //TopLevel.GetTopLevel(this).InputPane.StateChanged += InputPane_StateChanged;
    }

    private void InputPane_StateChanged(object? sender, InputPaneStateEventArgs e)
    {

    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ChatViewModel chatViewModel) throw new NullReferenceException();

        chatViewModel.ChatItemChanged += () => Dispatcher.UIThread.Invoke(() =>
        {
            // if (IsScrolledToBottom())
            // 如果已经滚动到底部，则自动滚动到底部
            sv.ScrollToEnd();
        });
    }

    private bool IsScrolledToBottom()
    {
        // 获取 ScrollViewer 的 Offset、Extent 和 Viewport
        var offset = sv.Offset;
        var extent = sv.Extent;
        var viewport = sv.Viewport;

        // 判断是否滚动到底部
        return offset.Y + viewport.Height >= extent.Height;
    }
}