using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests.DeepSeek;
using TabbyCat.Models.AiReqRes.AiChatResponses;

namespace TabbyCat.Services;

public sealed class DeepSeekModelRequestService(DeepSeekRequestModel requestModel, DeepSeekModel aiModel)
    : AiChatRequestServiceBase<DeepSeekRequestModel, DeepSeekModel, DeepSeekResponseModel>(requestModel, aiModel)
{
    protected override string PreProcessResponse(string responseString)
    {
        return responseString.Replace("data: ", "");
    }

    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(DeepSeekResponseModel response)
    {
        return Task.FromResult(
            UnityResponseModel.StreamData(response.Choices.FirstOrDefault()?.delta?.Content ?? string.Empty));
    }
}