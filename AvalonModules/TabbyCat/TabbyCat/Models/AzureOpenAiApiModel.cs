using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FantasyResultModel;
using FantasyResultModel.Impls;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TuDog.Extensions;

namespace TabbyCat.Models;

public class AzureOpenAiApiModel : AiApiKeyModelBase, IHasModels<string>, IEndPoint, IDeployName, IInitializeable
{
    public string EndPoint { get; set; } = string.Empty;
    public override AiModelType Provider => AiModelType.AzureOpenAiApi;
    public string SelectedModel { get; set; } = string.Empty;

    public string DeployName { get; set; } = string.Empty;

    public Task<IEnumerable<string>> GetModelsAsync()
    {
        return Task.FromResult<IEnumerable<string>>([]);
    }

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