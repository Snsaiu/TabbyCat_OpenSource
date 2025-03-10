namespace TabbyCat.Models.RunningHubs;

public class RunningHubOutputResponseModel
{
    [JsonProperty("fileUrl")] public string FileUrl { get; set; }

    [JsonProperty("fileType")] public string FileType { get; set; }
}