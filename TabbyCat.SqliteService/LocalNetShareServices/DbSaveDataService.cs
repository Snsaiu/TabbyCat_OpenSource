using FantasyResultModel;
using FantasyResultModel.Impls;
using TabbyCat.Repository;
using TabbyCat.Repository.Entities.LocalNetShareEntities;
using TabbyCat.Service.LocalNetShareServices;

using TuDog.IocAttribute;

namespace TabbyCat.SqliteService.LocalNetShareServices;

[Register<ISaveDataService>]
public class DbSaveDataService : DbBase, ISaveDataService
{
    protected override Task CreateTableAsync()
    {
        return connection.CreateTableAsync<SaveDataEntity>();
    }

    public async Task<ResultBase<List<SaveDataEntity>>> GetAllAsync()
    {
        try
        {

            var list = await connection.Table<SaveDataEntity>().ToListAsync();
            return new SuccessResultModel<List<SaveDataEntity>>(list);
        }
        catch (Exception e)
        {
            return new ErrorResultModel<List<SaveDataEntity>>(e.Message);
        }
    }

    public async Task<ResultBase<bool>> DeleteDataAsync(string guid)
    {
        var i = await connection.Table<SaveDataEntity>().DeleteAsync(x => x.Guid == guid);
        return i > 0 ? new SuccessResultModel<bool>(true) : new ErrorResultModel<bool>("清除失败！");
    }

    public async Task<ResultBase<bool>> ClearAsync()
    {
        var i = await connection.Table<SaveDataEntity>().DeleteAsync(x => x.Guid != "");
        return i > 0 ? new SuccessResultModel<bool>(true) : new ErrorResultModel<bool>("清除失败！");
    }

    public async Task<ResultBase<SaveDataEntity>> AddAsync(SaveDataEntity model)
    {
        var x = await connection.InsertAsync(model);
        return x > 0 ? new SuccessResultModel<SaveDataEntity>(model) : new ErrorResultModel<SaveDataEntity>("插入失败");
    }
}