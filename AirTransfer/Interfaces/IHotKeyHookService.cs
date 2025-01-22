using SharpHook.Native;

namespace AirTransfer.Interfaces;

public interface IHotKeyHookService
{
    void InitService();

    Action<IEnumerable<KeyCode>> Action { get; set; }

}