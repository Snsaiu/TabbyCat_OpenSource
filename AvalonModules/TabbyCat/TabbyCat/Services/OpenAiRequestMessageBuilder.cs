using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatRequests.OpenAi;
using TabbyCat.Models.AiReqRes.AiChatRequests.TabbyCatAi;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Services;

public sealed class OpenAiRequestMessageBuilder : AiRequesMessageBuilderBase
{
    public override MessageSessionBase Build(AiApiModelBase aiModel)
    {
        var message = new OpenAiRequestModel()
        {
            Temperature = aiModel.Temperature
        };

        if (aiModel is ITopP topP)
            message.TopP = topP.TopP;

        if (aiModel is IHasModels<string> hasModels)
            message.Model = hasModels.SelectedModel == "Custom"
                ? ((IHasCustomModel)aiModel).CustomModelName
                : hasModels.SelectedModel;
        else if (aiModel.Provider == AiModelType.OpenAiApi) message.Model = ((IDeployName)aiModel).DeployName;
        return message;
    }
}
