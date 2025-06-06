namespace TabbyCat.Models.AiReqRes.AiChatResponses;

public class GoogleGeminiResponseModel : ChatResponseBase
{
    [JsonIgnore]
    public override string Model
    {
        get => base.Model;
        set => base.Model = value;
    }

    [JsonProperty("candidates")] public List<CandidatesItem> Candidates { get; set; } = [];

    [JsonProperty("usageMetadata")] public UsageMetadata UsageMetadata { get; set; } = new();

    [JsonProperty("modelVersion")] public string ModelVersion { get; set; } = string.Empty;
}

public class PartsItem
{
    [JsonProperty("text")] public string Text { get; set; } = string.Empty;
}

public class Content
{
    [JsonProperty("parts")] public List<PartsItem> Parts { get; set; } = [];

    [JsonProperty("role")] public string Role { get; set; } = string.Empty;
}

public class SafetyRatingsItem
{
    [JsonProperty("category")] public string Category { get; set; } = string.Empty;

    [JsonProperty("probability")] public string Probability { get; set; } = string.Empty;
}

public class CandidatesItem
{
    [JsonProperty("content")] public Content Content { get; set; } = new();

    [JsonProperty("finishReason")] public string FinishReason { get; set; } = string.Empty;

    [JsonProperty("index")] public int Index { get; set; }

    [JsonProperty("safetyRatings")] public List<SafetyRatingsItem> SafetyRatings { get; set; } = [];
}

public class UsageMetadata
{
    [JsonProperty("promptTokenCount")] public int PromptTokenCount { get; set; }

    [JsonProperty("candidatesTokenCount")] public int CandidatesTokenCount { get; set; }

    [JsonProperty("totalTokenCount")] public int TotalTokenCount { get; set; }
}