using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Ai.Models
{
    /// <summary>
    /// 兼容OpenAi的Api模型
    /// </summary>
    public class CompatibleOpenAiApiModel : AiApiDomainModelBase, IAlias, IApiPath, IDeployName, ITopP, ISaved
    {
        public override AiModelType Provider { get; } = AiModelType.Custom;

        public string ApiPath { get; set; } = "/chat/completions";
        public string DeployName { get; set; } = "gpt-4o";

        public override string ApiDomain { get; set; } = "https://api.openai.com/v1";
        public double TopP { get; set; }
        public string Alias { get; set; } = string.Empty;
        public bool IsSaved { get; set; }
    }
}
