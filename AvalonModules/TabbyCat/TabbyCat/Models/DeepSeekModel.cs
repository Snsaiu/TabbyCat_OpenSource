using System.Net.Http;
using TabbyCat.Models.RequestModelList;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Models;

public class DeepSeekModel : OpenAiApiModel
{
    public override AiModelType Provider => AiModelType.DeepSeek;
    public override string ApiDomain { get; set; } = "https://api.deepseek.com/chat/completions";

    public override async Task<IEnumerable<string>> GetModelsAsync()
    {
        if (string.IsNullOrEmpty(ApiKey))
            return [];
        try
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.deepseek.com/models");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {ApiKey}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var models = JsonConvert.DeserializeObject<DeepSeekModelList>(content);
            if (models is null)
                return [];
            return models.Data.Select(x => x.Id);
        }
        catch
        {
            return [];
        }
    }
}