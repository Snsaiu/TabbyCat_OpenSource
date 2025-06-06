using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;
using YouYan.Rabbit.IServices.LocalConfigs;

namespace YouYan.Rabbit.Services.LocalConfigs;

[Register<ILanguageService>]
public sealed class ConfigLanguageService
    : LocalConfigService<string>, ILanguageService
{
    public override string Key { get; } = "language";

    public override string Default { get; } = "en-US";
}