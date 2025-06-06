using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices;
using TabbyCat.Views;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IShowQuickMenuItemWindowService>(ServiceLifetime.Singleton)]
public sealed class ShowQuickMenuItemWindowService : IShowQuickMenuItemWindowService
{
    private QuickMenuItemWindow _window;

    public ShowQuickMenuItemWindowService(ILogger<ShowQuickMenuItemWindowService> logger)
    {
        _window = new QuickMenuItemWindow();
        _window.CanResize = false;
        _window.ShowInTaskbar = false;
        _window.Topmost = true;
        _window.Focusable = false;
        _window.SizeToContent = SizeToContent.WidthAndHeight;
        _window.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
        _window.ExtendClientAreaToDecorationsHint = true;
    }

    public Task ShowWindowAsync(string content, Point position)
    {
        if (_window.IsVisible)
            return Task.CompletedTask;
        
        _window.Init(content);
        _window.Position = new PixelPoint((int)position.X, (int)position.Y-60);
        _window.ShowActivated = false;
        _window.Show();
        return Task.CompletedTask;
    }

    public Task HideWindowAsync(Point position)
    {
        var mousePosition = _window.Screens.ScreenFromPoint(new PixelPoint((int)position.X, (int)position.Y));

        if (mousePosition != null)
        {
            // 将全局坐标转换为窗口内坐标
            var localPosition = _window.PointToClient(new PixelPoint((int)position.X, (int)position.Y));

            // 判断是否在窗口内
            if (!_window.Bounds.Contains(localPosition)&& !_window.IsActive)
                Dispatcher.UIThread.Invoke(() =>
                {
                    _window.Hide();
                });
               
        }

        return Task.CompletedTask;
    }

    public Task HideWindowAsync()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            if(!_window.IsActive)
                _window.Hide();
        });
        return Task.CompletedTask;
    }
}