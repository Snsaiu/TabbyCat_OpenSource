using FantasyResultModel;
using FantasyResultModel.Impls;

using System.Net;
using System.Net.Sockets;

namespace FantasyRemoteCopy.Core.Platforms
{
    public class DefaultLocalIp : IGetLocalIp
    {
        public DefaultLocalIp()
        {
        }

        public ResultBase<List<string>> GetLocalIp()
        {
            try
            {
                List<string> ips = [];
                IPHostEntry host;
                string? localIP = null;
                host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (IPAddress ip in host.AddressList)
                {
                    localIP = ip.ToString();

                    string[] temp = localIP.Split('.');

                    if (ip.AddressFamily == AddressFamily.InterNetwork && temp[0] == "192")
                    {
                        ips.Add(localIP);
                    }
                    else
                    {
                        localIP = null;
                    }
                }

                return ips.Count == 0 ? new ErrorResultModel<List<string>>("无法获得本机ip地址") : new SuccessResultModel<List<string>>(ips);
            }
            catch (Exception e)
            {
                return new ErrorResultModel<List<string>>(e.Message);
            }

        }
    }
}

