using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyResultModel;
using FantasyResultModel.Impls;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TuDog.Bootstrap;
using TuDog.Extensions;

namespace TabbyCat.Models;

public abstract partial class AiApiModelBase : ModelBase
{
    // 模型提供方
    public abstract AiModelType Provider { get; }

    [ObservableProperty] private int _contextCount;

    [ObservableProperty] private bool _contextCountLimit;

    [ObservableProperty] private double _temperature = 0.1;

    [ObservableProperty] private bool _isDefault;

}

public abstract partial class AiApiKeyModelBase : AiApiModelBase, IApiKey
{
    [ObservableProperty] private string _apiKey = string.Empty;
}

public abstract partial class AiApiDomainModelBase : AiApiKeyModelBase, IApiDomain
{
    [ObservableProperty] private string _apiDomain = string.Empty;
}

public abstract partial class AiApiHasModelsModelBase : AiApiDomainModelBase, IHasModels<string>, IInitializeable
{
    [ObservableProperty] private string _selectedModel = string.Empty;
    public abstract Task<IEnumerable<string>> GetModelsAsync();

    [ObservableProperty] private ObservableCollection<string> _models = [];

    public async Task<ResultBase<bool>> InitializeAsync()
    {
        var models = await GetModelsAsync();
        if (!models.Any())
            return new ErrorResultModel<bool>("No models found");
        Models.Reset(models);
        return new SuccessResultModel<bool>();

    }
}