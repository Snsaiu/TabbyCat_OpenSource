using TabbyCat.App.Interfaces.IConfigs;

namespace TabbyCat.App.Interfaces.Impls.Configs;

public sealed class CloseAppBehaviorService : ConfigServiceBase, ICloseAppBehaviorService
{
    public override string Key => "CloseAppBehavior";
}