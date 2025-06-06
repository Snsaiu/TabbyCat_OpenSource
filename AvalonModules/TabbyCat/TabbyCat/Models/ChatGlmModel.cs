using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Models;

public partial class ChatGlmModel : AiApiModelBase, IApiDomain
{
    public override AiModelType Provider => AiModelType.ChatGLM;

    [ObservableProperty] private string _apiDomain = "http://localhost:8000";
}