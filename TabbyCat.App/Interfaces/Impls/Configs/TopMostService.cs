using TabbyCat.App.Interfaces.IConfigs;

namespace TabbyCat.App.Interfaces.Impls.Configs;

public sealed class TopMostService:ConfigServiceBase, ITopMostService
{
    public override string Key => "TopMost";

}