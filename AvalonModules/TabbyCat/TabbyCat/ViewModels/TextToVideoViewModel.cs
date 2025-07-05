using System.Collections.ObjectModel;
using TabbyCat.Extensions;
using TabbyCat.Models.AiMediaResponses;
using TabbyCat.Repository.Entities.AiMediaEntities;
using TabbyCat.Shared.Enums;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

/// <summary>
/// 提示词生成视频
/// </summary>
[Register]
public partial class TextToVideoViewModel : Bases.GenerateVideoViewModelBase<TextToVideoRequestModel,
    TextToVideoRequestModel.TextToVideoPrompt, TextToVideoRequestModel.TextToVideoParameter>
{
    protected override AiMediaWorkType RunningHubWorkType => AiMediaWorkType.TextToVideo;

    protected override string CreateTaskUrl =>
        "https://dashscope.aliyuncs.com/api/v1/services/aigc/video-generation/video-synthesis";

    protected override Task<TextToVideoRequestModel> CreatePublishModelAsync()
    {
        return Task.FromResult(new TextToVideoRequestModel
        {
            Input = new TextToVideoRequestModel.TextToVideoPrompt { Prompt = Prompt }, Model = "wanx2.1-t2v-turbo",
            Parameters = new TextToVideoRequestModel.TextToVideoParameter { Duration = 5, Size = SelectedSize }
        });
    }
}