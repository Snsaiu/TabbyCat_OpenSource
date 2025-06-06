namespace TabbyCat.Models.Appendix;

/// <summary>
/// 图片数据
/// </summary>
public class ImageUrlDataModel
{
    [JsonProperty("url")] public string Url { get; set; }
}