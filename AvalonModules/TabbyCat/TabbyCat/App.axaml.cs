using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Duende.IdentityModel.OidcClient;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Extensions;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Shared;
using TabbyCat.ViewModels;
using TabbyCat.Views;
using TuDog.Bootstrap;
using TuDog.Interfaces.RegionManagers;

namespace TabbyCat;

public partial class App : TuDogApplication
{
    private Window? window;

    public override void Initialize()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        AvaloniaXamlLoader.Load(this);
        base.Initialize();
        InitLanguage();
        InitHub();
        InitBackgroundImage();
        if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
        {
            var showQuickMenuService = ServiceProvider.GetRequiredService<IShowQuickMenuService>();
            var useCpShowQuickMenu = showQuickMenuService.Get();
            if (useCpShowQuickMenu)
            {
                var hotkeyService = ServiceProvider.GetRequiredService<IHotKeyHookService>();
                hotkeyService.InitService();
            }
        }
    }

    private void InitBackgroundImage()
    {
        var backgroundImageConfigService = ServiceProvider.GetRequiredService<IBackgroundImageConfigService>();
        var backgroundImageConfig = ServiceProvider.GetRequiredService<IBackgroundImageConfig>();
        var temp = backgroundImageConfigService.Get();
        backgroundImageConfig.CustomImage = temp.CustomImage;
        backgroundImageConfig.Status = temp.Status;
        backgroundImageConfig.Opacity = temp.Opacity;
    }

  

    public override object CreateShell()
    {
        StartRunningHubWatch();

        if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
        {
            window = new MainWindow();
            window.ShowInTaskbar = false;
#if !DEBUG
            window.Topmost = ServiceProvider.GetRequiredService<ITopMostService>().Get();
#endif
            return window;
        }
        else
        {
            var regionManager = ServiceProvider.GetRequiredService<IRegionManager>();
            var view = regionManager.GetViewByViewModel<MobileStartViewModel>();
            return view;
        }
    }


    private void StartRunningHubWatch()
    {
        var mannager = ServiceProvider.GetRequiredService<IAiMediaRunningStateManager>();
        mannager.StartWatchAsync();
    }

    private void InitLanguage()
    {
        var languageService = ServiceProvider.GetRequiredService<ILanguageService>();
        var language = languageService.Get();
        LocalizationResourceManager.Instance.SetCulture(new CultureInfo(language));
    }


    private void Exit(object? sender, EventArgs e)
    {
        if (OperatingSystem.IsWindows())
            Environment.Exit(0);
        else
            window?.Close();
    }

    private void Show(object? sender, EventArgs e)
    {
        if (window is not { } w) return;

        w.WindowState = WindowState.Normal;
        w.Show();
        w.Activate();
    }

    private void InitHub()
    {
    }

    protected override void Register(IServiceCollection collection)
    {
        collection.AddHttpClient();

        var exeName = Path.GetFileName(Environment.ProcessPath)?.Split(".").FirstOrDefault();
        if (string.IsNullOrEmpty(exeName))
            throw new NullReferenceException("无法获得程序文件名称");
        var folder = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), exeName);

        collection.AddLoggerBuilder(string.Empty, null, folder);
    }
}