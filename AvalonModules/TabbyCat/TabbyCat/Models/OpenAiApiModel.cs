using System.Collections.ObjectModel;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyResultModel;
using FantasyResultModel.Impls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Models.RequestModelList;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TuDog.Bootstrap;
using TuDog.Extensions;

namespace TabbyCat.Models;

/// <summary>
/// open ai 专属模板
/// </summary>
public partial class OpenAiApiModel : AiApiDomainModelBase, IHasCustomModel, ITopP, IInitializeable, IApiPath
{
    private ILogger<OpenAiApiModel> _logger =
        TuDogApplication.ServiceProvider.GetRequiredService<ILogger<OpenAiApiModel>>();

    public override AiModelType Provider => AiModelType.OpenAiApi;

    [ObservableProperty] private string _selectedModel = string.Empty;
    [ObservableProperty] private string _customModelName = string.Empty;
    [ObservableProperty] private double _topP = 0.1;

    public OpenAiApiModel()
    {
        ApiDomain = "https://api.openai.com";
    }

    public virtual async Task<IEnumerable<string>> GetModelsAsync()
    {
        if (string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(ApiDomain))
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
            return models is null ? [] : models.Data.Select(x => x.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "获得OpenAi的模型发生错误。");
            return [];
        }
    }

    public ObservableCollection<string> Models { get; set; } = [];

    public virtual async Task<ResultBase<bool>> InitializeAsync()
    {
        var models = await GetModelsAsync();
        if (!models.Any())
            return new ErrorResultModel<bool>("No models found");
        Models.Reset(models);
        return new SuccessResultModel<bool>();
    }

    public virtual string ApiPath { get; set; } = "/v1/chat/completions";
}