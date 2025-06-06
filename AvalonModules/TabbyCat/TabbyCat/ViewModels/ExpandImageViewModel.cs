using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class ExpandImageViewModel:OnlyOneImageUploadAiGenerateImageBase
{

    [ObservableProperty]
    private double _top = 1.5;
    
    [ObservableProperty]
    private double _left = 1.5;
    
    [ObservableProperty]
    private double _right = 1.5;
    
    [ObservableProperty]
    private double _bottom = 1.5;
    
    protected override AiMediaWorkType RunningHubWorkType => AiMediaWorkType.ExpandImage;
    public override string PromptDescription => AppResources.ExpandImagePromptDescription;
    protected override Task<OnlyOneImageUploadAiGenerateImageEditModel> CreateModelAsync()
    {
        return Task.FromResult<OnlyOneImageUploadAiGenerateImageEditModel>(new OnlyOneImageUploadAiGenerateImageEditModel()
        {
            Input = new OnlyOneImageUploadAiGenerateImageEditModel.OnlyOneImageAiGenerateImageInput(){Function = "expand", Image = LocalImage,Prompt = Prompt},
            Model = "wanx2.1-imageedit",
            Parameters = new ExpandImageParameter(){Count = SelectedCount,BottomScale = Bottom,LeftScale = Left,RightScale = Right,TopScale = Top}
        });
    }
}