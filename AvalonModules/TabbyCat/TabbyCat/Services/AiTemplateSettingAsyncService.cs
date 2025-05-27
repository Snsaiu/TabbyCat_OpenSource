using System.Net.Http;
using FantasyResultModel;
using FantasyResultModel.Impls;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Extensions;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TuDog.Extensions;
using TuDog.IocAttribute;
using YouYan.Hamster.ResultModels;

namespace TabbyCat.Services;

[Register<IAiTemplateSettingSyncService>(ServiceLifetime.Singleton)]
public class AiTemplateSettingAsyncService:IAiTemplateSettingSyncService
{
    private HttpClient _httpClient;
    
    public AiTemplateSettingAsyncService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ConstParameter.Auth);
    }
    
    public async Task<IResultModel< IEnumerable<AiTemplateSettingEntity>>> SyncRemoteAiTemplateSettingEntitiesAsync(DownloadSettingDto downloadSettingDto)
    {
        
        var json = JsonConvert.SerializeObject(downloadSettingDto);
        var content = new StringContent(json);
        content.Headers.ContentType = new("application/json");
        var response = await _httpClient.PostAsync("/api/app/tabby-cat-ai-template-setting/download-ai-template-setting", content);
        response.EnsureSuccessStatusCode();
        return !response.IsSuccessStatusCode
            ?  ResultModelFactory.Error<IEnumerable<AiTemplateSettingEntity>>(response.ReasonPhrase)
            : ResultModelFactory.Success<IEnumerable<AiTemplateSettingEntity>>( JsonConvert.DeserializeObject<IEnumerable<AiTemplateSettingEntity>>(await response.Content.ReadAsStringAsync()));
    }

    public async Task<IResultModel<int>> QueryLatestVersionAsync(string email)
    {
       var queryResult = await  _httpClient.GetAsync(
            $"/api/app/tabby-cat-ai-template-setting/query-latest-ai-template-setting-version-setting?email={email}");
       if (queryResult.IsSuccessStatusCode)
       {
           var content = await queryResult.Content.ReadAsStringAsync();

           if (string.IsNullOrEmpty(content))
               return ResultModelFactory.Success<int>(0);
           
           return ResultModelFactory.Success<int>( int.Parse(content));
       }
       else
       {
           return ResultModelFactory.Error<int>(queryResult.ReasonPhrase??string.Empty);
       }
       
    }

    public async Task UploadNewVersionAsync(string email, IEnumerable<AiTemplateSettingEntity> settings)
    {
        var setting = new UploadSettingsDto()
        {
            Email = email,
            Settings = settings
        };
       await  _httpClient.PostRequestAsync<UploadSettingsDto>(
            "/api/app/tabby-cat-ai-template-setting/upload-ai-template-setting", setting);
    }
}