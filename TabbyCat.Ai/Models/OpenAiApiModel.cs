using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Ai.Models;

/// <summary>
/// open ai 专属模板
/// </summary>
public class OpenAiApiModel : AiApiDomainModelBase, IHasCustomModel, ITopP
{
    public string SelectedModel { get; set; } = string.Empty;

    public string? CustomModelName { get; set; }

    public double TopP { get; set; }
    public override AiModelType Provider => AiModelType.OpenAiApi;
    public override string ApiDomain { get; set; } = "https://api.openai.com";

    public Task<IEnumerable<string>> GetModelsAsync()
    {
        return Task.FromResult<IEnumerable<string>>([]);
    }
}