using Device = TabbyCat.Shared.Enums.Device;

namespace TabbyCat.App.Interfaces;

public interface IDeviceType
{
    public Device Device { get; }
}