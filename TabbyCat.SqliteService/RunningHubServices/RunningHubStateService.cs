using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Repository;
using TabbyCat.Repository.Entities.RunningHubEntities;
using TabbyCat.Service.RunningHubServices;
using TabbyCat.Shared.Enums;
using TuDog.IocAttribute;

namespace TabbyCat.SqliteService.RunningHubServices;

[Register<IRunningHubStateService>]
public sealed class RunningHubStateService:DbServiceBase<RunningHubStateEntity,Guid>,IRunningHubStateService
{
}