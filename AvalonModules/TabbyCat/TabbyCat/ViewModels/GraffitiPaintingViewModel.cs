using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Components.ViewModels;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class GraffitiPaintingViewModel : AiGenerateImageEditBase<GraffitiPaintingImageEditModel,
    GraffitiPaintingImageEditModel.OnlyOneImageAiGenerateImageInput, object>
{
    [ObservableProperty] private Dictionary<string, string> _styles = [];

    [ObservableProperty] private IEnumerable<int> _sketchWeights = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    [ObservableProperty] private int _selectedSketchWeight = 1;

    [ObservableProperty] private string _selectedStyle = "<auto>";

    public GraffitiPaintingViewModel()
    {
        Styles.Add(AppResources.Auto, "<auto>");
        Styles.Add(AppResources.ThreeDCartoon, "<3d cartoon>");
        Styles.Add(AppResources.Cartoon, "<anime>");
        Styles.Add(AppResources.OilPainting, "<oil painting>");
        Styles.Add(AppResources.Sketch, "<sketch>");
        Styles.Add(AppResources.ChinesePainting, "<chinese painting>");
        Styles.Add(AppResources.WaterColor, "<watercolor>");
        Styles.Add(AppResources.FlatIllustrations, "<flat illustrations>");
    }

    [RelayCommand]
    private async Task OpenDrawMaskDialog()
    {
        var result = await DialogServer.ShowDialogAsync<DrawMaskViewModel, string, string>(AppResources.DrawMask,
            AppResources.Ok,
            AppResources.Cancel, string.Empty);
        if (!result.Ok) return;

        LocalImage = result.Data;
    }

    protected override AiMediaWorkType RunningHubWorkType => AiMediaWorkType.GraffitiPainting;
    public override string PromptDescription => AppResources.GraffitiPaintingPromptDescription;

    protected override Task<GraffitiPaintingImageEditModel> CreateModelAsync()
    {
        return Task.FromResult(new GraffitiPaintingImageEditModel()
        {
            Model = "wanx-sketch-to-image-lite", Input = new() { Image = LocalImage, Prompt = Prompt },
            Parameters = new GraffitiPaintingParameter()
                { Size = "768*768", SketchWeight = SelectedSketchWeight, Count = SelectedCount, Style = SelectedStyle }
        });
    }
}