namespace TabbyCat.Models.RunningHubs;

public class RunningHubTaskResponseModel
{

    [JsonProperty("taskId")]
    public string TaskId { get; set; }= string.Empty;

    [JsonProperty("clientId")] public string ClientId { get; set; } = string.Empty;

    [JsonProperty("taskStatus")]
    public string TaskStatus { get; set; }= string.Empty;
 
    [JsonProperty("promptTips")]
    public string PromptTips { get; set; }= string.Empty;
    
}