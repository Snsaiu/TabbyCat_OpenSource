using TabbyCat.Enums;
using TabbyCat.Extensions;

namespace TabbyCat.Models.AiMediaResponses;


public class CreateAiMediaTaskResponseModel
{

    [JsonProperty("output")] public ResponseOutput Output { get; set; } = new();

    [JsonProperty("request_id")]
    public string RequestId { get; set; } = string.Empty;


    public class ResponseOutput
    {
        [JsonConverter(typeof(AiMediaRunningStatusJsonConverter))]
        [JsonProperty("task_status")]
        public AiMediaRunningStatus TaskStatus { get; set; }


        [JsonProperty("task_id")]
        public string TaskId { get; set; } = string.Empty;
    }
}