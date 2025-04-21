using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyResultModel;
using FantasyResultModel.Impls;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Extensions;
using TabbyCat.Shared.Enums;
using TuDog.Bootstrap;
using TuDog.Extensions;

namespace TabbyCat.Models;

public partial class TabbyCatAiModel : OpenAiApiModel
{
    private IHttpClientFactory httpclientfactory = TuDogApplication.ServiceProvider.GetRequiredService<IHttpClientFactory>();

    private HttpClient httpclient;

   public override AiModelType Provider => AiModelType.TabbyCatAi;

   public TabbyCatAiModel()
   {
       httpclient = httpclientfactory.CreateClient(ConstParameter.Auth);
       ApiDomain = "https://api.openai-hk.com";
   }

   public override async Task<ResultBase<bool>> InitializeAsync()
   {
       var queryKeyResult = await httpclient.GetAsync("https://api.yyan.cc/api/app/ai-api-key/api-key");
       if (!queryKeyResult.IsSuccessStatusCode)
           return new ErrorResultModel<bool>(queryKeyResult.ReasonPhrase ?? "query api key error!");

       ApiKey = await queryKeyResult.Content.ReadAsStringAsync();
       Models.Reset(await GetModelsAsync());
       SelectedModel = Models.FirstOrDefault()??string.Empty;
       return new SuccessResultModel<bool>();
   }

   public override Task<IEnumerable<string>> GetModelsAsync()
   {
       return Task.FromResult<IEnumerable<string>>(["gpt-3.5-turbo"]);
   }
}