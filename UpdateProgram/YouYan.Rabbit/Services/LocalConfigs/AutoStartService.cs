using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;
using YouYan.Rabbit.IServices.LocalConfigs;

namespace YouYan.Rabbit.Services.LocalConfigs;

[Register<IAutoStartService>]
public sealed class AutoStartService: LocalConfigService<bool>, IAutoStartService
{
    public override string Key { get; }="autostart";
    public override bool Default { get; } = true;
}