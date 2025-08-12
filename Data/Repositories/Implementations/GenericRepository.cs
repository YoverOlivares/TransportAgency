using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TransportAgency.Data.Context;
using TransportAgency.Data.Repositories.Interfaces;

namespace TransportAgency.Data.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly TransportAgencyContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(TransportAgencyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log exception here if needed
                throw new Exception($"Error al obtener todas las entidades: {ex.Message}", ex);
            }
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la entidad con ID {id}: {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener entidades con filtro: {ex.Message}", ex);
            }
        }

        public virtual async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la primera entidad: {ex.Message}", ex);
            }
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                await _dbSet.AddAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar la entidad: {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                await _dbSet.AddRangeAsync(entities);
                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar las entidades: {ex.Message}", ex);
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                _dbSet.Update(entity);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar la entidad: {ex.Message}", ex);
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                _dbSet.Remove(entity);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la entidad: {ex.Message}", ex);
            }
        }

        public virtual async Task DeleteByIdAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    await DeleteAsync(entity);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la entidad con ID {id}: {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<T>> GetWithIncludeAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                IQueryable<T> query = _dbSet;

                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener entidades con includes: {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<T>> GetWhereWithIncludeAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                IQueryable<T> query = _dbSet;

                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return await query.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener entidades con filtro e includes: {ex.Message}", ex);
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
                throw new Exception($"Error al contar las entidades: {ex.Message}", ex);
            }
        }

        public virtual async Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.CountAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar las entidades con filtro: {ex.Message}", ex);
            }
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar la existencia de la entidad: {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;

                return await _dbSet
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la página {pageNumber}: {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<T>> GetPagedWhereAsync(
            Expression<Func<T, bool>> predicate,
            int pageNumber,
            int pageSize)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;

                return await _dbSet
                    .Where(predicate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la página {pageNumber} con filtro: {ex.Message}", ex);
            }
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar los cambios: {ex.Message}", ex);
            }
        }
    }
}