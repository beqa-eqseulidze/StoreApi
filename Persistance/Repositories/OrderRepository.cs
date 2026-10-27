using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistance.Common;

namespace Persistance.Repositories
{
    public class OrderRepository(AppDbContext dbContext) : BaseRepository<Order>(dbContext), IOrderRepository
    {
        public async Task<Order?> GetByIdWithItemsAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id, ct);
        }

        public async Task<IQueryable<Order>> GetUserOrdersAsync(int userId, CancellationToken ct = default)
        {
            return _dbSet
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems);
        }
    }
}
