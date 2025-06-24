using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.IServices;

public interface IAiTemplateSettingSyncService : IIncrementSyncService<IEnumerable<AiTemplateSettingEntity>>;