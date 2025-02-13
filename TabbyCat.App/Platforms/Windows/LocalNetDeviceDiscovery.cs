using TabbyCat.App.Interfaces.Impls;
using TabbyCat.App.Interfaces.Impls.UdpTransfer;
namespace TabbyCat.App;

public sealed class LocalNetDeviceDiscovery : LocalNetDeviceDiscoveryBase
{
    public LocalNetDeviceDiscovery(DeviceLocalIpBase localIpBase) : base(localIpBase)
    {
    }
}