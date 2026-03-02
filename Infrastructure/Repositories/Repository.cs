using Application.Common;
using Application.Exceptions;
using Application.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly ILogger _logger;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context, ILogger<Repository<T>> logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = _context.Set<T>();
        }
        
            public virtual async Task PatchAsync(int id, Action<T> patchAction)
            {
                try
                {
                    var entity = await _dbSet.FindAsync(id);
                    if (entity == null)
                        throw new BaseServiceException($"{typeof(T).Name} not found: {id}", ExceptionCodes.ItemNotFound);
                    patchAction(entity);
                    _dbSet.Update(entity);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error patching {typeof(T).Name}: {id}");
                    throw new BaseServiceException($"Error patching {typeof(T).Name}: {id}", ex, ExceptionCodes.Database);
                }
            }

            // UPSERT: Insert if not exists, otherwise update
            public virtual async Task UpsertAsync(T entity, Expression<Func<T, bool>> match)
            {
                try
                {
                    var existing = await _dbSet.FirstOrDefaultAsync(match);
                    if (existing == null)
                    {
                        await _dbSet.AddAsync(entity);
                    }
                    else
                    {
                        _context.Entry(existing).CurrentValues.SetValues(entity);
                        _dbSet.Update(existing);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error upserting {typeof(T).Name}");
                    throw new BaseServiceException($"Error upserting {typeof(T).Name}", ex, ExceptionCodes.Database);
                }
            }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting all {typeof(T).Name}s");
                throw new BaseServiceException($"Error getting all {typeof(T).Name}s", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task<List<T>> FindAllAsync(Expression<Func<T, bool>>? match = null)
        {
            try
            {
                return match == null ? await _dbSet.ToListAsync() : await _dbSet.Where(match).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error finding all {typeof(T).Name}s");
                throw new BaseServiceException($"Error finding all {typeof(T).Name}s", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? match = null)
        {
            try
            {
                return match == null ? await _dbSet.ToListAsync() : await _dbSet.Where(match).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting all {typeof(T).Name}s with filter");
                throw new BaseServiceException($"Error getting all {typeof(T).Name}s with filter", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task<T?> FindAsync(Expression<Func<T, bool>> match, bool trackChanges = false)
        {
            try
            {
                var query = _dbSet.AsQueryable();
                if (!trackChanges)
                    query = query.AsNoTracking();
                return await query.FirstOrDefaultAsync(match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error finding {typeof(T).Name}");
                throw new BaseServiceException($"Error finding {typeof(T).Name}", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking existence of {typeof(T).Name}");
                throw new BaseServiceException($"Error checking existence of {typeof(T).Name}", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task<int> CountAsync()
        {
            try
            {
                return await _dbSet.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error counting {typeof(T).Name}s");
                throw new BaseServiceException($"Error counting {typeof(T).Name}s", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                await _dbSet.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding range of {typeof(T).Name}s");
                throw new BaseServiceException($"Error adding range of {typeof(T).Name}s", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing range of {typeof(T).Name}s");
                throw new BaseServiceException($"Error removing range of {typeof(T).Name}s", ex, ExceptionCodes.Database);
            }
        }

        public virtual IEnumerable<T> Filter(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? page = null, int? pageSize = null, params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                IQueryable<T> query = _dbSet;
                if (filter != null)
                    query = query.Where(filter);
                foreach (var includeProperty in includeProperties)
                    query = query.Include(includeProperty);
                if (orderBy != null)
                    query = orderBy(query);
                if (page.HasValue && pageSize.HasValue)
                    query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
                return query.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error filtering {typeof(T).Name}s");
                throw new BaseServiceException($"Error filtering {typeof(T).Name}s", ex, ExceptionCodes.Database);
            }
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null, bool ignoreGlobalQueryFilter = false, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> query = _dbSet;
                if (predicate != null)
                    query = query.Where(predicate);
                foreach (var include in includes)
                    query = query.Include(include);
                // Note: ignoreGlobalQueryFilter is not implemented here, but can be added if needed
                return query;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error querying {typeof(T).Name}s");
                throw new BaseServiceException($"Error querying {typeof(T).Name}s", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing raw SQL for {typeof(T).Name}s");
                throw new BaseServiceException($"Error executing raw SQL for {typeof(T).Name}s", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    throw new BaseServiceException($"{typeof(T).Name} not found: {id}", ExceptionCodes.ItemNotFound);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting {typeof(T).Name} by id: {id}");
                throw new BaseServiceException($"Error getting {typeof(T).Name} by id: {id}", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding {typeof(T).Name}");
                throw new BaseServiceException($"Error adding {typeof(T).Name}", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating {typeof(T).Name}");
                throw new BaseServiceException($"Error updating {typeof(T).Name}", ex, ExceptionCodes.Database);
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    throw new BaseServiceException($"{typeof(T).Name} not found: {id}", ExceptionCodes.ItemNotFound);
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting {typeof(T).Name}: {id}");
                throw new BaseServiceException($"Error deleting {typeof(T).Name}: {id}", ex, ExceptionCodes.Database);
            }
        }
    }
}
