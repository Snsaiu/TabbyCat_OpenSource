using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatRequests.DeepSeek;
using TabbyCat.Models.AiReqRes.AiChatRequests.GoogleGemini;
using TabbyCat.Models.AiReqRes.AiChatRequests.OpenAi;
using TabbyCat.Models.AiReqRes.AiChatRequests.TabbyCatAi;
using TabbyCat.Services;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Factories;

public static class AiRequestFactory
{
    public static IAiResponse CreateService(MessageSessionBase requestMessage, AiApiModelBase aiModel)
    {
        return aiModel.Provider switch
        {
            AiModelType.TabbyCatAi => new TabbyCatAiModelRequestService(requestMessage as TabbyCatAiRequestModel,
                aiModel as TabbyCatAiModel),
            AiModelType.OpenAiApi => new OpenAiModelRequestService(requestMessage as OpenAiRequestModel,
                aiModel as OpenAiApiModel),
            AiModelType.AzureOpenAiApi => throw new NotImplementedException(),
            AiModelType.Claude =>
                new ClaudeRequestService(requestMessage as ClaudeRequestModel, aiModel as ClaudeModel),
            AiModelType.GoogleGemini => new GoogleGeminiRequestService(requestMessage as GoogleGeminiRequestModel,
                aiModel as GoogleGeminiModel),
            AiModelType.Ollama =>
                new OllamaRequestService(requestMessage as OllamaRequestModel, aiModel as OllamaModel),
            AiModelType.Groq => throw new NotImplementedException(),
            AiModelType.ChatGLM => throw new NotImplementedException(),
            AiModelType.Custom => new CompatibleOpenAiRequestService(requestMessage as CompatibleRequestModel,
                aiModel as CompatibleOpenAiApiModel),
            AiModelType.DeepSeek => new DeepSeekModelRequestService(requestMessage as DeepSeekRequestModel,
                aiModel as DeepSeekModel),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static MessageSessionBase CreateMessageSession(AiApiModelBase aiModel)
    {
        return aiModel.Provider switch
        {
            AiModelType.TabbyCatAi => new TabbyCatAiRequestMessageBuilder().Build(aiModel),
            AiModelType.OpenAiApi => new OpenAiRequestMessageBuilder().Build(aiModel),
            AiModelType.AzureOpenAiApi => throw new NotImplementedException(),
            AiModelType.Claude => new ClaudeRequestMessageBuilder().Build(aiModel),
            AiModelType.GoogleGemini => new GoogleGeminiRequestMessageBuilder().Build(aiModel),
            AiModelType.Ollama => new OllamaRequestMessageBuilder().Build(aiModel),
            AiModelType.Groq => throw new NotImplementedException(),
            AiModelType.ChatGLM => throw new NotImplementedException(),
            AiModelType.DeepSeek => new DeepSeekRequestMessageBuilder().Build(aiModel),
            AiModelType.Custom => new CompatibleOpenAiRequestMessageBuilder().Build(aiModel),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}