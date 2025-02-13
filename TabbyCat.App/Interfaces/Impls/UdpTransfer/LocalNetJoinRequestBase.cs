using System.Net;

using TabbyCat.App.Models;

using TabbyCat.Shared.ConstParameters;

namespace TabbyCat.App.Interfaces.Impls.UdpTransfer;

public abstract class LocalNetJoinRequestBase : UdpSendBase<JoinMessageModel>
{
    protected override IPEndPoint SetTarget(JoinMessageModel message)
    {
        return new(IPAddress.Parse(message.SendTarget), ConstParams.JOIN_PORT);
    }
}