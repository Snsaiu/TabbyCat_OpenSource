using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using Duende.IdentityModel.OidcClient;

using HotAvalonia;

using Microsoft.Extensions.DependencyInjection;

using SharpHook.Native;

using TabbyCat.Extensions;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users.Configs;
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
            this.EnableHotReload();
            AvaloniaXamlLoader.Load(this);
            base.Initialize();

            ValidateUserLoginState();
            InitBackgroundImage();
            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
            {
                var hotkeyStartProgramService = ServiceProvider.GetRequiredService<IHotKeyStartProgramService>();
                var useHotkey = hotkeyStartProgramService.Get();
                if (useHotkey)
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
                    if (window is null)
                        return;

                    if (window.IsActive)
                    {
                        window.Hide();
                    }
                    else
                    {
                        window.WindowState = WindowState.Normal;
                        window.Show();
                        window.Activate();
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
            var mannager = ServiceProvider.GetRequiredService<IRunningHubStateManager>();
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
            collection.AddHttpClient(ConstParameter.Auth).AddHttpMessageHandler<TokenHandler>();

            collection.AddLoggerBuilder("http://24.233.2.12:3100", LogLabelProvider.GetLabels());
        }
    }
}