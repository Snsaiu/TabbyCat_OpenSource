using TabbyCat.Enums;

namespace TabbyCat.Models.AiMediaResponses;

public sealed class
    AiMediaImageResponseModel : AiMediaTaskStateResponseModelBase<AiMediaImageOutputData, IEnumerable<ImageResultData>>
{
    [JsonProperty("request_id")] public override string RequestId { get; set; }
    [JsonProperty("output")] public override AiMediaImageOutputData? Output { get; set; }
    [JsonIgnore]
    public override IEnumerable<string>? DownloadUrls =>
        TaskStatus != AiMediaRunningStatus.Success ? null : Output?.Data.Select(x => x.Url);
}

public sealed class AiMediaImageOutputData : AiMediaOutputBase<IEnumerable<ImageResultData>>
{
    [JsonProperty("results")] public override IEnumerable<ImageResultData> Data { get; set; }
}

public sealed class ImageResultData
{
    [JsonProperty("orig_prompt")] public string OrigPrompt { get; set; } = string.Empty;

    [JsonProperty("actual_prompt")] public string ActualPrompt { get; set; } = string.Empty;

    [JsonProperty("url")] public string Url { get; set; } = string.Empty;
}

public sealed class AiMediaOnlyOneImageResultResponseModel : AiMediaTaskStateResponseModelBase<
    AiMediaOnlyOneImageResultResponseModel.OnlyOneImageResultModel, string>
{
    public class OnlyOneImageResultModel : AiMediaOutputBase<string>
    {
        [JsonProperty("output_image_url")] public override string Data { get; set; }
    }

    [JsonProperty("request_id")] public override string RequestId { get; set; }
    [JsonProperty("output")] public override OnlyOneImageResultModel? Output { get; set; }
    [JsonIgnore] public override IEnumerable<string>? DownloadUrls { get; }
}