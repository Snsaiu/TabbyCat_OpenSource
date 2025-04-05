namespace TabbyCat.Models.RequestModelList;

public class OpenApiModelList
{
    [JsonProperty("object")] public string Type { get; set; } = string.Empty;

    [JsonProperty("data")] public List<OpenApiModelDataItem> Data { get; set; } = [];
}

public class OpenApiModelDataItem
{
    [JsonProperty("id")] public string Id { get; set; }= string.Empty;

    [JsonProperty("object")] public string Type { get; set; }= string.Empty;

    [JsonProperty("owned_by")] public string OwnedBy { get; set; } = string.Empty;

    [JsonProperty("created")] public string Created { get; set; }= string.Empty;
}