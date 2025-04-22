using TabbyCat.Models.AiReqRes.AiChatRequests.OpenAi;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Models.Appendix;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Models.AiReqRes.AiChatRequests.TabbyCatAi;

public class TabbyCatAiRequestModel : OpenAiRequestModel
{
    [JsonProperty("modalities")] public IEnumerable<string> Modalities { get; set; } = ["text", "audio"];

    [JsonProperty("audio")] public AudioModel Audio { get; set; } = new();

    [JsonIgnore] public override List<MessagesItem> Messages { get; set; } = [];


    [JsonProperty("messages")] public List<TabbyCatMessageItem> Contents { get; set; } = [];
}

public class TabbyCatMessageItem
{
    [JsonConverter(typeof(RoleConverter))]
    [JsonProperty("role")]
    public virtual Role Role { get; set; }

    [JsonProperty("content")] public IEnumerable<IAiAppendixModel> AppendixModels { get; set; } = [];
}

public class AudioModel
{
    [JsonProperty("voice")] public string Voice { get; set; } = "Cherry";

    [JsonProperty("format")] public string Format { get; set; } = "wav";
}