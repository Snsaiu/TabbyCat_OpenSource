using FantasyResultModel;
using TabbyCat.Repository.Entities.LocalNetShareEntities;

namespace TabbyCat.Service.LocalNetShareServices;

public interface ISaveDataService
{
    Task<ResultBase<List<SaveDataEntity>>> GetAllAsync();

    Task<ResultBase<bool>> DeleteDataAsync(string guid);

    Task<ResultBase<bool>> ClearAsync();

    Task<ResultBase<SaveDataEntity>> AddAsync(SaveDataEntity model);
}