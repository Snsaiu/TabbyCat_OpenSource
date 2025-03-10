using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<IHotKeyStartProgramService>]
public class HotKeyStartProgramService(IPreferenceService preferenceService) :LocalConfigService<bool>(preferenceService),IHotKeyStartProgramService
{
    public override string Key { get; }="useHotKeyStartProgram";
    public override bool Default { get; } = false;
}