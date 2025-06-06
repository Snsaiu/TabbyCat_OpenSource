using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;

using TuDog.IocAttribute;

namespace TabbyCat.SqliteService.AiServices;

[Register<IAiChatMessageRecordService>]
public sealed class AiChatMessageRecordService : DbServiceBase<AiChatMessageRecordEntity, Guid>, IAiChatMessageRecordService;