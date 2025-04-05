using System.Net.Http;
using FantasyResultModel;
using FantasyResultModel.Impls;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Extensions;
using TabbyCat.Shared.Enums;
using TuDog.Bootstrap;
using TuDog.Extensions;

namespace TabbyCat.Models;

public class TabbyCatAiModel : OpenAiApiModel
{
    private IHttpClientFactory httpclientfactory = TuDogApplication.ServiceProvider.GetRequiredService<IHttpClientFactory>();

    private HttpClient httpclient;

   public override AiModelType Provider { get; } = AiModelType.TabbyCatAi;

   public override string ApiDomain { get; set; } = "https://api.openai-hk.com";

   public TabbyCatAiModel()
   {
       httpclient = httpclientfactory.CreateClient(ConstParameter.Auth);
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