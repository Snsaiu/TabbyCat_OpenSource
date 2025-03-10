using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<ITopMostService>]
public sealed class TopMostService(IPreferenceService preferenceService) :LocalConfigService<bool>(preferenceService),ITopMostService
{
    public override string Key { get; } = "isTopMost";
}