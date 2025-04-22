namespace TabbyCat.Models.Appendix;

/// <summary>
/// 音频附加数据模型
/// </summary>
public sealed class AudioAppendixModel : IAiAppendixModel<AudioDataModel>
{
    [JsonProperty("type")] public  string Type => "input_audio";

    [JsonProperty("input_audio")] public  AudioDataModel Data { get; set; }
}