using TabbyCat.App.Interfaces;

using Device = TabbyCat.Shared.Enums.Device;

namespace TabbyCat.App;

public sealed class DeviceTypeProvider : IDeviceType
{
    public Device Device { get; } = Device.Desktop;
}