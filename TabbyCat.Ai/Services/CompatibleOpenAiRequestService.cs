using TabbyCat.Ai.IServices;
using TabbyCat.Ai.Models;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;
using TabbyCat.Ai.Models.AiReqRes.AiChatResponses;

namespace TabbyCat.Ai.Services;

public sealed class CompatibleOpenAiRequestService(
    CompatibleRequestModel requestModel,
    CompatibleOpenAiApiModel aiModel)
    : AiChatRequestServiceBase<CompatibleRequestModel, CompatibleOpenAiApiModel, CustomResponseModel>(requestModel, aiModel)
{
    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(CustomResponseModel response)
    {
        return Task.FromResult(UnityResponseModel.Success(response.Choices.FirstOrDefault()?.Message.Content ?? string.Empty));
    }
}