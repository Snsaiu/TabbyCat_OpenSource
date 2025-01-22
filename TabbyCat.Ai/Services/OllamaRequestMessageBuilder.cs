using TabbyCat.Ai.IServices;
using TabbyCat.Ai.Models;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Ai.Services;

public sealed class OllamaRequestMessageBuilder : AiRequesMessageBuilderBase
{
    public override MessageSessionBase Build(AiApiModelBase aiModel)
    {
        var message = new OllamaRequestModel();

        if (aiModel is IHasModels<string> hasModels)
        {
            message.Model = hasModels.SelectedModel == "Custom" ? ((IHasCustomModel)aiModel).CustomModelName : hasModels.SelectedModel;
        }
        return message;
    }
}