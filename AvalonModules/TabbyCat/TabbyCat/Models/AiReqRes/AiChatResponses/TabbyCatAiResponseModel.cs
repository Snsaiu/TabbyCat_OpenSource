namespace TabbyCat.Models.AiReqRes.AiChatResponses;

public class TabbyCatAiResponseModel : ChatResponseBase
{
    [JsonProperty("choices")] public List<TabbyCatResponseChoicesItem> Choices { get; set; }

    [JsonProperty("object")] public string Object { get; set; }

    [JsonProperty("usage")] public string Usage { get; set; }

    [JsonProperty("created")] public int Created { get; set; }

    [JsonProperty("system_fingerprint")] public string SystemFingerprint { get; set; }

    [JsonProperty("model")] public string Model { get; set; } = string.Empty;

    [JsonProperty("id")] public string Id { get; set; } = string.Empty;
}

public class TabbyCatResponseAudio
{
    [JsonProperty("transcript")] public string Transcript { get; set; } = string.Empty;
}

public class TabbyCatResponseDelta
{
    [JsonProperty("audio")] public TabbyCatResponseAudio Audio { get; set; }

    [JsonProperty("content")] public string Content { get; set; } = string.Empty;

    [JsonProperty("reasoning_content")] public string ReasoningContent { get; set; } = string.Empty;

}

public class TabbyCatResponseChoicesItem
{
    [JsonProperty("delta")] public TabbyCatResponseDelta Delta { get; set; }
    [JsonProperty("finish_reason")] public string FinishReason { get; set; }
    [JsonProperty("index")] public int Index { get; set; }
    [JsonProperty("logprobs")] public string Logprobs { get; set; } = string.Empty;
}