using System.Net.Sockets;

using TabbyCat.App.Models;

using TabbyCat.Shared.ConstParameters;

namespace TabbyCat.App.Interfaces.Impls.UdpTransfer;

/// <summary>
/// 设备发现
/// </summary>
public abstract class LocalNetDeviceDiscoveryBase(DeviceLocalIpBase localIpBase)
    : UdpLoopIListenBase<DeviceDiscoveryMessage>(localIpBase)
{
    protected override UdpClient CreateUdpClient()
    {
        return new(ConstParams.INVITE_PORT);
    }
}