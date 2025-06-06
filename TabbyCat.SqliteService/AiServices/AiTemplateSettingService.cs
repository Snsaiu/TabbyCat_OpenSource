using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;

using TuDog.IocAttribute;

namespace TabbyCat.SqliteService.AiServices;

[Register<IAiTemplateSettingService>]
public sealed class AiTemplateSettingService : DbServiceBase<AiTemplateSettingEntity, Guid>, IAiTemplateSettingService;