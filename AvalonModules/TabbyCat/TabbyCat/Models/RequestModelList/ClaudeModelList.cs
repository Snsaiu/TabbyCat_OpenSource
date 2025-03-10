namespace TabbyCat.Models.RequestModelList;

public class ClaudeModelList
{
    [JsonProperty("data")] public List<ClaudeModel> Data { get; set; } = [];

    [JsonProperty("has_more")] public string HasMore { get; set; } = string.Empty;

    [JsonProperty("first_id")] public string FirstId { get; set; } = string.Empty;

    [JsonProperty("last_id")] public string LastId { get; set; } = string.Empty;
}

public class ClaudeModel
{
    [JsonProperty("type")] public string Type { get; set; } = string.Empty;

    [JsonProperty("id")] public string Id { get; set; } = string.Empty;

    [JsonProperty("display_name")] public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("created_at")] public string CreatedAt { get; set; } = string.Empty;
}