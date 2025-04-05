namespace TabbyCat.Models.RunningHubs;

public class RunningHubOutputResponseModel
{
    [JsonProperty("fileUrl")] public string FileUrl { get; set; } = string.Empty;

    [JsonProperty("fileType")] public string FileType { get; set; } = string.Empty;
}