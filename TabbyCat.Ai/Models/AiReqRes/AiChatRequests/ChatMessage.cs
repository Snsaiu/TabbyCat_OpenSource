using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Ai.Models.AiReqRes.AiChatRequests;

public abstract class MessageSessionBase
{
    [JsonProperty("messages")] public virtual List<MessagesItem> Messages { get; set; } = [];

    [JsonProperty("model")] public virtual string Model { get; set; } = string.Empty;

    [JsonProperty("stream")] public virtual bool Stream { get; set; } = false;
}

public class MessagesItem
{
    [JsonProperty("content")] public virtual string Content { get; set; } = string.Empty;

    [JsonConverter(typeof(RoleConverter))]
    [JsonProperty("role")]
    public virtual Role Role { get; set; }
}

public class ResponseFormat
{
    [JsonProperty("type")] public string? Type { get; set; }
}

public class StreamOption
{
    [JsonProperty("include_usage")] public bool IncludeUsage { get; set; }
}

public abstract class MaxTokensMessageSession : MessageSessionBase
{
    [JsonProperty("max_tokens")] public int MaxTokens { get; set; } = 2048;
}


public abstract class MessageSession : MaxTokensMessageSession
{
    [JsonProperty("frequency_penalty")] public int FrequencyPenalty { get; set; }

    [JsonProperty("presence_penalty")] public int PresencePenalty { get; set; }

    [JsonProperty("response_format")] public ResponseFormat ResponseFormat { get; set; } = new() { Type = "text" };

    [JsonProperty("stop")] public List<string>? Stop { get; set; }

    [JsonProperty("stream_options")] public StreamOption StreamOptions { get; set; } = new();

    [JsonProperty("temperature")] public double Temperature { get; set; }

    [JsonProperty("top_p")] public double TopP { get; set; }

    [JsonProperty("tools")] public object? Tools { get; set; }

    [JsonProperty("tool_choice")] public string ToolChoice { get; set; } = "none";

    [JsonProperty("logprobs")] public bool Logprobs { get; set; }

    [JsonProperty("top_logprobs")] public object? TopLogprobs { get; set; } = null;


    public HttpRequestMessage BuildHttpRequestMessage(AiApiModelBase aiModel)
    {
        if (aiModel is AiApiDomainModelBase apiDomain)
        {
            var urlPath = apiDomain.ApiDomain;
            if (aiModel is IApiPath apiPath) urlPath = $"{apiDomain.ApiDomain}{apiPath.ApiPath}";
            var request = new HttpRequestMessage(HttpMethod.Post, urlPath);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {apiDomain.ApiKey}");

            return request;
        }

        throw new NotImplementedException();
    }
}

public class RoleConverter : StringEnumConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString().ToLower());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return reader.Value.ToString() switch
        {
            "user" => Role.User,
            "system" => Role.System,
            "assistant" => Role.Assistant,
            "model" => Role.Model,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}