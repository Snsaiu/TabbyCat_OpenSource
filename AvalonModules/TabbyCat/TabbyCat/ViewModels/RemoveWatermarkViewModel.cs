using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class RemoveWatermarkViewModel : OnlyOneImageUploadAiGenerateImageBase
{
    protected override AiMediaWorkType RunningHubWorkType => AiMediaWorkType.RemoveWatermark;
    public override string PromptDescription => AppResources.RemoveWaterMarkPromptDescription;

    protected override Task<OnlyOneImageUploadAiGenerateImageEditModel> CreateModelAsync()
    {
        return Task.FromResult(new OnlyOneImageUploadAiGenerateImageEditModel()
        {
            Model = "wanx2.1-imageedit",
            Input = new()
                { Function = "remove_watermark", Image = LocalImage, Prompt = Prompt },
            Parameters = new ImageCountParameter() { Count = SelectedCount }
        });
    }
}