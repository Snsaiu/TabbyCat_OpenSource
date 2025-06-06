using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaEdit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpHook;
using SharpHook.Native;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;
using TuDog.Interfaces.MessageBarService;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IHotKeyHookService>(ServiceLifetime.Singleton)]
public class HotKeyHookService(
    ILogger<HotKeyHookService> logger,
    IShowQuickMenuItemWindowService showQuickMenuItemWindowService,
    IShowQuickMenuService showQuickMenuService) : IHotKeyHookService
{
    private TaskPoolGlobalHook? hook;
    private readonly HashSet<KeyCode> keyCodes = [];
    private readonly Timer timer = new(200);
    private Point? _lastMousePosition;

    public void InitService()
    {
        var showQuickMenu = showQuickMenuService.Get();

        timer.Elapsed += (s, e) =>
        {
            if (keyCodes.Any())
                // logger.LogDebug("定时器启动并清空");
                keyCodes.Clear();

            timer.Stop();
        };
        hook = new TaskPoolGlobalHook(1, GlobalHookType.All, runAsyncOnBackgroundThread: true);
        hook.KeyPressed += (sender, e) =>
        {
            keyCodes.Add(e.Data.KeyCode);
            if (keyCodes.Count > 1)
            {
                if (showQuickMenu)
                {
                    if (OperatingSystem.IsMacOS())
                    {
                        if (keyCodes.First() == KeyCode.VcLeftMeta && keyCodes.Last() == KeyCode.VcC)
                            Task.Run(async () =>
                            {
                                var clipboard = TuDogApplication.TopLevel.Clipboard;
                                await Task.Delay(500);
                                var text = await clipboard.GetTextAsync();
                                if (string.IsNullOrEmpty(text))
                                    return;
                                if (!string.IsNullOrEmpty(text) && _lastMousePosition.HasValue)
                                    Dispatcher.UIThread.Invoke(() =>
                                    {
                                        showQuickMenuItemWindowService.ShowWindowAsync(text,
                                            _lastMousePosition.Value);
                                    });

                                keyCodes.Clear();
                            });
                    }
                    else if (OperatingSystem.IsWindows())
                    {
                        if (keyCodes.First() == KeyCode.VcLeftControl && keyCodes.Last() == KeyCode.VcC)
                            Task.Run(async () =>
                            {
                                var clipboard = TuDogApplication.TopLevel.Clipboard;
                                await Task.Delay(500);
                                var text = await clipboard.GetTextAsync();
                                if (string.IsNullOrEmpty(text))
                                    return;
                                if (!string.IsNullOrEmpty(text) && _lastMousePosition.HasValue)
                                    Dispatcher.UIThread.Invoke(() =>
                                    {
                                        showQuickMenuItemWindowService.ShowWindowAsync(text,
                                            _lastMousePosition.Value);
                                    });

                                keyCodes.Clear();
                            });
                    }
                }

                Action?.Invoke(new List<KeyCode>(keyCodes));
            }
            else
            {
                if (showQuickMenu)
                    showQuickMenuItemWindowService.HideWindowAsync();
            }
        };
        hook.MouseReleased += (sender, e) =>
        {
            if (showQuickMenu)
            {
                _lastMousePosition = new Point(e.Data.X, e.Data.Y);

                Dispatcher.UIThread.Invoke(() =>
                {
                    if (OperatingSystem.IsMacOS())
                    {
                        var ptr = IntPtr.Zero;
                        try
                        {
                            ptr = get_selected_text();
                            if (ptr == IntPtr.Zero)
                            {
                                showQuickMenuItemWindowService.HideWindowAsync(new Point(e.Data.X, e.Data.Y));
                                return;
                            }

                            var str = Marshal.PtrToStringAnsi(ptr);
                            if (string.IsNullOrEmpty(str))
                            {
                                showQuickMenuItemWindowService.HideWindowAsync(new Point(e.Data.X, e.Data.Y));
                                return;
                            }

                            logger.LogDebug("选中的文本:{0}", str);
                            showQuickMenuItemWindowService.ShowWindowAsync(str, new Point(e.Data.X, e.Data.Y));
                        }
                        catch (Exception exception)
                        {
                            logger.LogError(exception.Message, exception);
                        }
                        finally
                        {
                            if (ptr != IntPtr.Zero)
                                free(ptr);
                        }
                    }
                    else if (OperatingSystem.IsWindows())
                    {
                        showQuickMenuItemWindowService.HideWindowAsync(new Point(e.Data.X, e.Data.Y));
                    }
                });
            }
        };
        hook.KeyReleased += (sender, e) =>
        {
            // if (keyCodes.Count >= 2)
            //     keyCodes.Clear();
            if (!timer.Enabled)
                timer.Start();
        };
#if !DEBUG
        hook.RunAsync();
#endif
    }

    public required Action<IEnumerable<KeyCode>> Action { get; set; }

    public void Dispose()
    {
        hook?.Dispose();
    }

    [DllImport("libTabbyCatBridge.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_selected_text();

    [DllImport("libSystem.B.dylib", EntryPoint = "free", CallingConvention = CallingConvention.Cdecl)]
    public static extern void free(IntPtr ptr);
}