using System.Net;
using System.Net.Sockets;

namespace TabbyCat.App.Extensions;

public static class SocketExtensions
{
    /// <summary>
    /// 检查端口是否被占用
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="port">需要检查的端口号</param>
    /// <returns>true表示端口占用，否则端口没有被占用</returns>
    public static bool TcpPortOpen(this Socket socket, int port)
    {
        try
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.Stop();
            return false;
        }
        catch (SocketException)
        {
            return true;
        }
    }
}