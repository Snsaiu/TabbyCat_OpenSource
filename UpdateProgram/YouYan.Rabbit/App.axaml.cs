using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HotAvalonia;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Platform;
using Avalonia.Threading;
using SharpHook;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;
using YouYan.Rabbit.ViewModels;
using YouYan.Rabbit.Views;

namespace YouYan.Rabbit;

public partial class App : TuDogApplication
{
    public static Window _window;
    public override object CreateShell()
    {
        var hook = new TaskPoolGlobalHook();
        hook.MouseClicked += (s, e) =>
        {
            var x = e.Data.X;
            var y = e.Data.Y;

            var mousePosition = _window.Screens.ScreenFromPoint(new(x, y));

            if (mousePosition != null)
            {
                // 将全局坐标转换为窗口内坐标
                var localPosition = _window.PointToClient(new(x, y));

                // 判断是否在窗口内
                if (!_window.Bounds.Contains(localPosition))
                    Dispatcher.UIThread.Invoke(() => { _window.Hide(); });
            }
        };

        hook.RunAsync();
        _window = new MainWindow();
        return _window;
    }

    public override void Initialize()
    {
        this.EnableHotReload();
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }

    protected override void Register(IServiceCollection collection)
    {
        collection.AddHttpClient();
    }


    private void Show(object? sender, EventArgs e)
    {
        // 获取主屏幕尺寸
        var screen = _window.Screens.Primary;
        if (screen is null)
            return;

        if (OperatingSystem.IsWindows())
        {
            // 计算托盘位置 (右下角)
            var x = screen.WorkingArea.Width - _window.Width - 10;
            var y = screen.WorkingArea.Height - _window.Height;
            _window.Position = new((int)x, (int)y);
            _window.Show();
            _window.Activate();
        }
        else
        {
            var x = screen.WorkingArea.Width - _window.Width - 10;
            var y = 10;
            _window.Position = new((int)x, (int)y);
            _window.Show();
            _window.Activate();
        }
    }

    private async void Exit(object? sender, EventArgs e)
    {
        var v = _window as MainWindow;
        if (v.CanExit())
           Environment.Exit(0);
        var dialog = ServiceProvider.GetService<IDialogServer>();
        await dialog.ShowMessageDialogAsync("有任务正在进行，无法退出");
    }
}