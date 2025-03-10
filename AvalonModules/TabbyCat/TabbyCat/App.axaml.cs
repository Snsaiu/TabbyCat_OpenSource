using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using Microsoft.Extensions.DependencyInjection;

using SharpHook.Native;

using System.Diagnostics;

using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
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
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
            
            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
            {
                var hotkeyStartProgramService = ServiceProvider.GetService<IHotKeyStartProgramService>();
                var useHotkey = hotkeyStartProgramService.Get();
                if (useHotkey)
                {
                    var hotkeyService = ServiceProvider.GetService<IHotKeyHookService>();
                    hotkeyService.InitService();
                    hotkeyService.Action += HotKeyImplement;
                }
            }
        }

        private void HotKeyImplement(IEnumerable<KeyCode> code)
        {
            ShowInputDialog(code);
        }

        private void ShowInputDialog(IEnumerable<KeyCode> code)
        {
            if (code.Count() == 2 && code.First() == KeyCode.VcLeftControl && code.Last() == KeyCode.VcSpace)
            {
                Debug.WriteLine("open or hide window");
                Dispatcher.UIThread.Invoke(() =>
                {
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
                window.Topmost = TuDogApplication.ServiceProvider.GetService<ITopMostService>().Get();

                return window;
            }
            else
            {
                var regionManager = TuDogApplication.ServiceProvider.GetService<IRegionManager>();
                var view = regionManager!.GetViewByViewModel<MainViewModel>();
                return view;
            }
        }


        private void StartRunningHubWatch()
        {
            var mannager = ServiceProvider.GetService<IRunningHubStateManager>();
            mannager.StartWatchAsync();
        }

        private void InitLanguage()
        {
            var languageService = ServiceProvider.GetService<ILanguageService>();
            var language = languageService.Get();
            LocalizationResourceManager.Instance.SetCulture(new(language));
        }


        private void Exit(object? sender, EventArgs e)
        {
            if (OperatingSystem.IsWindows())
                Environment.Exit(0);
            else
                window.Close();
        }

        private void Show(object? sender, EventArgs e)
        {
            this.window.WindowState = WindowState.Normal;
            window.Show();
            window.Activate();
        }
    }
}