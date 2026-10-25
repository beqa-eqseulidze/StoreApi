namespace Domain.Interfaces
{
   public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        Task SaveChangesAsync(CancellationToken ct = default);
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}
