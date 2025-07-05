using System.IO;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TuDog.Bootstrap;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

/// <summary>
///  图片+提示词生成视频
/// </summary>
[Register]
internal partial class ImageToVideoViewModel : Bases.GenerateVideoViewModelBase<ImageToVideoRequestModel,
    ImageToVideoRequestModel.ImageToVideoPrompt, ImageToVideoRequestModel.ImageToVideoParameter>
{
    public ImageToVideoViewModel()
    {
        SelectedSize = "480P";
    }


    [RelayCommand]
    private async Task OpenPickImageDialog()
    {
        var files = await TuDogApplication.TopLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = AppResources.ChooseImage,
            AllowMultiple = false,
            FileTypeFilter =
                [FilePickerFileTypes.ImageAll]
        });
        if (!files.Any())
            return;
        ImageUrl = files[0].Path.LocalPath;
    }

    protected override AiMediaWorkType RunningHubWorkType { get; } = AiMediaWorkType.ImageToVideo;

    protected override string CreateTaskUrl { get; } =
        "https://dashscope.aliyuncs.com/api/v1/services/aigc/video-generation/video-synthesis";

    [ObservableProperty] private string _imageUrl = string.Empty;

    protected override async Task<bool> ValidateConfirmAsync()
    {
        return await base.ValidateConfirmAsync() && !string.IsNullOrEmpty(ImageUrl) && File.Exists(ImageUrl);
    }

    public override IEnumerable<string> Size { get; } = ["480P", "720P"];

    protected override async Task<ImageToVideoRequestModel> CreatePublishModelAsync()
    {
        var imageResource = await UploadImageAsync(ImageUrl);
        if (imageResource is not { Ok: true, Data: var image })
            throw new Exception("将图片上传到云端并下载失败");

        var result = new ImageToVideoRequestModel
        {
            Input = new ImageToVideoRequestModel.ImageToVideoPrompt { Prompt = Prompt, ImageUrl = image },
            Model = "wanx2.1-i2v-turbo",
            Parameters = new ImageToVideoRequestModel.ImageToVideoParameter { Resolution = SelectedSize }
        };
        return result;
    }
}