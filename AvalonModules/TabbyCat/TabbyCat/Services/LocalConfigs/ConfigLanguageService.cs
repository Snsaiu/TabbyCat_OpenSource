using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<ILanguageService>]
public sealed class ConfigLanguageService
    : LocalConfigService<string>, ILanguageService
{
    public override string Key { get; } = "language";

    public override string Default { get; } = "en-US";
}