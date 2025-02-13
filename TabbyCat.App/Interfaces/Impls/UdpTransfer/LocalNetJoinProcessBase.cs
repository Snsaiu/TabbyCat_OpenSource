using System.Net.Sockets;

using TabbyCat.App.Models;

using TabbyCat.Shared.ConstParameters;

namespace TabbyCat.App.Interfaces.Impls.UdpTransfer;

public class LocalNetJoinProcessBase(DeviceLocalIpBase localIpBase) : UdpLoopIListenBase<JoinMessageModel>(localIpBase)
{
    protected override UdpClient CreateUdpClient()
    {
        return new(ConstParams.JOIN_PORT);
    }
}