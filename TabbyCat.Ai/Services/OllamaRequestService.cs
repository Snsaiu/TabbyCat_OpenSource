using TabbyCat.Ai.IServices;
using TabbyCat.Ai.Models;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;
using TabbyCat.Ai.Models.AiReqRes.AiChatResponses;

namespace TabbyCat.Ai.Services;

public sealed class OllamaRequestService(
    OllamaRequestModel requestModel,
    OllamaModel aiModel)
    : AiChatRequestServiceBase<OllamaRequestModel, OllamaModel, OllamaResponseModel>(requestModel, aiModel)
{
    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(OllamaResponseModel response)
    {
        return Task.FromResult<UnityResponseModel>(UnityResponseModel.Success(response.Message.Content ?? string.Empty));
    }
}