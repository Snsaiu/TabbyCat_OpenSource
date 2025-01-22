using Newtonsoft.Json;

using TabbyCat.Ai.IServices;
using TabbyCat.Ai.Models;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;
using TabbyCat.Ai.Models.AiReqRes.AiChatResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Ai.Services;

public sealed class ClaudeRequestService(ClaudeRequestModel requestModel, ClaudeModel aiModel)
    : AiChatRequestServiceBase<ClaudeRequestModel, ClaudeModel, ClaudeResponseModel>(requestModel, aiModel)
{
    protected override HttpRequestMessage BuildHttpRequestMessage(ClaudeModel aiModel)
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
            return request;
        }
        throw new NotImplementedException();
    }

    protected override Task<string> RequestModelToJsonString(ClaudeRequestModel requestModel)
    {

        foreach (var item in requestModel.Messages)
        {

            if (item.Role == Role.System)
                item.Role = Role.Assistant;
        }
        var json = JsonConvert.SerializeObject(requestModel);
        return Task.FromResult(json);

    }

    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(ClaudeResponseModel response)
    {
        return Task.FromResult<UnityResponseModel>(UnityResponseModel.Success(response.Content.LastOrDefault()?.Text ?? string.Empty));
    }
}