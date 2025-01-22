using TabbyCat.Shared.Enums;

namespace TabbyCat.Repository.Entities.AiEntities;

public class AiChatSessionEntity : AuditEntityBase
{

    /// <summary>
    /// 多轮对话的主题内容，默认可能是会话1 会话2 会话3......
    /// </summary>
    public string Theme { get; set; } = string.Empty;

    /// <summary>
    /// 自定义主题名称，如果为空，那么默认使用Theme，否则优先使用CustomTheme
    /// </summary>
    public string? CustomTheme { get; set; }

    public AssistantOccupation Occupation { get; set; }

    /// <summary>
    /// 自定义角色,如果<see cref="AssistantOccupation"/>是<see cref="AssistantOccupation.Custom"/>，那么这个字段必须有值
    /// </summary>
    public string? CustomOccupationName { get; set; }

    /// <summary>
    /// 是否是默认的绘画
    /// </summary>
    public bool  IsDefault { get; set; }

    public static AiChatSessionEntity CreateDefault()
    {
        return new() { Theme = "默认会话", Occupation = AssistantOccupation.Common, IsDefault = true};
    }
}