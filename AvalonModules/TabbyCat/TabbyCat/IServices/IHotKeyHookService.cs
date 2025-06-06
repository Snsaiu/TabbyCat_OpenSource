using SharpHook.Native;

namespace TabbyCat.IServices;

public interface IHotKeyHookService
{
    void InitService();

    Action<IEnumerable<KeyCode>> Action { get; set; }
}