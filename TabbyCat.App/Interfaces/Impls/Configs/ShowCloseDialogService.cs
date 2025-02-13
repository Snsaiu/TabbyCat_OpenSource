using TabbyCat.App.Interfaces.IConfigs;

namespace TabbyCat.App.Interfaces.Impls.Configs;

public sealed class ShowCloseDialogService : ConfigServiceBase, IShowCloseDialogService
{
    public override string Key => "ShowCloseDialog";
}