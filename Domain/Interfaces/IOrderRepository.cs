using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<Order?> GetByIdWithItemsAsync(int id, CancellationToken ct = default);
        Task<IQueryable<Order>> GetUserOrdersAsync(int userId, CancellationToken ct = default);
    }
}
