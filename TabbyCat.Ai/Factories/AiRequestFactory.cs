using TabbyCat.Ai.Models;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests.GoogleGemini;
using TabbyCat.Ai.Services;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Ai.Factories;

public static class AiRequestFactory
{
    public static IAiResponse CreateService(MessageSessionBase requestMessage, AiApiModelBase aiModel)
    {
        return aiModel.Provider switch
        {
            AiModelType.OpenAiApi => throw new NotImplementedException(),
            AiModelType.AzureOpenAiApi => throw new NotImplementedException(),
            AiModelType.Claude => new ClaudeRequestService(requestMessage as ClaudeRequestModel, aiModel as ClaudeModel),
            AiModelType.GoogleGemini => new GoogleGeminiRequestService(requestMessage as GoogleGeminiRequestModel, aiModel as GoogleGeminiModel),
            AiModelType.Ollama => new OllamaRequestService(requestMessage as OllamaRequestModel, aiModel as OllamaModel),
            AiModelType.Groq => throw new NotImplementedException(),
            AiModelType.ChatGLM => throw new NotImplementedException(),
            AiModelType.Custom => new CompatibleOpenAiRequestService(requestMessage as CompatibleRequestModel, aiModel as CompatibleOpenAiApiModel),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static MessageSessionBase CreateMessageSession(AiApiModelBase aiModel)
    {
        return aiModel.Provider switch
        {
            AiModelType.OpenAiApi => throw new NotImplementedException(),
            AiModelType.AzureOpenAiApi => throw new NotImplementedException(),
            AiModelType.Claude => new ClaudeRequestMessageBuilder().Build(aiModel),
            AiModelType.GoogleGemini => new GoogleGeminiRequestMessageBuilder().Build(aiModel),
            AiModelType.Ollama => new OllamaRequestMessageBuilder().Build(aiModel),
            AiModelType.Groq => throw new NotImplementedException(),
            AiModelType.ChatGLM => throw new NotImplementedException(),
            AiModelType.Custom => new CompatibleOpenAiRequestMessageBuilder().Build(aiModel),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}