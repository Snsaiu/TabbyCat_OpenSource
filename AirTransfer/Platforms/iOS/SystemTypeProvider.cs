using AirTransfer.Interfaces;

using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;


namespace AirTransfer;

public sealed class SystemTypeProvider : ISystemType
{
    public SystemType System { get; } = SystemType.IOS;
}