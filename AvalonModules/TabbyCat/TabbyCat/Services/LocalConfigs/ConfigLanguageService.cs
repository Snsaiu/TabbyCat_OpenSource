using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<ILanguageService>]
public sealed class ConfigLanguageService(IPreferenceService preferenceService)
    : LocalConfigService<string>(preferenceService), ILanguageService
{
    public override string Key { get; } = "language";

    public override string Default { get; } = "en-US";
}