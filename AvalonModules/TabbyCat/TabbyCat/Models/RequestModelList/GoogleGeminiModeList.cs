namespace TabbyCat.Models.RequestModelList;

public class GoogleGeminiModel
{
    [JsonProperty("name")] public required string Name { get; set; }

    [JsonProperty("version")] public required string Version { get; set; }

    [JsonProperty("displayName")] public required string DisplayName { get; set; }


    [JsonProperty("description")] public required string Description { get; set; }

    [JsonProperty("inputTokenLimit")] public int InputTokenLimit { get; set; }

    [JsonProperty("outputTokenLimit")] public int OutputTokenLimit { get; set; }

    [JsonProperty("supportedGenerationMethods")]
    public required List<string> SupportedGenerationMethods { get; set; }

    [JsonProperty("temperature")] public double Temperature { get; set; }

    [JsonProperty("topP")] public double TopP { get; set; }


    [JsonProperty("topK")] public int TopK { get; set; }


    [JsonProperty("maxTemperature")] public double MaxTemperature { get; set; }
}

public class GoogleGeminiModelList
{
    [JsonProperty("models")] public required List<GoogleGeminiModel> Models { get; set; }
}