using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.Service.AiServices;

public interface IAiChatSessionService : IDbService<AiChatSessionEntity, Guid>;