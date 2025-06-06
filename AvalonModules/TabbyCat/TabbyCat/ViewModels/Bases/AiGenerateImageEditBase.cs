using System.IO;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Extensions;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Languages;
using TuDog.Bootstrap;

namespace TabbyCat.ViewModels.Bases;

/// <summary>
/// 通用图片编辑基类
/// </summary>
/// <typeparam name="TPublishModel"></typeparam>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TParameter"></typeparam>
public abstract partial class
    AiGenerateImageEditBase<TPublishModel, TInput, TParameter> : AiMediaViewModelBase<TPublishModel, TInput,
        TParameter>
    where TPublishModel : UploadSourceImageAiGenerateImageEditModelBase<TInput, TParameter>
    where TInput : OnlyOneImageInput
{
    
    
    protected override string CreateTaskUrl =>
        "https://dashscope.aliyuncs.com/api/v1/services/aigc/image2image/image-synthesis";

    protected override string DownloadFileExtension => ".png";


    [RelayCommand]
    private async Task OpenPickImageDialog()
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
        LocalImage = files[0].Path.LocalPath;
    }


    public IEnumerable<int> Count => [1, 2, 3, 4];

    [ObservableProperty] private int _selectedCount = 1;

    /// <summary>
    /// 是否显示强度
    /// </summary>
    [ObservableProperty] private bool _showStrength = false;

    [ObservableProperty] private double _strength = 0.5;

    [ObservableProperty] private bool _showImageCount = true;

    /// <summary>
    /// 提示词
    /// </summary>
    [ObservableProperty] private string _prompt = string.Empty;

    public abstract string PromptDescription { get; }

    /// <summary>
    /// 原图
    /// </summary>
    [ObservableProperty] private string _localImage = string.Empty;

    /// <summary>
    ///
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="Exception">请求失败时候抛出</exception>
    protected async Task<string> UploadFileAsync(TPublishModel model)
    {
        var result = await UploadImageAsync(model.Input.Image);
        if (result.Ok)
            return result.Data;
        throw new(result.ErrorMsg);
    }

    protected abstract Task<TPublishModel> CreateModelAsync();


    protected  override async Task<TPublishModel> CreatePublishModelAsync()
    {
        var model = await CreateModelAsync();
        var file = await UploadFileAsync(model);
        model.Input.Image = file;
        return model;
    }

}

public abstract class
    NavigationAiMediaBase<TPublishModel, TInput, TParameter> : AiGenerateImageEditBase<TPublishModel,
    TInput, TParameter>,IMediaNavigation
    where TPublishModel : UploadSourceImageAiGenerateImageEditModelBase<TInput, TParameter>
    where TInput : OnlyOneImageInput
{
    public Task NavigationAsync(object? parameter)
    {
        if (parameter is IReadOnlyCollection<string> { Count: 1 } and var paths)
        {
            var file = paths.First();
            this.LocalImage = file;
            return Task.CompletedTask;
        }

        throw new ArgumentException();

    }
}

/// <summary>
/// 只要上传一张图+提示词就可以生成图片
/// </summary>
public abstract class OnlyOneImageUploadAiGenerateImageBase : NavigationAiMediaBase<
    OnlyOneImageUploadAiGenerateImageEditModel,
    OnlyOneImageUploadAiGenerateImageEditModel.OnlyOneImageAiGenerateImageInput, object>
{
    protected override Task<bool> ValidateConfirmAsync()
    {
        return Task.FromResult( !string.IsNullOrEmpty(LocalImage)&&File.Exists(LocalImage)&&!string.IsNullOrEmpty(Prompt));
    }
}

/// <summary>
/// 需要上传一张mask+一张原图+提示词可以生成图片
/// </summary>
public abstract partial class MaskImageUploadAiGenerateImageBase : NavigationAiMediaBase<
    MaskImageUploadAiGenerateImageEditModel, MaskImageUploadAiGenerateImageEditModel.MaskImageUploadImageInput,
    ImageCountParameter>
{
    [ObservableProperty] private string _maskImage = string.Empty;

    protected ICacheFolderService CacheFolderService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<ICacheFolderService>();

    protected override Task<bool> ValidateConfirmAsync()
    {
        if (string.IsNullOrEmpty(LocalImage) || !File.Exists(LocalImage))
        {
            ErrorMessage = AppResources.PleaseSelectImageFirst;
            return Task.FromResult(false);
        }

        if (string.IsNullOrEmpty(MaskImage) || !File.Exists(MaskImage))
        {
            ErrorMessage = AppResources.PleaseDrawMaskFirst;
            return Task.FromResult(false);
        }

        if (string.IsNullOrEmpty(Prompt))
        {
            ErrorMessage = AppResources.PleaseInputPrompt;
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }


    protected override async Task<MaskImageUploadAiGenerateImageEditModel> CreatePublishModelAsync()
    {
        var model = await CreateModelAsync();

        var maskImage = Path.Combine(CacheFolderService.Get(), $"{Guid.NewGuid():N}.png");

        Util.InvertImageColors(model.Input.MaskImage, maskImage);

        var file = await UploadFileAsync(model);
        var maskResult = await UploadImageAsync(maskImage);

        if (!maskResult.Ok)
            throw new Exception(maskResult.ErrorMsg);
        model.Input.Image = file;
        model.Input.MaskImage = maskResult.Data;
        return model;
    }
}