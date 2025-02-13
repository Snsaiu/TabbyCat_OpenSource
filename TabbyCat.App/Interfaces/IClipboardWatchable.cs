namespace TabbyCat.App.Interfaces;

/// <summary>
///监控剪切板
/// </summary>
public interface IClipboardWatchable : IDisposable
{
    Action<object>? ClipboardUpdate { get; set; }
    void Initialize(object parameter);
}