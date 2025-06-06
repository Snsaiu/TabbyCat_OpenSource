using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class ImageSuperResolutionViewModel : OnlyOneImageUploadAiGenerateImageBase
{
    protected override AiMediaWorkType RunningHubWorkType => AiMediaWorkType.ImageSuperResolution;
    public override string PromptDescription => AppResources.ImageSuperResolutionPromptDescription;

    [ObservableProperty] private IEnumerable<int> _factors = [1, 2, 3, 4];

    [ObservableProperty] private int _selectedFactor = 1;

    protected override Task<OnlyOneImageUploadAiGenerateImageEditModel> CreateModelAsync()
    {
        return Task.FromResult(new OnlyOneImageUploadAiGenerateImageEditModel()
        {
            Model = "wanx2.1-imageedit",
            Input = new() { Function = "super_resolution", Image = LocalImage, Prompt = Prompt },
            Parameters = new ImageSuperResolutionParameter() { Count = SelectedCount, UpscaleFactor = SelectedFactor }
        });
    }
}