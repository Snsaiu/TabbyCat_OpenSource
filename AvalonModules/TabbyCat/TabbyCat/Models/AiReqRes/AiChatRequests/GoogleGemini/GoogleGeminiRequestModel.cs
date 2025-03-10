namespace TabbyCat.Models.AiReqRes.AiChatRequests.GoogleGemini;

public class GoogleGeminiRequestModel : MessageSessionBase
{
    [JsonIgnore]
    public override List<MessagesItem> Messages
    {
        get => base.Messages;
        set => base.Messages = value;
    }

    [JsonProperty("contents")] public List<GoogleGeminiRequestMessage> Content { get; set; } = [];

    [JsonIgnore]
    public override string Model
    {
        get => base.Model;
        set => base.Model = value;
    }

    [JsonIgnore]
    public override bool Stream
    {
        get => base.Stream;
        set => base.Stream = value;
    }
}