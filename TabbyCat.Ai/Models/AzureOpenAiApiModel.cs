using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Ai.Models;

public class AzureOpenAiApiModel : AiApiKeyModelBase, IHasModels<string>, IEndPoint, IDeployName
{
    public string EndPoint { get; set; } = string.Empty;
    public override AiModelType Provider => AiModelType.AzureOpenAiApi;
    public string SelectedModel { get; set; } = string.Empty;

    public string DeployName { get; set; } = string.Empty;

    public Task<IEnumerable<string>> GetModelsAsync()
    {
        return Task.FromResult<IEnumerable<string>>([]);
    }
}