using System.Threading.Tasks;

using TuDog.IocAttribute;

namespace YouYan.Rabbit.ViewModels;

[Register]
public sealed partial class DownloadInstallViewModel : ConfirmViewModelBase
{

    public DownloadInstallViewModel()
    {

    }

    protected override Task OnConfirm()
    {
        throw new System.NotImplementedException();
    }
}