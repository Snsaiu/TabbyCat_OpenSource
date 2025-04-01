namespace TabbyCat.Repository.Entities;

public abstract class UserBaseEntity : AuditEntityBase
{
    public string PhoneNumber { get; set; } = string.Empty;
}