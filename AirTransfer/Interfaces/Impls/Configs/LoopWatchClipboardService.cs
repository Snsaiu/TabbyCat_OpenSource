using TabbyCat.Shared.ConstParameters;
using TabbyCat.Shared.Interfaces;

namespace AirTransfer.Interfaces.Impls.Configs;

public class LoopWatchClipboardService(IStateManager stateManager) : ILoopWatchClipboardService
{

    public void SetState(bool state)
    {
        stateManager.SetState(ConstParams.StateManagerKeys.LoopWatchClipboardKey, state);
        Preferences.Default.Set(ConstParams.StateManagerKeys.LoopWatchClipboardKey, state);

    }

    public bool GetState()
    {
        return Preferences.Default.Get(ConstParams.StateManagerKeys.LoopWatchClipboardKey, false);
    }
}