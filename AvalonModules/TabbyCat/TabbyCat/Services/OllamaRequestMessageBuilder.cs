using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Services;

public sealed class OllamaRequestMessageBuilder : AiRequesMessageBuilderBase
{
    public override MessageSessionBase Build(AiApiModelBase aiModel)
    {
        var message = new OllamaRequestModel();

        if (aiModel is IHasModels<string> hasModels)
            message.Model = hasModels.SelectedModel == "Custom"
                ? ((IHasCustomModel)aiModel).CustomModelName
                : hasModels.SelectedModel;
        return message;
    }
}