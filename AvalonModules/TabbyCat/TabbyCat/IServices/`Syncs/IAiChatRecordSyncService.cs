using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.IServices;

public interface IAiChatRecordSyncService : ISyncService<IEnumerable<AiChatMessageRecordEntity>>;