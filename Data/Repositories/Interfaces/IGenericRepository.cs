using System.Linq.Expressions;

namespace TransportAgency.Data.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Operaciones de consulta
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        // Operaciones de modificación
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteByIdAsync(int id);

        // Operaciones con filtros avanzados
        Task<IEnumerable<T>> GetWithIncludeAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetWhereWithIncludeAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties);

        // Operaciones de conteo y existencia
        Task<int> CountAsync();
        Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        // Operaciones de paginación
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<T>> GetPagedWhereAsync(
            Expression<Func<T, bool>> predicate,
            int pageNumber,
            int pageSize);

        // Operación para guardar cambios
        Task<int> SaveChangesAsync();
    }
}