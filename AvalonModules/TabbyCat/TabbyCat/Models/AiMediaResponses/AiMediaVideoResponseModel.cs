using TabbyCat.Enums;

namespace TabbyCat.Models.AiMediaResponses;

public sealed class AiMediaVideoResponseModel : AiMediaTaskStateResponseModelBase<AiMediaVideoResponseModel.AiMediaVideoOutputData, string>
{
    public sealed class AiMediaVideoOutputData : AiMediaOutputBase<string>
    {
        [JsonProperty("video_url")]
        public override string Data { get; set; }
    }
    
    [JsonProperty("request_id")] public override string RequestId { get; set; }
    [JsonProperty("output")] public override AiMediaVideoOutputData? Output { get; set; }

    [JsonIgnore]
    public override IEnumerable<string>? DownloadUrls =>
        TaskStatus != AiMediaRunningStatus.Success ? null : string.IsNullOrEmpty(Output?.Data) ? null : [Output?.Data];
}

