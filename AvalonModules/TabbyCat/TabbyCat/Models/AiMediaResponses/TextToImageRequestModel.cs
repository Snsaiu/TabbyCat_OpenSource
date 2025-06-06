namespace TabbyCat.Models.AiMediaResponses;

/// <summary>
/// 文生图请求模型
/// </summary>
public sealed class TextToImageRequestModel:AiMediaRequestModelBase<TextToImageRequestModel.TextToImagePrompt,TextToImageRequestModel.TextToImageParameter>
{
    public sealed class TextToImagePrompt
    {
        [JsonProperty("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonProperty("negative_prompt")]
        public string NegativePrompt { get; set; } = string.Empty;
    }

    public sealed class TextToImageParameter
    {
        [JsonProperty("size")] public string Size { get; set; } = "1024*1024";
        
        [JsonProperty("n")]
        public int Count { get; set; } = 1;
    }
    
}