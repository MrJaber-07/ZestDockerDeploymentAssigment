using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);

        Task<List<T>> FindAllAsync(System.Linq.Expressions.Expression<System.Func<T, bool>> match = null);
        Task<IEnumerable<T>> GetAllAsync(System.Linq.Expressions.Expression<System.Func<T, bool>> match = null);
        Task<T?> FindAsync(System.Linq.Expressions.Expression<System.Func<T, bool>> match, bool trackChanges = false);
        Task<bool> IsExistAsync(System.Linq.Expressions.Expression<System.Func<T, bool>> predicate);
        Task<int> CountAsync();
        Task AddRangeAsync(IEnumerable<T> entities);
        Task RemoveRangeAsync(IEnumerable<T> entities);
        System.Collections.Generic.IEnumerable<T> Filter(System.Linq.Expressions.Expression<System.Func<T, bool>> filter = null, System.Func<System.Linq.IQueryable<T>, System.Linq.IOrderedQueryable<T>> orderBy = null, int? page = null, int? pageSize = null, params System.Linq.Expressions.Expression<System.Func<T, object>>[] includeProperties);
        System.Linq.IQueryable<T> Query(System.Linq.Expressions.Expression<System.Func<T, bool>> predicate = null, bool ignoreGlobalQueryFilter = false, params System.Linq.Expressions.Expression<System.Func<T, object>>[] includes);
        Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
        Task PatchAsync(int id, System.Action<T> patchAction);
        Task UpsertAsync(T entity, System.Linq.Expressions.Expression<System.Func<T, bool>> match);
    }
}
