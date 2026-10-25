using Application.Common.Exeptions;
using Domain.Common;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Persistance.Common
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseAuditabelEntity
    {
        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>(); // ავტომატურად იღებს შესაბამისი ტიპის DbSet-ს
        }

        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _dbSet.FindAsync([id], ct) ?? throw new EntityNotFoundExeption($"Task with Id {id} not found.");
            _dbSet.Remove(entity);
            return await _dbSet.FindAsync([id], ct);
        }

        public virtual async Task<IQueryable<T>> GetFilteredAsync(CancellationToken ct = default)
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
            return entity;
        }

        public virtual async Task<T> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _dbSet.FindAsync([id], ct) ?? throw new EntityNotFoundExeption($"Task with Id {id} not found.");
            _dbSet.Remove(entity);
            return entity;
        }
    }
}
