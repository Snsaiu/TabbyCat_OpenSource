using TabbyCat.Shared.Enums;

namespace TabbyCat.Repository.Entities.AiEntities;

public class AiChatMessageRecordEntity : UserBaseEntity
{
    public Guid SessionId { get; set; }
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 附件
    /// </summary>
    public string Appendix { get; set; } = string.Empty;

    /// <summary>
    /// 只能是User或者Assistant
    /// </summary>
    public Role Role { get; set; }

    /// <summary>
    /// 是否被加入收藏
    /// </summary>
    public bool IsFavourite { get; set; }
}