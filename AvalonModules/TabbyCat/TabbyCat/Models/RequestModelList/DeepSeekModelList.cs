namespace TabbyCat.Models.RequestModelList;

public class DeepSeekDataItem
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("object")] public string Type { get; set; }

    [JsonProperty("owned_by")] public string OwnedBy { get; set; }
}

public class DeepSeekModelList
{
    [JsonProperty("object")] public string Type { get; set; } = string.Empty;

    [JsonProperty("data")] public List<DeepSeekDataItem> Data { get; set; } = [];
}