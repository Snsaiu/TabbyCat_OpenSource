using Newtonsoft.Json;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;

namespace TabbyCat.Ai.Models.AiReqRes.AiChatResponses;


public class ChoicesItem
{

    [JsonProperty("index")]
    public int Index { get; set; }

    [JsonProperty("message")]
    public MessagesItem Message { get; set; }

    [JsonProperty("logprobs")]
    public string logprobs { get; set; }

    /// <summary>
    ///
    /// </summary>
    [JsonProperty("finish_reason")]
    public string finish_reason { get; set; }
}

public class Usage
{
    /// <summary>
    ///
    /// </summary>
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    ///
    /// </summary>
    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }

    /// <summary>
    ///
    /// </summary>
    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }

    /// <summary>
    ///
    /// </summary>
    [JsonProperty("prompt_cache_hit_tokens")]
    public int PromptCacheHitTokens { get; set; }

    /// <summary>
    ///
    /// </summary>
    [JsonProperty("prompt_cache_miss_tokens")]
    public int PromptCacheMissTokens { get; set; }
}

public abstract class ChatResponseBase
{

    [JsonProperty("model")]
    public virtual string Model { get; set; }


    //[JsonProperty("created")]
    //public virtual long Created { get; set; }
}

public class ChatResponse : ChatResponseBase
{

    [JsonProperty("choices")]
    public virtual List<ChoicesItem> Choices { get; set; }

    /// <summary>
    ///
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    ///
    /// </summary>
    [JsonProperty("object")]
    public string @Object { get; set; }

    /// <summary>
    ///
    /// </summary>
    [JsonProperty("usage")]
    public Usage Usage { get; set; }

    /// <summary>
    ///
    /// </summary>
    [JsonProperty("system_fingerprint")]
    public string SystemFingerprint { get; set; }
}