using Domain.Common;

namespace Domain.Interfaces
{
    public interface IBaseRepository<T> where T : BaseAuditabelEntity
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IQueryable<T>> GetFilteredAsync( CancellationToken ct = default);
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        Task<T> DeleteAsync(int id, CancellationToken ct = default);
    }  
}
