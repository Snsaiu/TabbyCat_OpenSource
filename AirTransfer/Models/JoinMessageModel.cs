
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

using Device = TabbyCat.Shared.Enums.Device;

namespace AirTransfer.Models;

public class JoinMessageModel(
    SystemType systemType,
    Device device,
    string flag,
    string? deviceName,
    string sendTarget,
    string name) : ScanDevice(systemType, device, flag, deviceName), IName
{
    public string SendTarget { get; init; } = sendTarget;
    public string Name { get; init; } = name;
}