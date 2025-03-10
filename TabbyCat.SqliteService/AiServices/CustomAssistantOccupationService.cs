using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;

using TuDog.IocAttribute;

namespace TabbyCat.SqliteService.AiServices;

[Register<ICustomAssistantOccupationService>]
public sealed class CustomAssistantOccupationService:DbServiceBase<CustomAssistantOccupationEntity, Guid>, ICustomAssistantOccupationService;
