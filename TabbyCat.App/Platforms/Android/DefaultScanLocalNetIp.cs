using TabbyCat.App.Interfaces.Impls;
using TabbyCat.App.Models;

using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace TabbyCat.App
{
    public sealed class DefaultScanLocalNetIp(DeviceLocalIpBase deviceLocalIpBase)
        : LocalIpScannerBase(deviceLocalIpBase)
    {
        public override async IAsyncEnumerable<ScanDevice> GetDevicesAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var localIp = await DeviceLocalIpBase.GetLocalIpAsync();
            var localSplits = localIp.Split(".")[0..3];
            var baseIP = string.Join(".", localSplits);

            // 创建4个线程任务，每个线程负责部分 IP
            var numberOfThreads = 4;
            var tasks = new List<Task<List<ScanDevice>>>();

            for (var t = 0; t < numberOfThreads; t++)
            {
                var start = t * (255 / numberOfThreads) + 1;
                var end = t == numberOfThreads - 1 ? 255 : start + 255 / numberOfThreads - 1;

                tasks.Add(Task.Run(async () =>
                {
                    var devices = new List<ScanDevice>();
                    for (var i = start; i <= end; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        var ip = $"{baseIP}.{i}";
                        try
                        {
                            var ping = new Ping();
                            var reply = await ping.SendPingAsync(ip, 100);
                            if (reply.Status == IPStatus.Success) devices.Add(new(ip));
                        }
                        catch (Exception)
                        {

                        }

                    }

                    return devices;
                }, cancellationToken));
            }

            // 等待所有任务完成并返回结果
            var results = await Task.WhenAll(tasks);

            foreach (var result in results.SelectMany(r => r)) yield return result;
        }
    }
}