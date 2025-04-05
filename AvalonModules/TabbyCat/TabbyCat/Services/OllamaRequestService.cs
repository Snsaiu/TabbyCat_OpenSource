using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatResponses;

namespace TabbyCat.Services;

public sealed class OllamaRequestService(
    OllamaRequestModel requestModel,
    OllamaModel aiModel)
    : AiChatRequestServiceBase<OllamaRequestModel, OllamaModel, OllamaResponseModel>(requestModel, aiModel)
{

    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(OllamaResponseModel response)
    {
        return Task.FromResult<UnityResponseModel>(
            UnityResponseModel.StreamData(response.Message?.Content ?? string.Empty));
    }
}