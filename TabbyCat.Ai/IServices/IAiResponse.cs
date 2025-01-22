using TabbyCat.Ai.Models;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;
using TabbyCat.Ai.Models.AiReqRes.AiChatResponses;

namespace TabbyCat.Ai.Services;

public interface IAiResponse<TRequestModel, TAiModel, TResponseModel> : IAiResponse
    where TRequestModel : MessageSessionBase
    where TAiModel : AiApiModelBase
    where TResponseModel : ChatResponseBase
{
    Task<UnityResponseModel?> ProcessRequestAsync();
    Task<UnityResponseModel?> IAiResponse.ProcessRequestAsync() => ProcessRequestAsync();
}

public interface IAiResponse
{
    Task<UnityResponseModel?> ProcessRequestAsync();
}