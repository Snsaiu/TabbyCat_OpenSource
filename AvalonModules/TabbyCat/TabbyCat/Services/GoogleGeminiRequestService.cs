using System.Net.Http;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests.GoogleGemini;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Services;

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

    protected override HttpRequestMessage BuildHttpRequestMessage(GoogleGeminiModel aiModel)
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
            return request;
        }

        throw new NotImplementedException();
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