namespace TabbyCat.Shared.Interfaces;

public interface ILoopWatchClipboardService
{
    void SetState(bool state);

    bool GetState();
}