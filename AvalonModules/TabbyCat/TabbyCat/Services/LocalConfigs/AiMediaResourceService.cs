using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<IAiMediaResourceService>]
public sealed class AiMediaResourceService
    : LocalConfigService<string>, IAiMediaResourceService
{
    public override string Default { get; } = System.IO.Path.GetTempPath();
    public override string Key { get; } = "aiMediaConfig";
}