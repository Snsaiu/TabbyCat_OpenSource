using SharpHook.Native;

namespace TabbyCat.App.Interfaces;

public interface IHotKeyHookService
{
    void InitService();

    Action<IEnumerable<KeyCode>> Action { get; set; }

}