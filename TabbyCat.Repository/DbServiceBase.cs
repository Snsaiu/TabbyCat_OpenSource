using System.Linq.Expressions;
using TabbyCat.Repository.Entities;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Repository;

public class DbServiceBase<T, TPrimaryKey> : DbBase, IDbService<T, TPrimaryKey> where T : IPrimaryKey<TPrimaryKey>, new()
{
    public async Task<IEnumerable<T>> QueryAsync()
    {

        var list = await connection.Table<T>().ToListAsync();
        return list;
    }

    public async Task<T?> QueryAsync(TPrimaryKey key)
    {
        var find = await connection.Table<T>().FirstOrDefaultAsync(x => x.Key != null && x.Key.Equals(key));
        return find;
    }

    public async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> where)
    {
        var list = await connection.Table<T>().Where(where).ToListAsync();
        return list;
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> where)
    {
        var count = await connection.Table<T>().CountAsync(where);
        return count;
    }

    public async Task<bool> AddAsync(T entity) => (await connection.InsertAsync(entity)) > 0;

    public async Task<bool> AddRangeAsync(IEnumerable<T> entities)
    {
        var count = await connection.InsertAllAsync(entities);
        return count == entities.Count();
    }

    public async Task<T?> DeleteAsync(Expression<Func<T, bool>> where)
    {
        var finds = await QueryAsync(where);
        if (!finds.Any()) return default;

        await connection.DeleteAsync(finds.First());
        return finds.First();
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        if (entity is AuditEntityBase auditEntity)
        {
            auditEntity.UpdateTime = DateTime.Now;
        }
        return (await connection.UpdateAsync(entity)) > 0;
    }

    public async Task<bool> UpdateRangeAsync(IEnumerable<T> entities)
    {
        return await connection.UpdateAllAsync(entities) > 0;
    }

    public async Task<T?> DeleteAsync(TPrimaryKey key)
    {
        var find = await QueryAsync(key);
        if (find is null)
            return default;
        await connection.DeleteAsync(find);
        return find;
    }

    public async Task<IEnumerable<T>?> DeleteRangeAsync(Expression<Func<T, bool>> where)
    {
        var finds = await QueryAsync(where);
        if (!finds.Any()) return null;

        foreach (var item in finds)
        {
            await connection.DeleteAsync(item);
        }
        return finds;
    }

    public async Task<IEnumerable<T>?> DeleteRangeAsync(IEnumerable<T> entities)
    {
        foreach (var item in entities)
        {
            await connection.DeleteAsync(item);
        }
        return entities;
    }

    protected override Task CreateTableAsync()
    {
        return connection.CreateTableAsync<T>();
    }
}