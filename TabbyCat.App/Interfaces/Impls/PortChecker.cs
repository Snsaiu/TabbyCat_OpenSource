using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace TabbyCat.App.Interfaces.Impls;

/// <inheritdoc />
public class PortChecker : IPortCheckable
{
    public Task<bool> IsPortInUse(int port)
    {
#if WINDOWS || MACCATALYST
          var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        var tcpConnections = ipGlobalProperties.GetActiveTcpConnections();

        return Task.FromResult(tcpConnections.Any(c => c.LocalEndPoint.Port == port));
#else
        try
        {
            // 创建一个 TcpListener 尝试绑定到指定端口
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.Stop();
            return Task.FromResult(false); // 没有异常，说明端口未被占用
        }
        catch (SocketException exception)
        {
            return Task.FromResult(true); // 捕获 SocketException，说明端口被占用
        }

#endif

    }
}