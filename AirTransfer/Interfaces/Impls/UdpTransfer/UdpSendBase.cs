using System.Net;
using AirTransfer.Extensions;
using Newtonsoft.Json;

using TabbyCat.Shared.Extensions;
using TabbyCat.Shared.Interfaces;

namespace AirTransfer.Interfaces.Impls.UdpTransfer;

public abstract class UdpSendBase<T> : UdpBase, ISendeable<T>
{
    protected abstract IPEndPoint SetTarget(T message);

    public Task SendAsync(T message, CancellationToken cancellationToken)
    {
        UdpClient ??= CreateUdpClient();
        var json = JsonConvert.SerializeObject(message);
        var data = json.ToBytes();

        return UdpClient.SendAsync(data, data.Length, SetTarget(message));
    }
}