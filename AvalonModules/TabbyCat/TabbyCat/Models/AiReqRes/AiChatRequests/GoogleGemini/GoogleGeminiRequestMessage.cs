using System.Collections.Generic;
using TabbyCat.Shared.Enums;


namespace TabbyCat.Models.AiReqRes.AiChatRequests.GoogleGemini;

public sealed class GoogleGeminiRequestMessage
{
    [JsonConverter(typeof(RoleConverter))]
    [JsonProperty("role")]
    public Role Role { get; set; }

    [JsonProperty("parts")] public required List<GoogleRequestPart> Parts { get; set; }
}