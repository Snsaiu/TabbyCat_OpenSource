using Newtonsoft.Json;

namespace TabbyCat.Ai.Models.AiReqRes.AiChatRequests.GoogleGemini;

public sealed class GoogleRequestPart
{
    [JsonProperty("text")] public string Text { get; set; } = string.Empty;
}