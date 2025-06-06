using System.Net.Http;
using FantasyResultModel;
using FantasyResultModel.Impls;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests.GoogleGemini;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TabbyCat.Shared.Languages;

namespace TabbyCat.Services;

public sealed class GoogleGeminiRequestService : AiChatRequestServiceBase<GoogleGeminiRequestModel, GoogleGeminiModel, GoogleGeminiResponseModel>
{
    
    private ILogger logger;

    public GoogleGeminiRequestService(GoogleGeminiRequestModel requestModel, GoogleGeminiModel aiModel) : base(requestModel,
        aiModel)
    {
        logger = LoggerFactory.CreateLogger(nameof(ClaudeRequestService));
    }

    protected override Task<string> RequestModelToJsonString(GoogleGeminiRequestModel requestModel)
    {
        foreach (var item in requestModel.Messages)
        {
            var role = item.Role switch
            {
                Role.System => Role.User,
                Role.Assistant => Role.Model,
                _ => Role.User
            };

            requestModel.Content.Add(new()
            {
                Role = role,
                Parts =
                    [new GoogleRequestPart { Text = item.Content }]
            });
        }

        var json = JsonConvert.SerializeObject(requestModel);
        return Task.FromResult(json);
    }

    protected override ResultBase< HttpRequestMessage> BuildHttpRequestMessage(GoogleGeminiModel aiModel)
    {
        if (aiModel is IApiDomain apiDomain)
        {
            var urlPath = apiDomain.ApiDomain;

            if (aiModel is IApiPath apiPath) urlPath = $"{apiDomain.ApiDomain}{apiPath.ApiPath}";
            if (aiModel is IApiKey apiKey)
                urlPath =
                    $"{urlPath}/v1beta/models/{aiModel.SelectedModel}:streamGenerateContent?alt=sse&key={apiKey.ApiKey}";
            var request = new HttpRequestMessage(HttpMethod.Post, urlPath);
            // request.Headers.Add("Accept", "text/event-stream");
            request.Headers.Add("Accept", "application/json");
            return new SuccessResultModel<HttpRequestMessage>(request);
        }

        logger.LogError("暂不支持提供者{0}不是{1}类型的AiModel。",aiModel.Provider.ToString(),typeof(IApiDomain));

        return new ErrorResultModel<HttpRequestMessage>(string.Format(AppResources.NotSupport,aiModel.Provider));
    }

    protected override string PreProcessResponse(string responseString)
    {
        return responseString.Replace("data: ", "");
    }

    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(GoogleGeminiResponseModel response)
    {
        var content = response.Candidates.FirstOrDefault()?.Content.Parts.LastOrDefault()?.Text;
        return Task.FromResult<UnityResponseModel>(UnityResponseModel.StreamData(content ?? string.Empty));
    }
}