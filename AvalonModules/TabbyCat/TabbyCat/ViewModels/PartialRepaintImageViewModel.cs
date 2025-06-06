using System.IO;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Components.ViewModels;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TuDog.Interfaces.IDialogServers;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

/// <summary>
/// 局部重绘
/// </summary>
[Register]
public sealed partial class PartialRepaintImageViewModel():Bases.MaskImageUploadAiGenerateImageBase
{

    [RelayCommand]
    private async Task OpenMaskDialog()
    {
        if (string.IsNullOrEmpty(LocalImage) || !File.Exists(LocalImage))
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectImageFirst, AppResources.Warning,
                AppResources.Ok);
            return;
        }

        var result = await DialogServer.ShowDialogAsync<DrawMaskViewModel, string, string>(AppResources.DrawMask,
            AppResources.Ok, AppResources.Cancel, LocalImage);
        if(!result.Ok)
            return;
        MaskImage = result.Data;
    }

    protected override AiMediaWorkType RunningHubWorkType => AiMediaWorkType.PartialRepaintImage;
    public override string PromptDescription => AppResources.PartialRepaintImagePromptDescription;
    protected override Task<MaskImageUploadAiGenerateImageEditModel> CreateModelAsync()
    {
        return Task.FromResult(new MaskImageUploadAiGenerateImageEditModel()
        {
            Model = "wanx2.1-imageedit",
            Input = new MaskImageUploadAiGenerateImageEditModel.MaskImageUploadImageInput()
            {
                Function = "description_edit_with_mask", Image = LocalImage, MaskImage = MaskImage, Prompt = Prompt
            },
            Parameters = new ImageCountParameter() { Count = SelectedCount }
        });
    }
}