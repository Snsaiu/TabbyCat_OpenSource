using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiMediaEntities;

namespace TabbyCat.Service.RunningHubServices;

public interface IAiMediaService:IDbService<AiMediaResultEntity,Guid>
{
    
}