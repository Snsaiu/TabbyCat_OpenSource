namespace TabbyCat.Repository.Entities;

public abstract class UserBaseEntity : AuditEntityBase
{
    public string Email { get; set; } = string.Empty;
}