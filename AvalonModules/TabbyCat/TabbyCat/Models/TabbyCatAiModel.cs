using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyResultModel;
using FantasyResultModel.Impls;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Extensions;
using TabbyCat.IServices;
using TabbyCat.Shared.Enums;
using TuDog.Bootstrap;
using TuDog.Extensions;

namespace TabbyCat.Models;

public partial class TabbyCatAiModel : OpenAiApiModel
{

   private readonly IRemoteServerService remoteServerService= TuDogApplication.ServiceProvider.GetRequiredService<IRemoteServerService>();

   public override AiModelType Provider => AiModelType.TabbyCatAi;

   public override string ApiPath { get; set; } = "/compatible-mode/v1/chat/completions";

   public TabbyCatAiModel()
   {

       ApiDomain = "https://dashscope.aliyuncs.com";
   }

   public override async Task<ResultBase<bool>> InitializeAsync()
   {
      var keyResult =  await remoteServerService.GetAiKeyAsync();
      if (keyResult.Ok)
      {
          ApiKey = keyResult.Data;
      }
      else
      {
          return new ErrorResultModel<bool>(keyResult.ErrorMsg);
      }
       Models.Reset(await GetModelsAsync());
       SelectedModel = Models.FirstOrDefault()??string.Empty;
       return new SuccessResultModel<bool>();
   }

   public override Task<IEnumerable<string>> GetModelsAsync()
   {
       return Task.FromResult<IEnumerable<string>>(["qwen-vl-max"]);
   }
}