using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<IUseMarkdownService>]
public sealed class UseMarkdownService(IPreferenceService preferenceService)
    : LocalConfigService<bool>(preferenceService), IUseMarkdownService
{
    public override string Key { get; } = "useMarkdown";
    public override bool Default { get; } = false;
}