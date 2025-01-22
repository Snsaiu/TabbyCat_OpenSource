using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Ai.Models;

public class GroqModel : AiApiKeyModelBase, IHasModels<string>
{
    public override AiModelType Provider { get; } = AiModelType.Groq;
    public string SelectedModel { get; set; } = string.Empty;

    public Task<IEnumerable<string>> GetModelsAsync()
    {
        return Task.FromResult<IEnumerable<string>>([]);
    }
}