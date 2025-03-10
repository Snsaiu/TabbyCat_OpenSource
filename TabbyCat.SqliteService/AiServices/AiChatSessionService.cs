using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;

using TuDog.IocAttribute;

namespace TabbyCat.SqliteService.AiServices;

[Register<IAiChatSessionService>]
public sealed class AiChatSessionService : DbServiceBase<AiChatSessionEntity, Guid>, IAiChatSessionService;