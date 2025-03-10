using TabbyCat.Repository;
using TabbyCat.Repository.Entities.RunningHubEntities;

namespace TabbyCat.Service.RunningHubServices;

public interface IRunningHubService:IDbService<RunningHubEntity,Guid>
{
    
}