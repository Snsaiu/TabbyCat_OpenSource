using TabbyCat.Repository;
using TabbyCat.Repository.Entities.RunningHubEntities;
using TabbyCat.Service.RunningHubServices;
using TuDog.IocAttribute;

namespace TabbyCat.SqliteService.RunningHubServices;

[Register<IRunningHubResultService>]
public class RunningHubResultService : DbServiceBase<RunningHubResultEntity, Guid>, IRunningHubResultService
{
}