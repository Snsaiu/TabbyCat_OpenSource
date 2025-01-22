using System.Net.NetworkInformation;

using TabbyCat.Shared.Interfaces;

namespace AirTransfer.Interfaces.Impls;

public class GlobalScanBase : ISendeable<string>
{
    public async Task SendAsync(string message, CancellationToken cancellationToken)
    {
        var ipSplits = message.Remove(message.LastIndexOf('.'));

        await Task.Run(() =>
        {
            for (var i = 1; i < 255; i++)
            {
                var pingIp = ipSplits + "." + i.ToString();
                cancellationToken.ThrowIfCancellationRequested();
                using var myPing = new Ping();
                myPing.SendAsync(pingIp, 20, cancellationToken);
            }
        }, cancellationToken);
    }
}