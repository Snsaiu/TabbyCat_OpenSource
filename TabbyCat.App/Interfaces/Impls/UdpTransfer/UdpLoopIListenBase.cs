using System.Text;

using Newtonsoft.Json;

using TabbyCat.Shared.Interfaces;

namespace TabbyCat.App.Interfaces.Impls.UdpTransfer;

public abstract class UdpLoopIListenBase<T> : UdpBase, IListenable<T> where T : IFlag, IName
{
    private readonly DeviceLocalIpBase localIpBase;

    public UdpLoopIListenBase(DeviceLocalIpBase localIpBase)
    {
        this.localIpBase = localIpBase;
    }

    protected virtual Task OnCancelReceiveAsync()
    {
        return Task.CompletedTask;
    }

    public async Task ReceiveAsync(Action<T> receiveCallBack, CancellationToken cancellationToken)
    {
        var localIp = await localIpBase.GetLocalIpAsync();
        UdpClient ??= CreateUdpClient();

        while (true)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var receivedData = await UdpClient.ReceiveAsync(cancellationToken);
                var message = Encoding.UTF8.GetString(receivedData.Buffer);
                var model = JsonConvert.DeserializeObject<T>(message);
                if (model is null)
                    continue;
                if (model.Flag == localIp)
                    continue;
                receiveCallBack?.Invoke(model);
            }
            catch (OperationCanceledException)
            {
                await OnCancelReceiveAsync();
            }
        }
    }
}