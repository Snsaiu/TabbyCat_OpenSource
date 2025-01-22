using Newtonsoft.Json;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;

namespace TabbyCat.Ai.Models.AiReqRes.AiChatResponses;

public class OllamaResponseModel : ChatResponseBase
{

    [JsonProperty("message")]
    public MessagesItem Message { get; set; }
}