using AirTransfer.Interfaces;

using Device = TabbyCat.Shared.Enums.Device;

namespace AirTransfer;

public sealed class DeviceTypeProvider : IDeviceType
{
    public Device Device { get; } = Device.Desktop;
    Device IDeviceType.Device { get; }
}