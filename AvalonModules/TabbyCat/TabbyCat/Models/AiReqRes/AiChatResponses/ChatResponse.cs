using System.Collections.Generic;
using Newtonsoft.Json;
using TabbyCat.Models.AiReqRes.AiChatRequests;

namespace TabbyCat.Models.AiReqRes.AiChatResponses;

public class ChoicesItem
{
    [JsonProperty("index")] public int Index { get; set; }

    [JsonProperty("message")] public virtual MessagesItem? Message { get; set; }

    [JsonProperty("logprobs")] public string LogProbs { get; set; } = string.Empty;

    /// <summary>
    ///
    /// </summary>
    [JsonProperty("finish_reason")]
    public string finish_reason { get; set; } = string.Empty;
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
    [JsonProperty("model")] public virtual string Model { get; set; } = string.Empty;


    //[JsonProperty("created")]
    //public virtual long Created { get; set; }
}

public class ChatResponse<T> : ChatResponseBase
{
    [JsonProperty("choices")] public virtual List<T> Choices { get; set; } = [];

    [JsonProperty("id")] public virtual string Id { get; set; } = string.Empty;

    [JsonProperty("object")] public virtual string @Object { get; set; } = string.Empty;


    [JsonProperty("usage")] public virtual Usage? Usage { get; set; }

    [JsonProperty("system_fingerprint")] public virtual string SystemFingerprint { get; set; } = string.Empty;
}