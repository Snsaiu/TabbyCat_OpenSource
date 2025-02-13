
using System.Text;
using TabbyCat.App.Interfaces;

using TabbyCat.Shared.Interfaces;

namespace TabbyCat.App.Models;

public class SendTextModel(string flag, string targetFlag, string text, int port) : IFlag, ISize, ITargetFlag, IPort
{
    public string Flag { get; init; } = flag;

    public string Text { get; init; } = text;
    public long Size { get; } = Encoding.UTF8.GetByteCount(text);
    public string TargetFlag { get; init; } = targetFlag;
    public int Port { get; init; } = port;
}