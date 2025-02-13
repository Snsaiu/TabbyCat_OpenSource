using System.Net;
using System.Net.Sockets;

using TabbyCat.App.Models;

using TabbyCat.Shared.ConstParameters;

namespace TabbyCat.App.Interfaces.Impls.UdpTransfer;

/// <summary>
/// 局域网设备邀请
/// </summary>
public abstract class LocalNetInviteDeviceBase : UdpSendBase<DeviceDiscoveryMessage>
{
    protected override UdpClient CreateUdpClient()
    {
        return new UdpClient() { EnableBroadcast = true };
    }

    protected override IPEndPoint SetTarget(DeviceDiscoveryMessage invite)
    {
        return new IPEndPoint(IPAddress.Parse(invite.TargetFlag), ConstParams.INVITE_PORT);
    }
}