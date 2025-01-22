using AirTransfer.Interfaces;

using TabbyCat.Shared.Interfaces;


namespace AirTransfer.Models;

public abstract class SendModelBase(string flag, string targetFlag, string fileFullPath, int port)
    : IFlag, ISize, ITargetFlag, IPort
{
    public string FileFullPath { get; protected set; } = fileFullPath;
    public string Flag { get; } = flag;
    public abstract long Size { get; }
    public string TargetFlag { get; } = targetFlag;
    public int Port { get; } = port;
}