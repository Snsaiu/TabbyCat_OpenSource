using System.Diagnostics;
using System.Net;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using Duende.IdentityModel.OidcClient;

using HotAvalonia;

using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharpHook.Native;

using TabbyCat.Extensions;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users.Configs;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared;
using TabbyCat.ViewModels;
using TabbyCat.Views;

using TuDog.Bootstrap;
using TuDog.Interfaces.RegionManagers;

namespace TabbyCat
{
    public partial class App : TuDogApplication
    {
        private Window? window;


        public override void Initialize()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            
            AvaloniaXamlLoader.Load(this);
            base.Initialize();

            ValidateUserLoginState();
            SyncAiTemplateSettings();
            InitBackgroundImage();
            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
            {
                var hotkeyStartProgramService = ServiceProvider.GetRequiredService<IHotKeyStartProgramService>();
                var showQuickMenuService = ServiceProvider.GetRequiredService<IShowQuickMenuService>();
                var useHotkey = hotkeyStartProgramService.Get();
                var useCpShowQuickMenu = showQuickMenuService.Get();
                if (useHotkey||useCpShowQuickMenu)
                {
                    var hotkeyService = ServiceProvider.GetRequiredService<IHotKeyHookService>();
                    hotkeyService.InitService();
                    hotkeyService.Action += HotKeyImplement;
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

        private void ValidateUserLoginState()
        {
            var user = ServiceProvider.GetRequiredService<IUser>();
            var userService = ServiceProvider.GetRequiredService<ILoginUserService>();
            var u = userService.GetOrDefault();
            if (u is not null) user.ResetData(u);
        }

        private void SyncAiTemplateSettings()
        {
            var aiTemplateSettingAsyncManager = ServiceProvider.GetRequiredService<IAiTemplateSettingSyncManager>();
            aiTemplateSettingAsyncManager.StartLoopAsync();
            
        }

        private void HotKeyImplement(IEnumerable<KeyCode> code)
        {
            ShowInputDialog(code);
        }

        private void ShowInputDialog(IEnumerable<KeyCode> code)
        {
            if (code.Count() == 2 && code.First() == KeyCode.VcLeftControl && code.Last() == KeyCode.VcSpace)
            {

                Dispatcher.UIThread.Invoke(() =>
                {
                    Debug.Assert(window is not null, "windows 不应该为空");

                    if (window.IsActive)
                    {
                        window.Hide();
                        Log.Debug("隐藏窗口");
                    }
                    else
                    {
                        window.WindowState = WindowState.Normal;
                        window.Show();
                        window.Activate();
                        Log.Debug("显示窗口");
                    }
                });

            }
        }

        public override object CreateShell()
        {
            StartRunningHubWatch();
            InitLanguage();
            if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
            {
                window = new MainWindow();
                window.ShowInTaskbar = false;
#if !DEBUG
                window.Topmost = TuDogApplication.ServiceProvider.GetRequiredService<ITopMostService>().Get();
#endif
                return window;
            }
            else
            {
                var regionManager = TuDogApplication.ServiceProvider.GetRequiredService<IRegionManager>();
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
            LocalizationResourceManager.Instance.SetCulture(new(language));
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
            if (window is not { } w)
            {
                return;
            }

            w.WindowState = WindowState.Normal;
            w.Show();
            w.Activate();
        }

        protected override void Register(IServiceCollection collection)
        {
            collection.AddSingleton(typeof(OidcClient), _ => new OidcClient(OidcOptions.GetOptions()));
            collection.AddTransient<TokenHandler>();
            collection.AddHttpClient(ConstParameter.Auth,
                    client => { client.BaseAddress = new("https://api.yyan.cc/api"); })
                .AddHttpMessageHandler<TokenHandler>();

            collection.AddLoggerBuilder("http://24.233.2.12:3100", LogLabelProvider.GetLabels(),"TabbyCat");
        }
    }
}