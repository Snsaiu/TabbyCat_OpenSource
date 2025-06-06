using System.Collections;
using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.Service.AiServices;

public interface IAiTemplateSettingService : IDbService<AiTemplateSettingEntity, Guid>;