namespace TabbyCat.Models.RunningHubs;
public class UploadImageResponseModel
{
    [JsonProperty("fileName")]
    public string FileName { get; set; }= string.Empty;

    [JsonProperty("fileType")]
    public string FileType { get; set; } = string.Empty;
}
