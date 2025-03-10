using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TabbyCat.Models.RequestModelList;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Models;

public class ClaudeModel : AiApiHasModelsModelBase
{
    public override AiModelType Provider { get; } = AiModelType.Claude;

    public override string ApiDomain { get; set; } = "https://api.anthropic.com";


    public override async Task<IEnumerable<string>> GetModelsAsync()
    {
        if (string.IsNullOrEmpty(ApiKey))
            return [];

        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiDomain}/v1/models");
            request.Headers.Add("x-api-key", $"{ApiKey}");
            request.Headers.Add("anthropic-version", $"2023-06-01");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var modelString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(modelString))
                return [];

            var converts = JsonConvert.DeserializeObject<ClaudeModelList>(modelString);
            if (converts == null)
                return [];
            return converts.Data.Select(x => x.Id);
        }
        catch (Exception)
        {
            // todo 记录到日志中
            return [];
        }
    }
}