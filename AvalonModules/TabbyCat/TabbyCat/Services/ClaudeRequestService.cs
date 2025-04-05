using System.Net.Http;
using FantasyResultModel;
using FantasyResultModel.Impls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TabbyCat.Shared.Languages;
using TuDog.Bootstrap;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TabbyCat.Services;

public sealed class ClaudeRequestService : AiChatRequestServiceBase<ClaudeRequestModel, ClaudeModel, ClaudeResponseModel>
{
    private ILogger logger;
    
    public ClaudeRequestService(ClaudeRequestModel requestModel, ClaudeModel aiModel) : base(requestModel, aiModel)
    {
       logger = LoggerFactory.CreateLogger(nameof(ClaudeRequestService));
    }

    protected override ResultBase< HttpRequestMessage> BuildHttpRequestMessage(ClaudeModel aiModel)
    {
        if (aiModel is IApiDomain apiDomain)
        {
            var urlPath = apiDomain.ApiDomain;
            urlPath = $"{urlPath}/v1/messages";
            var request = new HttpRequestMessage(HttpMethod.Post, urlPath);

            if (aiModel is IApiKey apiKey)
            {
                request.Headers.Add("x-api-key", apiKey.ApiKey);
                request.Headers.Add("anthropic-version", "2023-06-01");
            }

            return new SuccessResultModel<HttpRequestMessage>(request);
        }


        logger.LogError("暂不支持提供者{0}不是{1}类型的AiModel。",aiModel.Provider.ToString(),typeof(IApiDomain));

        return new ErrorResultModel<HttpRequestMessage>(string.Format(AppResources.NotSupport,aiModel.Provider));
    }

    protected override Task<string> RequestModelToJsonString(ClaudeRequestModel requestModel)
    {
        foreach (var item in requestModel.Messages)
            if (item.Role == Role.System)
                item.Role = Role.Assistant;
        var json = JsonConvert.SerializeObject(requestModel);
        return Task.FromResult(json);
    }

    protected override string PreProcessResponse(string responseString)
    {
        return responseString.Replace("data: ", "");
    }

    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(ClaudeResponseModel response)
    {
        return Task.FromResult<UnityResponseModel>(
            UnityResponseModel.StreamData(response.Content.LastOrDefault()?.Text ?? string.Empty));
    }
}