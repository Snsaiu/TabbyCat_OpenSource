using Newtonsoft.Json;
using TabbyCat.Ai.Models;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Ai.Factories;

public static class AiTemplateFactory
{

    public static AiApiModelBase GetTemplate(AiModelType type)
    {
        return type switch
        {
            AiModelType.OpenAiApi => new OpenAiApiModel(),
            AiModelType.AzureOpenAiApi => new AzureOpenAiApiModel(),
            AiModelType.Claude => new ClaudeModel(),
            AiModelType.GoogleGemini => new GoogleGeminiModel(),
            AiModelType.Ollama => new OllamaModel(),
            AiModelType.Groq => new GroqModel(),
            AiModelType.ChatGLM => new ChatGlmModel(),
            AiModelType.Custom => new CompatibleOpenAiApiModel(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    public static AiApiModelBase GetTemplate(AiModelType type, IEnumerable<AiTemplateSettingEntity> aiTemplates)
    {
        var first = aiTemplates.FirstOrDefault(x => x.Provider == type);
        if (first is null)
            return GetTemplate(type);
        var convertType = type switch
        {
            AiModelType.OpenAiApi => typeof(OpenAiApiModel),
            AiModelType.AzureOpenAiApi => typeof(AzureOpenAiApiModel),
            AiModelType.Claude => typeof(ClaudeModel),
            AiModelType.GoogleGemini => typeof(GoogleGeminiModel),
            AiModelType.Ollama => typeof(OllamaModel),
            AiModelType.Groq => typeof(GroqModel),
            AiModelType.ChatGLM => typeof(ChatGlmModel),
            AiModelType.Custom => typeof(CompatibleOpenAiApiModel),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
        var model = (AiApiModelBase)JsonConvert.DeserializeObject(first.Template, convertType)!;
        model.IsDefault = first.IsDefault;
        return model;
    }

    public static AiApiModelBase GetTemplate(string customModelName, IEnumerable<AiTemplateSettingEntity> aiTemplates)
    {
        var custom = aiTemplates.FirstOrDefault(x => x.Provider == AiModelType.Custom && x.ModelName == customModelName);
        if (custom is null)
            throw new NullReferenceException();
        else
        {
            var model = JsonConvert.DeserializeObject<CompatibleOpenAiApiModel>(custom.Template) ?? throw new NullReferenceException();
            model.IsSaved = true;
            model.IsDefault = custom.IsDefault;
            return model;
        }
    }


}