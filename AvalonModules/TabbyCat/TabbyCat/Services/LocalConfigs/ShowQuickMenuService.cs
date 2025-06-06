using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<IShowQuickMenuService>]
public sealed class ShowQuickMenuService:LocalConfigService<bool>, IShowQuickMenuService
{
    public override string Key { get; }="showQuickMenu";
    public override bool Default { get; } = false;
}