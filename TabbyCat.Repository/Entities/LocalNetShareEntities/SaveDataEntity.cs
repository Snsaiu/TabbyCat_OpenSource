using SQLite;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Repository.Entities.LocalNetShareEntities;

/// <summary>
/// 保存的数据
/// </summary>
public class SaveDataEntity:AuditEntityBase
{
    
    public DateTime Time { get; set; }

    public SendType DataType { get; set; }

    public string SourceDeviceNickName { get; set; } = string.Empty;

    public string Guid { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
}