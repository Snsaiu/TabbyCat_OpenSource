namespace TabbyCat.Models.AiReqRes.AiChatResponses;

public class ClaudeResponseModel : ChatResponseBase
{
    [JsonProperty("content")] public List<ClaudeContentItem> Content { get; set; } = [];

    [JsonProperty("id")] public string Id { get; set; } = string.Empty;

    [JsonProperty("role")] public string Role { get; set; } = string.Empty;

    [JsonProperty("stop_reason")] public string StopReason { get; set; } = string.Empty;

    [JsonProperty("stop_sequence")] public string StopSequence { get; set; } = string.Empty;

    [JsonProperty("type")] public string Type { get; set; } = string.Empty;

    [JsonProperty("usage")] public required ClaudeUsage Usage { get; set; }
}

public class ClaudeContentItem
{
    [JsonProperty("text")] public string Text { get; set; } = string.Empty;
    [JsonProperty("type")] public string Type { get; set; } = string.Empty;
}

public class ClaudeUsage
{
    [JsonProperty("input_tokens")] public int InputTokens { get; set; }
    [JsonProperty("output_tokens")] public int OutputTokens { get; set; }
}