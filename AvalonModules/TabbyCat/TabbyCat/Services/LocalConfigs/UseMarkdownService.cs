using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<IUseMarkdownService>]
public sealed class UseMarkdownService
    : LocalConfigService<bool>, IUseMarkdownService
{
    public override string Key { get; } = "useMarkdown";
    public override bool Default { get; } = true;
}