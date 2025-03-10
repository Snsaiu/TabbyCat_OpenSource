using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Models;

public class ChatGlmModel : AiApiModelBase, IApiDomain
{
    public override AiModelType Provider { get; } = AiModelType.ChatGLM;
    public string ApiDomain { get; set; } = "http://localhost:8000";
}