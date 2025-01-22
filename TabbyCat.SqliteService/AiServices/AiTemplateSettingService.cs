using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;

namespace TabbyCat.SqliteService.AiServices;

public sealed class AiTemplateSettingService : DbServiceBase<AiTemplateSettingEntity, Guid>, IAiTemplateSettingService;