namespace TabbyCat.Repository.Entities;

/// <summary>
/// 带有审计信息的实体基类
/// </summary>
public abstract class AuditEntityBase : EntityBase
{
    public DateTime CreateTime { get; set; } = DateTime.Now;
    public DateTime UpdateTime { get; set; } = DateTime.Now;

}