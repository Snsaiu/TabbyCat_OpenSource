using TabbyCat.Models;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Factories;

public static class AiTemplateFactory
{
    public static async Task<AiApiModelBase> GetTemplateAsync(AiModelType type)
    {
        AiApiModelBase model = type switch
        {
            AiModelType.OpenAiApi => new OpenAiApiModel(),
            AiModelType.AzureOpenAiApi => new AzureOpenAiApiModel(),
            AiModelType.Claude => new ClaudeModel(),
            AiModelType.GoogleGemini => new GoogleGeminiModel(),
            AiModelType.Ollama => new OllamaModel(),
            AiModelType.Groq => new GroqModel(),
            AiModelType.ChatGLM => new ChatGlmModel(),
            AiModelType.DeepSeek => new DeepSeekModel(),
            AiModelType.Custom => new CompatibleOpenAiApiModel(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        if (model is IInitializeable init)
            await init.InitializeAsync();
        return model;
    }

    public static async Task<AiApiModelBase> GetTemplateAsync(AiModelType type,
        IEnumerable<AiTemplateSettingEntity> aiTemplates)
    {
        var first = aiTemplates.FirstOrDefault(x => x.Provider == type);
        if (first is null)
            return await GetTemplateAsync(type);
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
            AiModelType.DeepSeek => typeof(DeepSeekModel),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        var model = (AiApiModelBase)JsonConvert.DeserializeObject(first.Template, convertType)!;
        model.IsDefault = first.IsDefault;
        if (model is IInitializeable init) await init.InitializeAsync();
        return model;
    }

    public static async Task<AiApiModelBase> GetTemplateAsync(string customModelName,
        IEnumerable<AiTemplateSettingEntity> aiTemplates)
    {
        var custom =
            aiTemplates.FirstOrDefault(x => x.Provider == AiModelType.Custom && x.ModelName == customModelName);
        if (custom is null)
        {
            throw new NullReferenceException();
        }
        else
        {
            var model = JsonConvert.DeserializeObject<CompatibleOpenAiApiModel>(custom.Template) ??
                        throw new NullReferenceException();
            model.IsSaved = true;
            model.IsDefault = custom.IsDefault;
            if (model is IInitializeable init) await init.InitializeAsync();
            return model;
        }
    }
}