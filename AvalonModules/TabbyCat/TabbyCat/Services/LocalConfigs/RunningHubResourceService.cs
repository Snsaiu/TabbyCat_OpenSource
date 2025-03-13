using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<IRunningHubResourceService>]
public sealed class RunningHubResourceService
    : LocalConfigService<string>, IRunningHubResourceService
{
    public override string Default { get; } = System.IO.Path.GetTempPath();
    public override string Key { get; } = "runningHub";
}