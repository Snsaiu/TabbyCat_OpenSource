using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TabbyCat.ViewModels;

namespace TabbyCat.Views;

public partial class FloatingFrameView : UserControl
{
    private StackPanel stackPanel;
    private ScrollViewer scrollViewer;

    private FloatingFrameViewModel vm;

    public FloatingFrameView()
    {
        InitializeComponent();

        var togglebutton = this.GetControl<Button>("toggle");
        stackPanel = this.GetControl<StackPanel>("contentPanel");
        scrollViewer = this.GetControl<ScrollViewer>("sv");
        stackPanel.IsVisible = false;

        togglebutton.AddHandler(PointerPressedEvent, Toggle_PointerPressed, RoutingStrategies.Tunnel);
        togglebutton.AddHandler(PointerReleasedEvent, Toggle_PointerReleased, RoutingStrategies.Tunnel);
    }


    private Point? _dragStartPoint;

    private void Toggle_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
        {
            _dragStartPoint = e.GetPosition(null);
            var window = (sender as Visual)?.GetVisualRoot() as Window;
            window?.BeginMoveDrag(e);
        }
    }

    private void Toggle_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_dragStartPoint.HasValue)
        {
            var endPoint = e.GetPosition(null);
            var delta = endPoint - _dragStartPoint.Value;
            var distance = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);

            const double dragThreshold = 0.1;

            if (distance < dragThreshold) vm.ShowOutputPanel = !vm.ShowOutputPanel;

            _dragStartPoint = null;
        }
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not FloatingFrameViewModel chatViewModel) throw new NullReferenceException();

        vm = chatViewModel;

        chatViewModel.OnAiResponseChanged += () =>
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                // if (IsScrolledToBottom())
                // 如果已经滚动到底部，则自动滚动到底部
                if (chatViewModel.ToEnd)
                    scrollViewer.ScrollToEnd();
            });
        };
        scrollViewer.ScrollChanged += (s, e) =>
        {
            if (e.OffsetDelta.Y < 0)
                chatViewModel.ToEnd = false;
        };
    }
}