using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class DoodleImageViewModel : OnlyOneImageUploadAiGenerateImageBase
{
    [ObservableProperty] private bool _isSketch = false;

    protected override AiMediaWorkType RunningHubWorkType => AiMediaWorkType.DoodleImage;
    public override string PromptDescription => AppResources.DoodlePromptDescription;

    protected override Task<OnlyOneImageUploadAiGenerateImageEditModel> CreateModelAsync()
    {
        return Task.FromResult<OnlyOneImageUploadAiGenerateImageEditModel>(new()
        {
            Input = new()
            {
                Function = "doodle",
                Image = LocalImage,
                Prompt = Prompt
            },
            Parameters = new SketchParameter() { Count = SelectedCount, IsSketch = IsSketch }
        });
    }
}