using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TabbyCat.Models.RequestModelList;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TuDog.Extensions;

namespace TabbyCat.Models;

/// <summary>
/// open ai 专属模板
/// </summary>
public class OpenAiApiModel : AiApiDomainModelBase, IHasCustomModel, ITopP, IInitializeable,IApiPath
{
    public string SelectedModel { get; set; } = string.Empty;

    public string? CustomModelName { get; set; }

    public double TopP { get; set; }
    public override AiModelType Provider => AiModelType.OpenAiApi;

    public override string ApiDomain { get; set; } =
        "https://api.openai.com"; // "https://api.openai.com";

    public virtual async Task<IEnumerable<string>> GetModelsAsync()
    {
        if (string.IsNullOrEmpty(ApiKey))
            return [];
        if (string.IsNullOrEmpty(ApiDomain))
            return [];
        try
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiDomain}/v1/models");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {ApiKey}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var models = JsonConvert.DeserializeObject<OpenApiModelList>(content);
            if (models is null)
                return [];
            return models.Data.Select(x => x.Id);
        }
        catch (Exception e)
        {
            return [];
        }
    }
    public ObservableCollection<string> Models { get; set; } = [];

    public async Task InitializeAsync()
    {
        Models.Reset(await GetModelsAsync());
    }

    public virtual string ApiPath { get; set; }="/v1/chat/completions";
}