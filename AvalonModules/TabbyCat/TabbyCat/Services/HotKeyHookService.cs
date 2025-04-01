using System.Diagnostics;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using SharpHook;
using SharpHook.Native;
using TabbyCat.IServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IHotKeyHookService>(ServiceLifetime.Singleton)]
public class HotKeyHookService:IHotKeyHookService
{
    private TaskPoolGlobalHook? hook;
    private readonly HashSet<KeyCode> keyCodes = [];
    private Timer timer;
    public void InitService()
    {
        timer = new(200);
        timer.Elapsed += (s, e) =>
        {
            if (keyCodes.Any())
            {
                Debug.WriteLine("定时器启动并清空");
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
                Debug.WriteLine("HotKey hooked:" + string.Join(",", keyCodes));
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
#if !DEBUG
        hook.RunAsync();
#endif

    }

    public required Action<IEnumerable<KeyCode>> Action { get; set; }
    public void Dispose()
    {
        hook?.Dispose();
    }
}