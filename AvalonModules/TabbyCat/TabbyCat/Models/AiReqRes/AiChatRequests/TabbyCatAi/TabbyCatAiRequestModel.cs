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

    [JsonProperty("enable_search")] public bool EnableUseInternet { get; set; }

    [JsonProperty("search_options")] public SearchOptions SearchOption { get; set; } = new();

    /// <summary>
    /// 是否要支持深度思考
    /// </summary>
    [JsonIgnore]
    public bool EnableDeepThinking { get; set; }

    [JsonIgnore] public bool IsTranslate { get; set; }

    [JsonProperty("translation_options")] public TranslationOptions TranslationParameter { get; set; } = new();

    public bool ShouldSerializeTranslationParameter() => IsTranslate;

    public bool ShouldSerializeModalities() => !IsTranslate;

    public bool ShouldSerializeAudio() => !IsTranslate;

    public bool ShouldSerializeEnableUseInternet() => !IsTranslate;

    public bool ShouldSerializeSearchOption() => !IsTranslate;

    public bool ShouldSerializeTemperature() => !IsTranslate;
    public bool ShouldSerializeTopP() => !IsTranslate;

    public class SearchOptions
    {
        [JsonProperty("forced_search")] public bool ForcedSearch { get; set; } = true;
        [JsonProperty("search_strategy")] public string SearchStrategy { get; set; } = "pro";
    }

    public class TabbyCatMessageItem
    {
        [JsonConverter(typeof(RoleConverter))]
        [JsonProperty("role")]
        public virtual Role Role { get; set; }

        [JsonProperty("content")] public object Content { get; set; }
    }

    public class AudioModel
    {
        [JsonProperty("voice")] public string Voice { get; set; } = "Cherry";

        [JsonProperty("format")] public string Format { get; set; } = "wav";
    }

    public class TranslationOptions
    {
        [JsonProperty("source_lang")] public string SourceLang { get; set; } = string.Empty;

        [JsonProperty("target_lang")] public string TargetLang { get; set; } = string.Empty;
    }
}