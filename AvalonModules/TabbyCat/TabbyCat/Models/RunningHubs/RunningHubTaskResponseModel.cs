namespace TabbyCat.Models.RunningHubs;

public class RunningHubTaskResponseModel
{

    [JsonProperty("taskId")]
    public string TaskId { get; set; }

    [JsonProperty("clientId")]
    public string ClientId { get; set; }

    [JsonProperty("taskStatus")]
    public string TaskStatus { get; set; }
 
    [JsonProperty("promptTips")]
    public string PromptTips { get; set; }
    
}