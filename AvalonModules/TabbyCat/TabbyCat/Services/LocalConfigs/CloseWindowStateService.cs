using TabbyCat.Enums;
using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<ICloseWindowStateService>]
public sealed class CloseWindowStateService(IPreferenceService preferenceService) :LocalConfigService<WindowCloseState>(preferenceService),ICloseWindowStateService
{
    public override string Key { get; }="closeWindowState";

    public override WindowCloseState Default { get; } = WindowCloseState.Ask;
}