namespace TabbyCat.Models.AiMediaResponses;

public abstract class AiMediaRequestModelBase<TInput,TParameters>
{
    [JsonProperty("model")] public virtual string Model { get; set; } = string.Empty;

    [JsonProperty("input")]
    public virtual TInput Input { get; set; }

    [JsonProperty("parameters")]
    public virtual TParameters Parameters { get; set; }
    
    public bool ShouldSerializeParameters() => Parameters != null;
}