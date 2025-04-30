using TabbyCat.Repository;
using TabbyCat.Repository.Entities.AiMediaEntities;

namespace TabbyCat.Service.RunningHubServices;

public interface IAiMediaResultService : IDbService<AiMediaResultEntity, Guid>
{
}