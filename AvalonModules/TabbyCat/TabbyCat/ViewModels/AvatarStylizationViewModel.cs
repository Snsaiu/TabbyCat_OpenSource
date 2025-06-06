using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.Bootstrap;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class AvatarStylizationViewModel : NavigationAiMediaBase<AvatarStylizationEditModel,
    AvatarStylizationEditModel.AvatarStylizationInput, object>
{
    protected override string CreateTaskUrl { get; } =
        "https://dashscope.aliyuncs.com/api/v1/services/aigc/image-generation/generation";

    protected override AiMediaWorkType RunningHubWorkType { get; } = AiMediaWorkType.AvatarStylization;
    public override string PromptDescription { get; } = String.Empty;
    public IDictionary<string, int> StyleDictionary { get; } = new Dictionary<string, int>();

    [ObservableProperty] private string _refImage;

    public AvatarStylizationViewModel()
    {
        StyleDictionary.Add(AppResources.ReferUploadedImageStyle, -1);
        StyleDictionary.Add(AppResources.RetroComics, 0);
        StyleDictionary.Add(AppResources.ThreeDFairyTale, 1);
        StyleDictionary.Add(AppResources.TwoDFairyTale, 2);
        StyleDictionary.Add(AppResources.SmallFresh, 3);
        StyleDictionary.Add(AppResources.FutureTechnology, 4);
        StyleDictionary.Add(AppResources.ChinesePaintingAncientStyle, 5);
        StyleDictionary.Add(AppResources.ArmorStyle, 6);
        StyleDictionary.Add(AppResources.ColorfulCartoon, 7);
        StyleDictionary.Add(AppResources.ElegantChineseStyle, 8);
        StyleDictionary.Add(AppResources.WelcomeNewYear, 9);
    }

    [ObservableProperty] private int _selectedStyle = -1;


    [RelayCommand]
    private async Task OpenRefImageDialog()
    {
        var files = await TuDogApplication.TopLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = AppResources.ChooseImage,
            AllowMultiple = false,
            FileTypeFilter =
                [FilePickerFileTypes.ImageAll]
        });
        if (!files.Any())
            return;
        RefImage = files[0].Path.LocalPath;
    }

    partial void OnSelectedStyleChanged(int value)
    {
        if (SelectedStyle != -1)
            RefImage = string.Empty;
    }

    protected override Task<bool> ValidateConfirmAsync()
    {
        return string.IsNullOrEmpty(LocalImage)
            ? Task.FromResult(false)
            : Task.FromResult(!string.IsNullOrEmpty(RefImage) || SelectedStyle != -1);
    }
    
    protected override Task<AvatarStylizationEditModel> CreateModelAsync()
    {
        return Task.FromResult(new AvatarStylizationEditModel()
        {
            Model = "wanx-style-repaint-v1",
            Input = new AvatarStylizationEditModel.AvatarStylizationInput()
            {
                Image = LocalImage,
                StyleIndex = SelectedStyle,
                StyleRefImage = RefImage
            }
        });
    }
}