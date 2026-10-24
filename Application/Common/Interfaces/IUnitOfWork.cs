
namespace Application.Common.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {        
        ITaskRepository TaskRepository { get; }
        Task SaveChangesAsync(CancellationToken ct = default);
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}
