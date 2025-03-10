namespace TabbyCat.Models.RunningHubs;

public class RunningHubUserStateResponseModel
{
    [JsonProperty("remainCoins")]
    public string RemainCoins { get; set; }
    
    [JsonProperty("currentTaskCounts")]
    public int CurrentTaskCounts { get; set; }
}