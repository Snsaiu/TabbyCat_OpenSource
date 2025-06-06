using TabbyCat.Shared.Enums;

namespace TabbyCat.Repository.Entities.AiEntities;

/// <summary>
/// Ai模板设置
/// </summary>
public class AiTemplateSettingEntity : RemoteSyncEntityBase
{
    /// <summary>
    /// 模型模板,序列化json
    /// </summary>
    public string Template { get; set; } = string.Empty;

    /// <summary>
    /// 模型提供方
    /// </summary>
    public AiModelType Provider { get; set; }


    /// <summary>
    /// 模型,只有在<see cref="AiModelType"/> 为<see cref="AiModelType.Custom"/>有效
    /// </summary>
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// 是否是默认模板
    /// </summary>
    public bool IsDefault { get; set; }


}