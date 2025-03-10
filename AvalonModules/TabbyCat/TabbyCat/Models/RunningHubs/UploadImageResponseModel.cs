namespace TabbyCat.Models.RunningHubs;
public class UploadImageResponseModel
{
    [JsonProperty("fileName")]
    public string FileName { get; set; }

    [JsonProperty("fileType")]
    public string FileType { get; set; }
}
