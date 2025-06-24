namespace TabbyCat.Repository.Entities;

public abstract class SyncStatusEntity : UserBaseEntity
{
    public bool SyncStatus { get; set; }
}