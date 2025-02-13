using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TabbyCat.Shared.Enums;
using TabbyCat.App.Models;
using Device = TabbyCat.Shared.Enums.Device;

namespace TabbyCat.App.Interfaces.Impls;

public class LocalIpScannerBase(DeviceLocalIpBase deviceLocalIpBase) : IGetLocalNetDevices
{
    protected readonly DeviceLocalIpBase DeviceLocalIpBase = deviceLocalIpBase;
    protected virtual string Pattern { get; } = @"(?<ip>([0-9]{1,3}\.?){4})\s*\) at ([0-9]|[a-z])";

    public virtual async IAsyncEnumerable<ScanDevice> GetDevicesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var localIp = await DeviceLocalIpBase.GetLocalIpAsync();

        var list = localIp.Split(".").ToList()[0..2];
        var ipStart = string.Join(".", list);

        var scanIps = await Task.Run(() =>
        {
            List<string> temp = [];
            var pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a ";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            var cmdOutput = pProcess.StandardOutput.ReadToEnd();

            foreach (Match m in Regex.Matches(cmdOutput, Pattern, RegexOptions.IgnoreCase))
            {
                temp.Add(m.Groups["ip"].Value);
            }

            return temp.AsReadOnly();
        }, cancellationToken);

        if (!scanIps.Any())
            yield break;
        foreach (var item in scanIps)
        {
            if (item.StartsWith(ipStart))
                yield return new ScanDevice(SystemType.None, Device.None, item, null);
        }
    }


    private static IPAddress GetLowestIp(IPAddress address, IPAddress mask)
    {
        var addressBytes = address.GetAddressBytes();
        var maskBytes = mask.GetAddressBytes();
        if (addressBytes.Length != 4 || maskBytes.Length != 4)
            return IPAddress.None;
        var lowest = new byte[4];
        for (var i = 0; i < 4; i++)
            lowest[i] = (byte)(addressBytes[i] & maskBytes[i]);
        return new IPAddress(lowest);
    }

    private static IPAddress GetHighestIp(IPAddress address, IPAddress mask)
    {
        var addressBytes = address.GetAddressBytes();
        var maskBytes = mask.GetAddressBytes();
        if (addressBytes.Length != 4 || maskBytes.Length != 4)
            return IPAddress.None;
        var highest = new byte[4];
        for (var i = 0; i < 4; i++)
            highest[i] = (byte)((addressBytes[i] & maskBytes[i]) | ~maskBytes[i]);
        return new IPAddress(highest);
    }
}