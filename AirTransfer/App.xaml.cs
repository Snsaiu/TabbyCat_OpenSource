using AirTransfer.Components.Pages;
using AirTransfer.Extensions;
using AirTransfer.Interfaces;

#if WINDOWS
using Microsoft.UI.Windowing;
#endif
using SharpHook.Native;

using System.Diagnostics;
using System.Globalization;

using TabbyCat.Shared;
using TabbyCat.Shared.ConstParameters;

namespace AirTransfer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
            {
                Debug.WriteLine(args.Exception.Message);
            };
        }


        protected override Window CreateWindow(IActivationState? activationState)
        {
            InitLanguage();

            var title = LocalizationResourceManager.Instance["TabbyCat"];
#if WINDOWS || MACCATALYST

            InitHotKey();

            var window = new Window(new MainPage()) { Title = title, Width = 600, MinimumWidth = 600 };
            return window;
#endif
            return new Window(new MainPage()) { Title = title };
        }

        protected override void OnStart()
        {
            base.OnStart();
        }


        private void InitHotKey()
        {
            var hotKeyService = Handler.MauiContext.Services.GetRequiredService<IHotKeyHookService>();
            hotKeyService.InitService();
            hotKeyService.Action = keys =>
            {
                ShowChatWindows(keys);
            };
        }

        private void ShowChatWindows(IEnumerable<KeyCode> keyCodes)
        {
            if (keyCodes.Count() != 2)
            {
                return;
            }

            if (keyCodes.First() != KeyCode.VcLeftControl || keyCodes.Last() != KeyCode.VcSpace)
            {
                return;
            }

            Current?.Dispatcher.Dispatch(() =>
            {
                var windows = Current.Windows.ToList<Window>();
                if (windows.FirstOrDefault(x => x.Title == ConstParams.ShortChatWindowKey) is { } find)
                {
#if WINDOWS
                    Microsoft.UI.Xaml.Window w = (Microsoft.UI.Xaml.Window)find.Handler.PlatformView;
                    var appwindow = MauiAppExtension.ConvertToAppWindow(w);
                    var p = appwindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
                    if (p.State == OverlappedPresenterState.Minimized)
                    {
                        p.Restore();
                    }
                    else
                    {
                        p.Minimize(false);
                        p.SetBorderAndTitleBar(false, false);
                        if (windows.FirstOrDefault(y => y.Title == LocalizationResourceManager.Instance["TabbyCat"]) is { } m)
                        {
                            var mainW = ((Microsoft.UI.Xaml.Window)m.Handler.PlatformView);
                            var id = MauiAppExtension.GetWindowId(mainW);
                            WindowExtensions.Hwnd = (nint)id.Value;
                        }
                    }
#endif
                    return;
                }

                var window = new Window
                {
                    Page = new ShortChatPage(), Width = 400, Title = ConstParams.ShortChatWindowKey, Height = 400
                };
                Current?.OpenWindow(window);
            });
        }

        private void InitLanguage()
        {
            var languageService = Handler.MauiContext.Services.GetRequiredService<ILanguageService>();
            var language = languageService.GetLanguage();
            LocalizationResourceManager.Instance.SetCulture(new CultureInfo(language));
        }
    }
}