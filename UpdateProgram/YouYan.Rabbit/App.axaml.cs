using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using SharpHook;
using System;
using System.Globalization;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;
using TuDog.Interfaces.MessageBarService;
using YouYan.Rabbit.IServices.LocalConfigs;
using YouYan.Rabbit.Languages;
using YouYan.Rabbit.Views;

namespace YouYan.Rabbit;

public partial class App : TuDogApplication
{
    public static Window? _window = null;

    public override object CreateShell()
    {
        InitLanguage();
        var hook = new TaskPoolGlobalHook();
        hook.MouseClicked += (s, e) =>
        {
            var x = e.Data.X;
            var y = e.Data.Y;

            if (_window is null)
                return;
            var mousePosition = _window.Screens.ScreenFromPoint(new PixelPoint(x, y));

            if (mousePosition != null)
            {
                // 将全局坐标转换为窗口内坐标
                var localPosition = _window.PointToClient(new PixelPoint(x, y));

                // 判断是否在窗口内
                if (!_window.Bounds.Contains(localPosition))
                    Dispatcher.UIThread.Invoke(() => { _window.Hide(); });
            }
        };
#if !DEBUG
        hook.RunAsync();
#endif
        _window = new MainWindow();
        return _window;
    }

    public override void Initialize()
    {
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
            _window.Position = new PixelPoint((int)x, (int)y);
            _window.Show();
            _window.Activate();
        }
        else
        {
            var x = screen.WorkingArea.Width - _window.Width - 10;
            var y = 10;
            _window.Position = new PixelPoint((int)x, y);
            _window.Show();
            _window.Activate();
        }
    }

    private void Exit(object? sender, EventArgs e)
    {
        var v = _window as MainWindow;
        if (v.CanExit())
            Environment.Exit(0);

        var messageBarService = ServiceProvider.GetService<IMessageBarService>();
        messageBarService.ShowWarning(Language.HasTaskNoExit, Language.Warning, true);
    }

    private void InitLanguage()
    {
        var languageService = ServiceProvider.GetRequiredService<ILanguageService>();
        var language = languageService.Get();
        LocalizationResourceManager.Instance.SetCulture(new CultureInfo(language));
    }
}