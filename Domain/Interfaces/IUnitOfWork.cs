using Domain.Common;
namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IBaseRepository<TEntity> Repository<TEntity>() where TEntity : BaseAuditabelEntity;

        ITaskRepository Tasks { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<BaseAuditabelEntity> BeginTransactionAsync(CancellationToken ct = default);
        Task<BaseAuditabelEntity> CommitTransactionAsync(CancellationToken ct = default);
        Task<BaseAuditabelEntity> RollbackTransactionAsync(CancellationToken ct = default);
    }
}
