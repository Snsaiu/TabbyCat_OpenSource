namespace TabbyCat.Models.AiReqRes;

public abstract class AiMessageBase
{
    [JsonProperty("model")] public string Model { get; set; } = string.Empty;
}