using System.Threading;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatResponses;

namespace TabbyCat.IServices;

public interface IAiResponse<TRequestModel, TAiModel, TResponseModel> : IAiResponse
    where TRequestModel : MessageSessionBase
    where TAiModel : AiApiModelBase
    where TResponseModel : ChatResponseBase
{
    new Task<UnityResponseModel?> ProcessRequestAsync();


    new Task StreamProcessResponseAsync(Func<UnityResponseModel, bool> action, CancellationToken cancellationToken);

    Task IAiResponse.StreamProcessResponseAsync(Func<object, bool> action, CancellationToken cancellationToken)
    {
        return StreamProcessResponseAsync(action, cancellationToken);
    }


    Task<UnityResponseModel?> IAiResponse.ProcessRequestAsync()
    {
        return ProcessRequestAsync();
    }
}

public interface IAiResponse
{
    Task<UnityResponseModel?> ProcessRequestAsync();

    /// <summary>
    /// 采用sse的方式获得数据
    /// </summary>
    /// <param name="callBack">如何返回值是true,表示取消，否则继续</param>
    /// <returns></returns>
    Task StreamProcessResponseAsync(Func<object, bool> callBack, CancellationToken cancellationToken);
}