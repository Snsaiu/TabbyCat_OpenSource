using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class ImageColorizationViewModel : OnlyOneImageUploadAiGenerateImageBase
{
    protected override AiMediaWorkType RunningHubWorkType => AiMediaWorkType.ImageColorization;
    public override string PromptDescription => AppResources.ImageColorizationPromptDescription;

    protected override Task<OnlyOneImageUploadAiGenerateImageEditModel> CreateModelAsync()
    {
        return Task.FromResult(new OnlyOneImageUploadAiGenerateImageEditModel()
        {
            Input = new() { Function = "colorization", Image = LocalImage, Prompt = Prompt },
            Parameters = new ImageCountParameter() { Count = SelectedCount }
        });
    }
}