using System.Diagnostics;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpHook;
using SharpHook.Native;
using TabbyCat.IServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IHotKeyHookService>(ServiceLifetime.Singleton)]
public class HotKeyHookService(ILogger<HotKeyHookService> logger) : IHotKeyHookService
{
    private TaskPoolGlobalHook? hook;
    private readonly HashSet<KeyCode> keyCodes = [];
    private readonly Timer timer =new (200);
    public void InitService()
    {
        timer.Elapsed += (s, e) =>
        {
            if (keyCodes.Any())
            {
                logger.LogDebug("定时器启动并清空");
                keyCodes.Clear();
            }

            timer.Stop();
        };
        hook = new(1, GlobalHookType.Keyboard, runAsyncOnBackgroundThread: true);
        hook.KeyPressed += (sender, e) =>
        {
            keyCodes.Add(e.Data.KeyCode);
            if (keyCodes.Count > 1)
            {
                logger.LogDebug("HotKey hooked:{0}", string.Join(",", keyCodes));
                Action?.Invoke(new List<KeyCode>(keyCodes));
            }
        };
        hook.KeyReleased += (sender, e) =>
        {
            // if (keyCodes.Count >= 2)
            //     keyCodes.Clear();
            if (!timer.Enabled)
                timer.Start();
        };
        hook.RunAsync();

    }

    public required Action<IEnumerable<KeyCode>> Action { get; set; }
    public void Dispose()
    {
        hook?.Dispose();
    }
}