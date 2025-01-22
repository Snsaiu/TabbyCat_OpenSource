using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Ai.Models;

public abstract class AiApiModelBase
{
    // 模型提供方
    public abstract AiModelType Provider { get; }

    public int ContextCount { get; set; }

    public bool ContextCountLimit { get; set; }

    public double Temperature { get; set; }

    public bool IsDefault { get; set; }
}

public abstract class AiApiKeyModelBase : AiApiModelBase, IApiKey
{
    public string ApiKey { get; set; }
}

public abstract class AiApiDomainModelBase : AiApiKeyModelBase, IApiDomain
{
    public virtual string ApiDomain { get; set; } = string.Empty;
}

public abstract class AiApiHasModelsModelBase : AiApiDomainModelBase, IHasModels<string>
{
    public string SelectedModel { get; set; } = string.Empty;
    public abstract Task<IEnumerable<string>> GetModelsAsync();
}