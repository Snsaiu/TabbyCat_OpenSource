namespace TabbyCat.Models.AiMediaResponses;

/// <summary>
/// 文本生成视频请求模型
/// </summary>
public sealed class TextToVideoRequestModel : AiMediaRequestModelBase<TextToVideoRequestModel.TextToVideoPrompt,
    TextToVideoRequestModel.TextToVideoParameter>
{
    public class TextToVideoPrompt
    {
        [JsonProperty("prompt")] public string Prompt { get; set; } = string.Empty;
    }

    public class TextToVideoParameter
    {
        [JsonProperty("size")] public string Size { get; set; } = string.Empty;

        [JsonProperty("duration")] public int Duration { get; set; } = 5;
    }
}

/// <summary>
/// 图片+提示词生成视频
/// </summary>
public sealed class ImageToVideoRequestModel : AiMediaRequestModelBase<ImageToVideoRequestModel.ImageToVideoPrompt,
    ImageToVideoRequestModel.ImageToVideoParameter>
{
    public class ImageToVideoPrompt
    {
        [JsonProperty("prompt")] public string Prompt { get; set; } = string.Empty;
        [JsonProperty("img_url")] public string ImageUrl { get; set; } = string.Empty;
    }

    public class ImageToVideoParameter
    {
        [JsonProperty("resolution")] public string Resolution { get; set; } = string.Empty;
        [JsonProperty("prompt_extend")] public bool PromptExtend { get; set; } = true;
    }
}