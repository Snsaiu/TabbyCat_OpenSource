using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiMediaEntities;
using TabbyCat.Service.RunningHubServices;
using TuDog.IocAttribute;

namespace TabbyCat.SqliteService.RunningHubServices;

[Register<IAiMediaResultService>]
public class AiMediaResultService : DbServiceBase<AiMediaResultEntity, Guid>, IAiMediaResultService
{
}