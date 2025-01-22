using Newtonsoft.Json;

using System.Text;

using TabbyCat.Ai.Models;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;
using TabbyCat.Ai.Models.AiReqRes.AiChatResponses;
using TabbyCat.Ai.Services;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Ai.IServices;

public abstract class
    AiChatRequestServiceBase<TRequestModel, TAiModel, TResponseModel> : IAiResponse<TRequestModel, TAiModel,
    TResponseModel>
    where TRequestModel : MessageSessionBase
    where TAiModel : AiApiModelBase
    where TResponseModel : ChatResponseBase
{
    protected readonly TRequestModel _requestModel;
    protected readonly TAiModel _aiModel;

    protected AiChatRequestServiceBase(TRequestModel requestModel, TAiModel aiModel)
    {
        _requestModel = requestModel;
        _aiModel = aiModel;
    }

    protected abstract Task<UnityResponseModel> ConvertResponseToUnityResponseModel(TResponseModel response);

    protected virtual HttpRequestMessage BuildHttpRequestMessage(TAiModel aiModel)
    {
        if (aiModel is IApiDomain apiDomain)
        {
            var urlPath = apiDomain.ApiDomain;

            if (aiModel is IApiPath apiPath) urlPath = $"{apiDomain.ApiDomain}{apiPath.ApiPath}";
            var request = new HttpRequestMessage(HttpMethod.Post, urlPath);

            request.Headers.Add("Accept", "application/json");

            if (apiDomain is IApiKey apiKey)
                request.Headers.Add("Authorization", $"Bearer {apiKey.ApiKey}");

            return request;
        }

        throw new NotImplementedException();
    }

    protected virtual Task<string> RequestModelToJsonString(TRequestModel requestModel)
    {
        return Task.FromResult(JsonConvert.SerializeObject(requestModel));
    }

    public async Task<UnityResponseModel?> ProcessRequestAsync()
    {
        try
        {
            var requestMessage = BuildHttpRequestMessage(_aiModel);

            var json = await RequestModelToJsonString(_requestModel);

            var content = new StringContent(
                json,
                Encoding.UTF8, "application/json");

            requestMessage.Content = content;

            using var client = new HttpClient();
            var response = await client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode == false)
                return UnityResponseModel.Error("请求失败！");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<TResponseModel>(responseString);

            return responseModel == null ? null : await ConvertResponseToUnityResponseModel(responseModel);
        }
        catch (Exception e)
        {
            // todo记录到日志中
            return UnityResponseModel.Error(e.Message);
        }
    }
}