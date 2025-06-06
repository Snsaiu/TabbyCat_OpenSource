using TabbyCat.Models.AiReqRes.AiChatRequests;

namespace TabbyCat.Models.AiReqRes.AiChatResponses;

public class OllamaResponseModel : ChatResponseBase
{
    [JsonProperty("message")] public MessagesItem? Message { get; set; }
}