using System.Runtime.Serialization;

using TabbyCat.App.Interfaces.Impls;

using Android.Content;
using Android.Net.Wifi;

using Application = Android.App.Application;
using Formatter = Android.Text.Format.Formatter;

namespace TabbyCat.App;

public sealed class DefaultLocalIp : DeviceLocalIpBase
{
    public override Task<string> GetLocalIpAsync()
    {
        var context = Application.Context;
        if (context.GetSystemService(Context.WifiService) is not WifiManager wm)
            throw new NullReferenceException("获得wifi管理器失败");
        if (wm.ConnectionInfo is null) throw new NullReferenceException("获得wifi信息失败");
        var ip = Formatter.FormatIpAddress(wm.ConnectionInfo.IpAddress);
        return ip is null ? throw new NullReferenceException("无法获得ip地址") : Task.FromResult(ip);
    }
}