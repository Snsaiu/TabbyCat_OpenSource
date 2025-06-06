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
            AiModelType.OpenAiApi => new OpenAiModelRequestService((OpenAiRequestModel)requestMessage ,
               (OpenAiApiModel) aiModel ),
            AiModelType.AzureOpenAiApi => throw new NotImplementedException(),
            AiModelType.Claude =>
                new ClaudeRequestService((ClaudeRequestModel)requestMessage , (ClaudeModel)aiModel ),
            AiModelType.GoogleGemini => new GoogleGeminiRequestService((GoogleGeminiRequestModel)requestMessage ,
               (GoogleGeminiModel)aiModel),
            AiModelType.Ollama =>
                new OllamaRequestService((OllamaRequestModel)requestMessage , (OllamaModel)aiModel ),
            AiModelType.Groq => throw new NotImplementedException(),
            AiModelType.ChatGLM => throw new NotImplementedException(),
            AiModelType.Custom => new CompatibleOpenAiRequestService((CompatibleRequestModel)requestMessage ,
                (CompatibleOpenAiApiModel)aiModel ),
            AiModelType.DeepSeek => new DeepSeekModelRequestService((DeepSeekRequestModel)requestMessage ,
                (DeepSeekModel)aiModel ),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static MessageSessionBase CreateMessageSession(AiApiModelBase aiModel)
    {
        return aiModel.Provider switch
        {
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