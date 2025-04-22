namespace TabbyCat.Models.Appendix;

public class AudioDataModel
{
    [JsonProperty("format")] public string Format { get; set; } = "wav";

    [JsonProperty("data")] public string Data { get; set; }
}