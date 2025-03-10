using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TuDog.Extensions;

namespace TabbyCat.Models;

public class GroqModel : AiApiKeyModelBase, IHasModels<string>, IInitializeable
{
    public override AiModelType Provider { get; } = AiModelType.Groq;
    public string SelectedModel { get; set; } = string.Empty;

    public Task<IEnumerable<string>> GetModelsAsync()
    {
        return Task.FromResult<IEnumerable<string>>([]);
    }

    public ObservableCollection<string> Models { get; set; } = [];

    public async Task InitializeAsync()
    {
        Models.Reset(await GetModelsAsync());
    }
}