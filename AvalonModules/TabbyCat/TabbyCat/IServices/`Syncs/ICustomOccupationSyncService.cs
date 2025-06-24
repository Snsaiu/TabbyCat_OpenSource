using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.IServices;

/// <summary>
/// 同步自定义联系人
/// </summary>
public interface ICustomOccupationSyncService : IIncrementSyncService<IEnumerable<CustomAssistantOccupationEntity>>;