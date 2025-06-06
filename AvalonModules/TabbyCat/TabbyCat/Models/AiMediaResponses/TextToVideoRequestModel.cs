namespace TabbyCat.Models.AiMediaResponses;

/// <summary>
/// 文本生成视频请求模型
/// </summary>
public sealed class TextToVideoRequestModel:AiMediaRequestModelBase<TextToVideoRequestModel.TextToVideoPrompt,TextToVideoRequestModel.TextToVideoParameter>
{
    public class TextToVideoPrompt
    {
        [JsonProperty("prompt")] public string Prompt { get; set; } = string.Empty;
    }
    
    public class TextToVideoParameter
    {
        [JsonProperty("size")] public string Size { get; set; } = string.Empty;

        [JsonProperty("duration")]
        public int Duration { get; set; } = 5;

    }
    
}