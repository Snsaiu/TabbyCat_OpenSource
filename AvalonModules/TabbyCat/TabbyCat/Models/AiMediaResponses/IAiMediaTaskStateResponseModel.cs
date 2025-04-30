using TabbyCat.Enums;

namespace TabbyCat.Models.AiMediaResponses;

public interface IAiMediaTaskStateResponseModel
{
    string RequestId { get; set; }

    object? Output { get; set; }

    AiMediaRunningStatus TaskStatus { get; }

    IEnumerable<string>? DownloadUrls { get; }
}

public interface IAiMediaTaskStateResponseModel<TOutput, TData> : IAiMediaTaskStateResponseModel
    where TOutput : AiMediaOutputBase<TData>
{
    TOutput? Output { get; set; }

    object? IAiMediaTaskStateResponseModel.Output
    {
        get => Output;
        set => Output = (TOutput)value;
    }
}