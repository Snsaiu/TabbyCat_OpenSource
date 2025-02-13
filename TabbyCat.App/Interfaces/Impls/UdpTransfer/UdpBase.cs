using System.Net.Sockets;

namespace TabbyCat.App.Interfaces.Impls.UdpTransfer;

public abstract class UdpBase : IDisposable
{
    protected UdpClient? UdpClient;

    protected virtual UdpClient CreateUdpClient() => new();

    public void Dispose()
    {
        UdpClient?.Close();
    }
}