using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<IHotKeyStartProgramService>]
public sealed class HotKeyStartProgramService : LocalConfigService<bool>, IHotKeyStartProgramService
{
    public override string Key { get; }="useHotKeyStartProgram";
    public override bool Default { get; } = false;
}