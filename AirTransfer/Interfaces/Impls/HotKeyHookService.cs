using SharpHook;
using SharpHook.Native;

using System.Diagnostics;

namespace AirTransfer.Interfaces.Impls;

public sealed class HotKeyHookService : IHotKeyHookService, IDisposable
{
    private TaskPoolGlobalHook? hook;
    private readonly HashSet<KeyCode> keyCodes = [];
    public void InitService()
    {
        hook = new(1, GlobalHookType.Keyboard, runAsyncOnBackgroundThread: true);
        hook.KeyPressed += (sender, e) =>
        {
            keyCodes.Add(e.Data.KeyCode);
            if (keyCodes.Count > 1)
            {
                Debug.WriteLine("HotKey hooked:" + string.Join(",", keyCodes));
                Action?.Invoke(keyCodes);
            }
        };
        hook.KeyReleased += (sender, e) =>
        {
            keyCodes.Clear();
        };
#if WINDOWS
        hook.RunAsync();

#endif
    }

    public required Action<IEnumerable<KeyCode>> Action { get; set; }
    public void Dispose()
    {
        hook?.Dispose();
    }
}