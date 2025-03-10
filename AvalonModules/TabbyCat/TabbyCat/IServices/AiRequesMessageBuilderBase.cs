using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;

namespace TabbyCat.IServices;

public abstract class AiRequesMessageBuilderBase
{
    public abstract MessageSessionBase Build(AiApiModelBase aiModel);
}