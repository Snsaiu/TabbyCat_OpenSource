using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiMediaEntities;
using TabbyCat.Service.RunningHubServices;
using TuDog.IocAttribute;

namespace TabbyCat.SqliteService.RunningHubServices;

[Register<IAiMediaService>]
public sealed class AliMediaService:DbServiceBase<AiMediaResultEntity,Guid>,IAiMediaService
{
    
}