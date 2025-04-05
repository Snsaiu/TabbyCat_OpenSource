using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FantasyResultModel;
using FantasyResultModel.Impls;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TuDog.Bootstrap;
using TuDog.Extensions;

namespace TabbyCat.Models;

public abstract class AiApiModelBase : ModelBase
{
    // 模型提供方
    public abstract AiModelType Provider { get; }

    public int ContextCount { get; set; }

    public bool ContextCountLimit { get; set; }

    public double Temperature { get; set; } = 0.1;

    public bool IsDefault { get; set; }

}

public abstract class AiApiKeyModelBase : AiApiModelBase, IApiKey
{
    public string ApiKey { get; set; } = string.Empty;
}

public abstract class AiApiDomainModelBase : AiApiKeyModelBase, IApiDomain
{
    public virtual string ApiDomain { get; set; } = string.Empty;
}

public abstract class AiApiHasModelsModelBase : AiApiDomainModelBase, IHasModels<string>, IInitializeable
{
    public string SelectedModel { get; set; } = string.Empty;
    public abstract Task<IEnumerable<string>> GetModelsAsync();
    public ObservableCollection<string> Models { get; set; } = [];

    public async Task<ResultBase<bool>> InitializeAsync()
    {
        var models = await GetModelsAsync();
        if (!models.Any())
            return new ErrorResultModel<bool>("No models found");
        Models.Reset(models);
        return new SuccessResultModel<bool>();

    }
}