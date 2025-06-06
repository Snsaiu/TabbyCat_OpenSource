using TabbyCat.Models.AiReqRes.AiChatRequests;

namespace TabbyCat.Models.AiReqRes.AiChatResponses;

public class OpenAiResponseModel : ChatResponse<OpenAiMessageData>
{
}

public sealed class OpenAiMessageData : ChoicesItem
{
    [JsonProperty("delta")] public override MessagesItem? Message { get; set; }
}