using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyResultModel;
using FantasyResultModel.Impls;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TuDog.Extensions;

namespace TabbyCat.Models;

public partial class GroqModel : AiApiKeyModelBase, IHasModels<string>, IInitializeable
{
    public override AiModelType Provider => AiModelType.Groq;

    [ObservableProperty] private string _selectedModel = string.Empty;

    public Task<IEnumerable<string>> GetModelsAsync()
    {
        return Task.FromResult<IEnumerable<string>>([]);
    }

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