using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests.OpenAi;
using TabbyCat.Models.AiReqRes.AiChatRequests.TabbyCatAi;
using TabbyCat.Models.AiReqRes.AiChatResponses;

namespace TabbyCat.Services;

public sealed class TabbyCatAiModelRequestService(TabbyCatAiRequestModel requestModel, TabbyCatAiModel aiModel)
    : AiChatRequestServiceBase<TabbyCatAiRequestModel, TabbyCatAiModel, TabbyCatAiResponseModel>(requestModel, aiModel)
{
    protected override string PreProcessResponse(string responseString)
    {
        return responseString.Replace("data: ", "");
    }

    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(TabbyCatAiResponseModel response)
    {
        return Task.FromResult(
            UnityResponseModel.StreamData(response.Choices.FirstOrDefault()?.Message.Content ?? string.Empty));
    }
}