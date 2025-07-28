using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<IShowFloatingFrameService>]
public class ShowFloatingFrameService : LocalConfigService<bool>, IShowFloatingFrameService
{
    public override string Key { get; } = "showFloatingFrame";
    public override bool Default { get; } = true;
}