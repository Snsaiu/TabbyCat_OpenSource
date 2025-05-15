using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.ViewModels.Bases;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class TextToImageViewModel : AiMediaViewModelBase<TextToImageRequestModel,
    TextToImageRequestModel.TextToImagePrompt, TextToImageRequestModel.TextToImageParameter>
{
    protected override AiMediaWorkType RunningHubWorkType { get; } = AiMediaWorkType.TextToImage;

    [ObservableProperty] private string imageDescription = string.Empty;

    [ObservableProperty] private string negativePrompt = string.Empty;

    [ObservableProperty] private ObservableCollection<string> imageSizes =
    [
        "512*512", "1024*1024", "1280*720"
    ];

    [ObservableProperty] private ObservableCollection<int> _count = [1, 2, 3, 4];

    [ObservableProperty] private int _selectCount = 1;

    [ObservableProperty] private string selectImageSize = "512*512";

    protected override string DownloadFileExtension => ".png";

    protected override Task<bool> ValidateConfirmAsync()
    {
        return Task.FromResult(!string.IsNullOrEmpty(ImageDescription));
    }

    protected override string CreateTaskUrl =>
        "https://dashscope.aliyuncs.com/api/v1/services/aigc/text2image/image-synthesis";

    protected override Task<TextToImageRequestModel> CreatePublishModelAsync()
    {
        return Task.FromResult(new TextToImageRequestModel()
        {
            Input = new() { Prompt = ImageDescription }, Model = "wanx2.1-t2i-turbo", Parameters =
                new() { Count = SelectCount, Size = SelectImageSize }
        });
    }
}