using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Models;

/// <summary>
/// 兼容OpenAi的Api模型
/// </summary>
public partial class CompatibleOpenAiApiModel : AiApiDomainModelBase, IAlias, IApiPath, IDeployName, ITopP, ISaved
{
    public override AiModelType Provider => AiModelType.Custom;

    [ObservableProperty] private string _apiPath = "/chat/completions";
    [ObservableProperty] private string _deployName = "gpt-4o";

    [ObservableProperty] private double _topP = 0.1;
    [ObservableProperty] private string _alias = string.Empty;

    [ObservableProperty] private bool _isSaved;

    public CompatibleOpenAiApiModel()
    {
        ApiDomain = "https://api.openai.com/v1";
    }
}