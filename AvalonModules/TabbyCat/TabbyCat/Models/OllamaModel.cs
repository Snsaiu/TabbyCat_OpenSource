using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Models;

public class OllamaModel : AiApiModelBase, IApiDomain, IHasModels<string>, IApiPath
{
    public override AiModelType Provider { get; } = AiModelType.Ollama;
    public string ApiDomain { get; set; } = "http://127.0.0.1:11434";
    public string SelectedModel { get; set; } = string.Empty;

    public string ApiPath { get; set; } = string.Empty;

    public async Task<IEnumerable<string>> GetModelsAsync()
    {
        var models = new List<string>();

        if (string.IsNullOrEmpty(ApiDomain))
            return models;
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(1);
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiDomain}/api/tags");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var modelString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(modelString))
                return models;

            var converts = JsonConvert.DeserializeObject<OllamaModels>(modelString);
            if (converts == null)
                return models;

            models.AddRange(converts.models.Select(item => item.name));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // todo写log
        }

        return models;
    }

    public ObservableCollection<string> Models { get; set; } = [];
}

file class ModelsItem
{
    public string name { get; set; }
    public string model { get; set; }
    public DateTime modified_at { get; set; }
    public long size { get; set; }
    public string digest { get; set; }
}

file class OllamaModels
{
    /// <summary>
    ///
    /// </summary>
    public List<ModelsItem> models { get; set; }
}