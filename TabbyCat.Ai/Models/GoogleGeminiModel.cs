using Newtonsoft.Json;

using TabbyCat.Ai.Models.RequestModelList;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Ai.Models;

public class GoogleGeminiModel : AiApiHasModelsModelBase
{
    public override AiModelType Provider { get; } = AiModelType.GoogleGemini;


    //https://generativelanguage.googleapis.com/v1beta/models
    public override string ApiDomain { get; set; } = "https://generativelanguage.googleapis.com";

    public override async Task<IEnumerable<string>> GetModelsAsync()
    {
        if (string.IsNullOrEmpty(ApiKey))
            return [];

        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(1);
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiDomain}/v1beta/models?key={ApiKey}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var modelString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(modelString))
                return [];

            var converts = JsonConvert.DeserializeObject<GoogleGeminiModelList>(modelString);
            if (converts == null)
                return [];
            return converts.Models.Select(x => x.Name.Split("/").Last());
        }
        catch (Exception)
        {
            // todo 记录到日志中
            return [];
        }
    }
}