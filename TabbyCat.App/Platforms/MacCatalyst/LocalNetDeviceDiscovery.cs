using TabbyCat.App.Interfaces.Impls;
using TabbyCat.App.Interfaces.Impls.UdpTransfer;

namespace TabbyCat.App;

public sealed class LocalNetDeviceDiscovery(DeviceLocalIpBase localIpBase) : LocalNetDeviceDiscoveryBase(localIpBase)
{
}