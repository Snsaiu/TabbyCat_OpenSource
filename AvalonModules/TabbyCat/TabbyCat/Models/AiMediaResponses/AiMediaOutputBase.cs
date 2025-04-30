using TabbyCat.Enums;
using TabbyCat.Extensions;

namespace TabbyCat.Models.AiMediaResponses;

public abstract class AiMediaOutputBase<T>
{
    [JsonProperty("task_id")] public string TaskId { get; set; }

    [JsonConverter(typeof(AiMediaRunningStatusJsonConverter))]
    [JsonProperty("task_status")]
    public AiMediaRunningStatus TaskStatus { get; set; }

    [JsonProperty("submit_time")] public DateTime SubmitTime { get; set; }

    [JsonProperty("scheduled_time")] public DateTime ScheduledTime { get; set; }

    [JsonProperty("end_time")] public DateTime EndTime { get; set; }

    public abstract T Data { get; set; }
}