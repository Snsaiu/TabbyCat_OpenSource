using TabbyCat.App.Interfaces.Impls;
using TabbyCat.App.Interfaces.Impls.UdpTransfer;
namespace TabbyCat.App;

public sealed class LocalNetJoinProcess : LocalNetJoinProcessBase
{
    public LocalNetJoinProcess(DeviceLocalIpBase localIpBase) : base(localIpBase)
    {
    }
}