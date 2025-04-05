using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Models.RunningHubs;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class CutoutViewModel:AiMediaViewModelBase
{
    [ObservableProperty] private string sourceImagePath = string.Empty;

    protected override RunningHubWorkType RunningHubWorkType => RunningHubWorkType.Cutout;
    protected override long WorkFlowId => 1895766037193424897;

    protected override Task<bool> ValidateConfirmAsync()
    {
        if (!string.IsNullOrEmpty(SourceImagePath))
        {
            return Task.FromResult(true);
        }

        ErrorMessage = AppResources.PleaseSelectImageFirst;
        return Task.FromResult(false);
    }

    protected override async Task<bool> OnConfirmAsync()
    {
        var result = await UploadImageAsync(SourceImagePath);
        if (string.IsNullOrEmpty(result))
        {
            ErrorMessage = AppResources.FailedToUploadImage;
            return false;
        }

        var data = new NodeInfoListItem(){NodeId = "3",FieldName = "image", FieldValue = result};
        return await PublishTaskAsync([data]);
    }
}