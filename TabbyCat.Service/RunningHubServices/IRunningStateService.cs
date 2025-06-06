using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiMediaEntities;

namespace TabbyCat.Service.RunningHubServices;

public interface IRunningStateService:IDbService<AiMediaRunningStateEntity,Guid>
{
}