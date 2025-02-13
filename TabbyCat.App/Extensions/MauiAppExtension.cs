using TabbyCat.App.Components.Components;
using TabbyCat.App.Interfaces.IConfigs;

using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.Maui.LifecycleEvents;

using System.Runtime.InteropServices;

using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.ConstParameters;







#if WINDOWS
using Microsoft.UI.Windowing;
using Microsoft.UI;

using WinRT.Interop;
#endif


namespace TabbyCat.App.Extensions;

public static class MauiAppExtension
{
#if WINDOWS
    public static void HideTaskBar(Microsoft.UI.Xaml.Window window)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        var exStyle = (int)GetWindowLong(hWnd, (int)GetWindowLongFields.GWL_EXSTYLE);

        exStyle &= ~(int)ExtendedWindowStyles.WS_EX_APPWINDOW;
        exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;

        SetWindowLong(hWnd, (int)GetWindowLongFields.GWL_EXSTYLE, exStyle);
    }

    [Flags]
    public enum ExtendedWindowStyles
    {
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_APPWINDOW = 0x00040000
    }

    public enum GetWindowLongFields
    {
        // ...
        GWL_EXSTYLE = -20
        // ...
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public const int SW_HIDE = 0;
    public const int SW_SHOW = 5;

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

    public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        var error = 0;
        var result = IntPtr.Zero;
        // Win32 SetWindowLong doesn't clear error on success
        SetLastError(0);

        if (IntPtr.Size == 4)
        {
            // use SetWindowLong
            var tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
            error = Marshal.GetLastWin32Error();
            result = new(tempResult);
        }
        else
        {
            // use SetWindowLongPtr
            result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
            error = Marshal.GetLastWin32Error();
        }

        if (result == IntPtr.Zero && error != 0)
        {
            throw new System.ComponentModel.Win32Exception(error);
        }

        return result;
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    private static extern int IntSetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private static int IntPtrToInt32(IntPtr intPtr)
    {
        return unchecked((int)intPtr.ToInt64());
    }

    [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
    public static extern void SetLastError(int dwErrorCode);

#endif

#if WINDOWS


    private static void ConfigureMainWindow(Microsoft.UI.Xaml.Window del)
    {
        del.ExtendsContentIntoTitleBar = true;

        var appWindow = ConvertToAppWindow(del);
        appWindow.SetIcon(null);
        var presenter = appWindow.Presenter as OverlappedPresenter;

        var topService = ServiceProvider.RequestService<ITopMostService>();

        presenter.IsAlwaysOnTop = topService.Get<bool>();

        appWindow.Closing += async (sender, args) =>
        {
            var dialogService = ServiceProvider.RequestService<IDialogService>();
            args.Cancel = true;
            //get all of Microsoft.Maui.Controls.windows.
            var windows1 = Application.Current.Windows.ToList<Window>();
            var title = LocalizationResourceManager.Instance["TabbyCat"];
            foreach (var win in windows1)
            {
                if (win.Title == title)
                {
                    var showCloseDialogService = ServiceProvider.RequestService<IShowCloseDialogService>();
                    var closeAppBehaviorService = ServiceProvider.RequestService<ICloseAppBehaviorService>();

                    if (showCloseDialogService.Get<bool>())
                    {
                        // 不在显示

                        var closeState = (CloseAppBehavior)closeAppBehaviorService.Get<int>();

                        if (closeState == CloseAppBehavior.Exit)
                        {
                            Application.Current.CloseWindow(win);
                            Environment.Exit(0);

                            return;
                        }
                        else
                        {
                            var p = appWindow.Presenter as OverlappedPresenter;
                            p?.Minimize();
                            return;
                        }
                    }

                    var dialogResult = await dialogService.ShowDialogAsync<CloseDialog>(new()
                    {
                        ShowDismiss = false,
                        Title = LocalizationResourceManager.Instance["Warning"],
                        PrimaryAction = LocalizationResourceManager.Instance["MinimizeToTray"],
                        SecondaryAction = LocalizationResourceManager.Instance["ExitTheApplication"]
                    });
                    var result = await dialogResult.Result;
                    if (!result.Cancelled)
                    {
                        if (showCloseDialogService.Get<bool>())
                            closeAppBehaviorService.Set<int>((int)CloseAppBehavior.Minimize);
                        var p = appWindow.Presenter as OverlappedPresenter;
                        p?.Minimize();
                        await dialogResult.CloseAsync();

                    }

                    else
                    {
                        if (showCloseDialogService.Get<bool>())
                            closeAppBehaviorService.Set<int>((int)CloseAppBehavior.Exit);
                        Application.Current.CloseWindow(win);
                        Environment.Exit(0);
                    }
                }
            }
        };
    }

    public static AppWindow ConvertToAppWindow(Microsoft.UI.Xaml.Window window)
    {
        return Microsoft.UI.Windowing.AppWindow.GetFromWindowId(GetWindowId(window));
    }

    public static WindowId GetWindowId(Microsoft.UI.Xaml.Window window)
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        return id;
    }

#endif
    public static void ConfigureLifecycle(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(lifecycle =>
        {
#if WINDOWS
            lifecycle.AddWindows(windows => windows.OnWindowCreated((del) =>
            {
                // HideTaskBar(del);
                if (del.Title == LocalizationResourceManager.Instance["TabbyCat"])
                {
                    ConfigureMainWindow(del);
                }
                else if (del.Title == ConstParams.ShortChatWindowKey)
                {

                    del.ExtendsContentIntoTitleBar = false;
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(del);
                    WindowExtensions.Hwnd = hwnd;
                    var appWindow = ConvertToAppWindow(del);
                    appWindow.SetIcon(null);
                    var presenter = appWindow.Presenter as OverlappedPresenter;

                    presenter.SetBorderAndTitleBar(false, false);
                    presenter.IsAlwaysOnTop = true;

                    if (appWindow is not null)
                    {
                        var id = GetWindowId(del);
                        Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(id, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
                        if (displayArea is not null)
                        {
                            var CenteredPosition = appWindow.Position;
                            CenteredPosition.X =
 ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2) + appWindow.Size.Width;
                            CenteredPosition.Y =
 ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                            appWindow.Move(CenteredPosition);
                        }
                    }

                }


            }));
#endif

#if MACCATALYST
            // lifecycle.AddiOS(iOS => iOS.WillTerminate(application =>
            // {
            //     // 在应用即将关闭时执行最小化逻辑
            //     UIApplication.SharedApplication.KeyWindow?.ResignKeyWindow();
            // }));
            //
            //
            //    lifecycle.AddiOS(iOS=> iOS.OnActivated(window =>
            //     {
            //         MacOSWindowExtensions.EnableCloseButtonAsMinimize();
            //     }));

#endif
        });
    }
}