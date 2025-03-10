using TabbyCat.Shared.Enums;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class ImageToImageViewModel : AiMediaViewModelBase
{
    protected override RunningHubWorkType RunningHubWorkType { get; }
    protected override long WorkFlowId { get; }

    protected override Task<bool> OnConfirmAsync()
    {
        throw new NotImplementedException();
    }
}