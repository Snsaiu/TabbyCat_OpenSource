using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class CommandEditImageViewModel : OnlyOneImageUploadAiGenerateImageBase
{
    protected override AiMediaWorkType RunningHubWorkType => AiMediaWorkType.CommandEditImage;
    public override string PromptDescription => AppResources.CommandEditImagePromptDescription;

    protected override Task<OnlyOneImageUploadAiGenerateImageEditModel> CreateModelAsync()
    {
        return Task.FromResult(new OnlyOneImageUploadAiGenerateImageEditModel()
        {
            Model = "wanx2.1-imageedit", Input =
                new()
                    { Function = "description_edit", Prompt = Prompt, Image = LocalImage },
            Parameters = new ImageCountParameter() { Count = SelectedCount }
        });
    }
}