using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Extensions;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Repository.Entities.AiMediaEntities;
using TabbyCat.Shared.Enums;
using TabbyCat.ViewModels.Bases;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class TextToVideoViewModel: AiMediaViewModelBase<TextToVideoRequestModel,
    TextToVideoRequestModel.TextToVideoPrompt, TextToVideoRequestModel.TextToVideoParameter>
{
    protected override AiMediaWorkType RunningHubWorkType => AiMediaWorkType.TextToVideo;
    protected override string DownloadFileExtension => ".mp4";

    protected override string CreateTaskUrl => "https://dashscope.aliyuncs.com/api/v1/services/aigc/video-generation/video-synthesis";


    [ObservableProperty]
    private string _prompt=string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _size=["832*480","480*832","624*624","1280*720","720*1280","960*960","832*1088","1088*832"];

    [ObservableProperty]
    private string _selectedSize = "1280*720";

    protected override Task<bool> ValidateConfirmAsync()
    {
        return Task.FromResult(!string.IsNullOrWhiteSpace(Prompt));
    }

    protected override Task<TextToVideoRequestModel> CreatePublishModelAsync()
    {
        return Task.FromResult(new TextToVideoRequestModel()
        {
            Input = new() { Prompt = Prompt }, Model = "wanx2.1-t2v-turbo",
            Parameters = new() { Duration = 5, Size = SelectedSize }
        });
    }

}