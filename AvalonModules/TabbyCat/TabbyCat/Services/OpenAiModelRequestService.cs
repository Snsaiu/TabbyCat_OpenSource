using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests.OpenAi;
using TabbyCat.Models.AiReqRes.AiChatResponses;

namespace TabbyCat.Services;

public sealed class OpenAiModelRequestService(OpenAiRequestModel requestModel, OpenAiApiModel aiModel)
    : AiChatRequestServiceBase<OpenAiRequestModel, OpenAiApiModel, OpenAiResponseModel>(requestModel, aiModel)
{
    protected override string PreProcessResponse(string responseString)
    {
        return responseString.Replace("data: ", "");
    }

    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(OpenAiResponseModel response)
    {
        return Task.FromResult(
            UnityResponseModel.StreamData(response.Choices.FirstOrDefault()?.Message?.Content ?? string.Empty));
    }
}