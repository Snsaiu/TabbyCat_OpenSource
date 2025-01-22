using System.Diagnostics.CodeAnalysis;
using System.Timers;

namespace AirTransfer.Interfaces.Impls;

public abstract class LoopClipboardWatcherBase : IClipboardWatchable
{
    [NotNull]
    private System.Timers.Timer _timer = null!;
    private string? _lastText;

    public void Initialize(object parameter)
    {
        _timer = new(1000);
        _timer.Elapsed += CheckClipboard;
        _timer.AutoReset = true;
        //    _timer.Start();
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }

    private void CheckClipboard(object? sender, ElapsedEventArgs e)
    {
        return;
        Application.Current?.Dispatcher.Dispatch(async () =>
        {
            var content = await Clipboard.Default.GetTextAsync();
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            if (content == _lastText) return;

            _lastText = content;
            ClipboardUpdate?.Invoke(content);
        });
    }

    public Action<object>? ClipboardUpdate { get; set; }
}