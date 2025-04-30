using TabbyCat.Enums;

namespace TabbyCat.Models.AiMediaResponses;

public abstract class AiMediaTaskStateResponseModelBase<TOutput, TData> : IAiMediaTaskStateResponseModel<TOutput, TData>
    where TOutput : AiMediaOutputBase<TData>
{
    public abstract string RequestId { get; set; }
    public abstract TOutput? Output { get; set; }

    [JsonIgnore] public AiMediaRunningStatus TaskStatus => Output?.TaskStatus ?? AiMediaRunningStatus.Unknown;

    public abstract IEnumerable<string>? DownloadUrls { get; }
}