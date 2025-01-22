using Device = TabbyCat.Shared.Enums.Device;

namespace AirTransfer.Interfaces;

public interface IDeviceType
{
    public Device Device { get; }
}