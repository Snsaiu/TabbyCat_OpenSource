using Newtonsoft.Json;

using TabbyCat.Ai.IServices;
using TabbyCat.Ai.Models;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests.GoogleGemini;
using TabbyCat.Ai.Models.AiReqRes.AiChatResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Ai.Services;

public sealed class GoogleGeminiRequestService(GoogleGeminiRequestModel requestModel, GoogleGeminiModel aiModel)
    : AiChatRequestServiceBase<GoogleGeminiRequestModel, GoogleGeminiModel, GoogleGeminiResponseModel>(requestModel,
        aiModel)
{

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

            requestModel.Content.Add(new GoogleGeminiRequestMessage
            {
                Role = role,
                Parts =
                [new GoogleRequestPart { Text = item.Content }]
            });
        }
        var json = JsonConvert.SerializeObject(requestModel);
        return Task.FromResult(json);
    }

    protected override HttpRequestMessage BuildHttpRequestMessage(GoogleGeminiModel aiModel)
    {
        if (aiModel is IApiDomain apiDomain)
        {
            var urlPath = apiDomain.ApiDomain;

            if (aiModel is IApiPath apiPath)
            {
                urlPath = $"{apiDomain.ApiDomain}{apiPath.ApiPath}";
            }
            if (aiModel is IApiKey apiKey)
            {
                urlPath = $"{urlPath}/v1beta/models/{aiModel.SelectedModel}:generateContent?key={apiKey.ApiKey}";
            }
            var request = new HttpRequestMessage(HttpMethod.Post, urlPath);

            return request;
        }
        throw new NotImplementedException();
    }
    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(GoogleGeminiResponseModel response)
    {
        var content = response.Candidates.FirstOrDefault()?.Content.Parts.LastOrDefault()?.Text;
        return Task.FromResult<UnityResponseModel>(UnityResponseModel.Success(content ?? string.Empty));
    }
}