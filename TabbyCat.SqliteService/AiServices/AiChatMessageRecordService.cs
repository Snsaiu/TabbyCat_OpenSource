using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;

namespace TabbyCat.SqliteService.AiServices;

public sealed class AiChatMessageRecordService : DbServiceBase<AiChatMessageRecordEntity, Guid>, IAiChatMessageRecordService;