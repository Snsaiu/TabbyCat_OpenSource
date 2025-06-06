namespace TabbyCat.Models.Appendix;

/// <summary>
/// 图片附加数据模型
/// </summary>
public sealed class ImageUrlAiAppendixModel : IAiAppendixModel<ImageUrlDataModel>
{
    [JsonProperty("type")] public  string Type => "image_url";
    [JsonProperty("image_url")] public  ImageUrlDataModel Data { get; set; }
}