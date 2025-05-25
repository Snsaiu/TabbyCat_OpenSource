using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Components.ViewModels;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.IocAttribute;
using TuDog.Models;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class ImageErasureAndCompletionViewModel : NavigationAiMediaBase<ImageEraseCompletionEditModel,
    ImageEraseCompletionEditModel.ImageEraseCompletionEditInput,
    ImageEraseCompletionEditModel.ImageEraseCompletionParameter>
{
    public ImageErasureAndCompletionViewModel()
    {
        ShowImageCount = false;
    }

    protected override AiMediaWorkType RunningHubWorkType => AiMediaWorkType.ImageEraseCompletion;
    public override string PromptDescription { get; }

    [ObservableProperty] private string _maskImage = string.Empty;

    [ObservableProperty] private string _foregroundImage = string.Empty;

    [ObservableProperty] private bool _fastModel = true;

    [RelayCommand]
    private async Task OpenMaskDialog()
    {
        var result = await GetDrawMaskAsync();
        if (result is null)
            return;
        if (result.Ok)
            MaskImage = result.Data;
    }

    private async Task<DialogResultData<string>?> GetDrawMaskAsync()
    {
        if (string.IsNullOrEmpty(LocalImage) || !File.Exists(LocalImage))
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectImageFirst, AppResources.Warning,
                AppResources.Ok);
            return null;
        }

        var result = await DialogServer.ShowDialogAsync<DrawMaskViewModel, string, string>(AppResources.DrawMask,
            AppResources.Ok,
            AppResources.Cancel, LocalImage);
        return result;
    }

    [RelayCommand]
    private async Task OpenForegroundImageDialog()
    {
        var result = await GetDrawMaskAsync();
        if (result is null)
            return;
        if (result.Ok)
            ForegroundImage = result.Data;
    }

    protected override async Task<bool> ValidateConfirmAsync()
    {
        return await base.ValidateConfirmAsync() && !string.IsNullOrEmpty(LocalImage) && File.Exists(LocalImage)
               && !string.IsNullOrEmpty(Prompt)
               && !string.IsNullOrEmpty(MaskImage) && File.Exists(MaskImage)
               && !string.IsNullOrEmpty(ForegroundImage) && File.Exists(ForegroundImage);
    }

    protected override Task<ImageEraseCompletionEditModel> CreateModelAsync()
    {
        return Task.FromResult(new ImageEraseCompletionEditModel()
        {
            Input = new()
            {
                Image = LocalImage,
                MaskImage = MaskImage,
                ForegroundImage = ForegroundImage
            },
            Parameters = new() { FastMode = FastModel }
        });
    }

    protected override async Task<ImageEraseCompletionEditModel> CreatePublishModelAsync()
    {
        var model = await CreateModelAsync();
        var file = await UploadFileAsync(model);
        model.Input.Image = file;

        model.Input.MaskImage = await GetUploadImageUrlAsync(model.Input.MaskImage);
        model.Input.ForegroundImage = await GetUploadImageUrlAsync(model.Input.ForegroundImage);

        return model;
    }
    
}