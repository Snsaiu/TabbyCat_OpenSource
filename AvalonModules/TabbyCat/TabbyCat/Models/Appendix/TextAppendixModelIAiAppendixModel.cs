namespace TabbyCat.Models.Appendix;

public class TextAppendixModel:IAiAppendixModel<string>
{
    [JsonProperty("type")]
    public string Type =>"text";
    
    [JsonProperty("text")]
    public string Data { get; set; }
}