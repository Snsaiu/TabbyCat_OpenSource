using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatResponses;

namespace TabbyCat.Services;

public sealed class CompatibleOpenAiRequestService(
    CompatibleRequestModel requestModel,
    CompatibleOpenAiApiModel aiModel)
    : AiChatRequestServiceBase<CompatibleRequestModel, CompatibleOpenAiApiModel, CustomResponseModel>(requestModel,
        aiModel)
{
    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(CustomResponseModel response)
    {
        return Task.FromResult(
            UnityResponseModel.Success(response.Choices.FirstOrDefault()?.Message?.Content ?? string.Empty));
    }
}