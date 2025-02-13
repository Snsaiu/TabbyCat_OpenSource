using TabbyCat.App.Interfaces.Impls;

namespace TabbyCat.App;

public sealed class DefaultScanLocalNetIp(DeviceLocalIpBase deviceLocalIpBase) : LocalIpScannerBase(deviceLocalIpBase);