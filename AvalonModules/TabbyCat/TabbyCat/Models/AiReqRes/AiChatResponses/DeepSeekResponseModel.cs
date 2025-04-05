namespace TabbyCat.Models.AiReqRes.AiChatResponses;

public class Delta
{
    [JsonProperty("content")] public string Content { get; set; } = string.Empty;
}

public class DeepSeekChoicesItem
{
    public int index { get; set; }

    public Delta? delta { get; set; }


    public string logprobs { get; set; } = string.Empty;


    public string finish_reason { get; set; } = string.Empty;
}

public sealed class DeepSeekResponseModel : ChatResponseBase
{
    [JsonProperty("id")] public string Id { get; set; } = string.Empty;

    [JsonProperty("object")] public string ObjectData { get; set; }= string.Empty;

    [JsonProperty("created")] public int Created { get; set; }

    [JsonProperty("system_fingerprint")] public string SystemFingerprint { get; set; }= string.Empty;
    [JsonProperty("choices")] public List<DeepSeekChoicesItem> Choices { get; set; } = [];

    [JsonProperty("usage")] public string Usage { get; set; }= string.Empty;
}