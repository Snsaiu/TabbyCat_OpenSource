using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiMediaEntities;
using TabbyCat.Service.RunningHubServices;
using TabbyCat.Shared.Enums;
using TuDog.IocAttribute;

namespace TabbyCat.SqliteService.RunningHubServices;

[Register<IRunningStateService>]
public sealed class RunningStateService:DbServiceBase<AiMediaRunningStateEntity,Guid>,IRunningStateService
{
}