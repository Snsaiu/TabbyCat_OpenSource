using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<ITopMostService>]
public sealed class TopMostService : LocalConfigService<bool>, ITopMostService
{
    public override string Key { get; } = "isTopMost";
    public override bool Default { get; } = false;
}