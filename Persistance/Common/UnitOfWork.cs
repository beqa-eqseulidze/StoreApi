using Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Persistance.Repositories;

namespace Persistance.Common
{
    public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
    {
        private readonly AppDbContext _dbContext = dbContext;
        private IDbContextTransaction? _transaction;
        public IUserRepository? _users;
        public IUserRepository UserRepository => _users ??= new UserRepository(_dbContext);

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync(ct);           
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            try 
            {
                await SaveChangesAsync(ct);
                if (_transaction != null)
                {
                    await _transaction.CommitAsync(ct);
                }
            }
            catch
            {
                await RollbackTransactionAsync(ct);
                throw;
            }
            finally
            {
                if(_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(ct);
            }
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        public void Dispose()=>_dbContext.Dispose();
    }
}
