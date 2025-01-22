using AirTransfer.Interfaces.IConfigs;

namespace AirTransfer.Interfaces.Impls.Configs;

public sealed class TopMostService:ConfigServiceBase, ITopMostService
{
    public override string Key => "TopMost";

}