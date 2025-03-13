using TabbyCat.Enums;
using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<ICloseWindowStateService>]
public sealed class CloseWindowStateService : LocalConfigService<WindowCloseState>, ICloseWindowStateService
{
    public override string Key { get; }="closeWindowState";

    public override WindowCloseState Default { get; } = WindowCloseState.Ask;
}