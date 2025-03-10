using System.Linq.Expressions;

namespace TabbyCat.Repository;

public interface IDbService<T, TPrimaryKey>
{
    Task<IEnumerable<T>> QueryAsync();

    [Obsolete("内部还未完成实现",true)]
    Task<T?> QueryAsync(TPrimaryKey key);


    Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> where);

    Task<int> CountAsync(Expression<Func<T, bool>> where);

    Task<bool> AddAsync(T entity);

    Task<bool> AddRangeAsync(IEnumerable<T> entities);

    Task<T?> DeleteAsync(Expression<Func<T, bool>> where);


    Task<bool> UpdateAsync(T entity);

    Task<bool> UpdateRangeAsync(IEnumerable<T> entities);

    Task<T?> DeleteAsync(TPrimaryKey key);

    Task<IEnumerable<T>?> DeleteRangeAsync(Expression<Func<T, bool>> where);

    Task<IEnumerable<T>?> DeleteRangeAsync(IEnumerable<T> entities);
}