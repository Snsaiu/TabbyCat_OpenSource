using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Converters;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TuDog.Bootstrap;

namespace TabbyCat.Models.AiReqRes.AiChatRequests;

public abstract class MessageSessionBase:ModelBase
{
    [JsonProperty("messages")] public virtual List<MessagesItem> Messages { get; set; } = [];

    [JsonProperty("model")] public virtual string Model { get; set; } = string.Empty;

    [JsonProperty("stream")] public virtual bool Stream { get; set; } = true;
}

public partial class MessagesItem : ModelBase
{
    [JsonProperty("content")] [ObservableProperty] [property: JsonIgnore]
    private string content;

    [JsonIgnore] public Guid Key { get; set; }

    [JsonConverter(typeof(RoleConverter))]
    [JsonProperty("role")]
    public virtual Role Role { get; set; }

    [property: JsonIgnore] [ObservableProperty]
    private bool streamEnd = true;

    [property: JsonIgnore] [ObservableProperty]
    private bool isFavourite;

    [ObservableProperty] [property: JsonIgnore]
    private bool showMarkdownMode;

}

public class ResponseFormat
{
    [JsonProperty("type")] public string? Type { get; set; }
}

public class StreamOption
{
    [JsonProperty("include_usage")] public bool IncludeUsage { get; set; } = true;
}

public abstract class TemperatureAndTopMessageSessionBase : MessageSessionBase
{
    [JsonProperty("temperature")] public virtual double Temperature { get; set; } = 0.5;
    [JsonProperty("top_p")] public virtual double TopP { get; set; } = 0.5;
}

public abstract class MaxTokensMessageSession : TemperatureAndTopMessageSessionBase
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