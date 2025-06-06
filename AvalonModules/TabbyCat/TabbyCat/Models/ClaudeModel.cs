using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Models.RequestModelList;
using TabbyCat.Shared.Enums;
using TuDog.Bootstrap;

namespace TabbyCat.Models;

public partial class ClaudeModel : AiApiHasModelsModelBase
{
    public override AiModelType Provider => AiModelType.Claude;

    public ClaudeModel()
    {
        ApiDomain = "https://api.anthropic.com";
    }

    private ILogger<ClaudeModel> _logger =
        TuDogApplication.ServiceProvider.GetRequiredService<ILogger<ClaudeModel>>();


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
        catch (Exception exception)
        {
            _logger.LogError(exception,"获得模型发生错误。");
            return [];
        }
    }
}