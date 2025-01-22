using TabbyCat.Ai.Models;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;

namespace TabbyCat.Ai.IServices;

public abstract class AiRequesMessageBuilderBase
{
    public abstract MessageSessionBase Build(AiApiModelBase aiModel);
}