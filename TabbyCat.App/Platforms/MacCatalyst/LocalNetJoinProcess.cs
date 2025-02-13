using TabbyCat.App.Interfaces.Impls;
using TabbyCat.App.Interfaces.Impls.UdpTransfer;


namespace TabbyCat.App;

public sealed class LocalNetJoinProcess(DeviceLocalIpBase localIpBase) : LocalNetJoinProcessBase(localIpBase)
{
}