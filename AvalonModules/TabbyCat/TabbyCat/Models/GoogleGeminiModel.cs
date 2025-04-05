using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Models.RequestModelList;
using TabbyCat.Shared.Enums;
using TuDog.Bootstrap;

namespace TabbyCat.Models;

public class GoogleGeminiModel : AiApiHasModelsModelBase
{
    public override AiModelType Provider { get; } = AiModelType.GoogleGemini;


    //https://generativelanguage.googleapis.com/v1beta/models
    public override string ApiDomain { get; set; } = "https://generativelanguage.googleapis.com";
    
    private ILogger<GoogleGeminiModel> _logger=TuDogApplication.ServiceProvider.GetRequiredService<ILogger<GoogleGeminiModel>>();

    public override async Task<IEnumerable<string>> GetModelsAsync()
    {
        if (string.IsNullOrEmpty(ApiKey))
            return [];

        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(20);
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
        catch (Exception exception)
        {
            _logger.LogError(exception,"获得谷歌模型发生错误");
            return [];
        }
    }
}