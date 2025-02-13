using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace TabbyCat.App.Interfaces.Impls;

/// <summary>
///     设备获得本机ip
/// </summary>
public abstract class DeviceLocalIpBase : IGetLocalIp
{
    public virtual Task<string> GetLocalIpAsync()
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nic => nic.NetworkInterfaceType is NetworkInterfaceType.Wireless80211 or
                    NetworkInterfaceType.Ethernet && !nic.Description.Contains("VMware")
                                                  && !nic.Description.Contains("Virtual")
                                                  && !nic.Description.Contains("VPN")
                                                  && !nic.Description.Contains("Ethernet"));

        foreach (var nic in networkInterfaces)
        {
            var ipProps = nic.GetIPProperties();
            var ipAddress = ipProps.UnicastAddresses
                .FirstOrDefault(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork);

            if (ipAddress == null) continue;

            if (nic.OperationalStatus == OperationalStatus.Up) return Task.FromResult(ipAddress.Address.ToString());
        }

        throw new NullReferenceException("无法找到本机ip");
    }
}