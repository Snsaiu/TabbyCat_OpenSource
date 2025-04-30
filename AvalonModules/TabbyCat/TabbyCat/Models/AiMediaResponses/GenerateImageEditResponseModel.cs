using TabbyCat.Enums;

namespace TabbyCat.Models.AiMediaResponses;

public sealed class GenerateImageEditResponseModel : AiMediaTaskStateResponseModelBase<
    GenerateImageEditResponseModel.GenerateImageEditOutputData, IEnumerable<GenerateImageEditResponseModel.Url>>
{
    public sealed class GenerateImageEditOutputData : AiMediaOutputBase<IEnumerable<Url>>
    {
        [JsonProperty("results")]
        public override IEnumerable<Url> Data { get; set; }
    }
    
    public sealed class Url
    {
        [JsonProperty("url")]
        public string DonwloadLink { get; set; }
    }

    [JsonProperty("request_id")] public override string RequestId { get; set; }

    [JsonProperty("output")] public override GenerateImageEditOutputData Output { get; set; }

    [JsonIgnore]
    public override IEnumerable<string>? DownloadUrls =>
        TaskStatus != AiMediaRunningStatus.Success ? null : Output?.Data.Select(x => x.DonwloadLink);
}