using TabbyCat.Repository;
using TabbyCat.Repository.Entities.RunningHubEntities;
using TabbyCat.Service.RunningHubServices;
using TuDog.IocAttribute;

namespace TabbyCat.SqliteService.RunningHubServices;

[Register<IRunningHubService>]
public sealed class RunningHubService:DbServiceBase<RunningHubEntity,Guid>,IRunningHubService
{
    
}