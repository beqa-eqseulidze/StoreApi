
using Domain.Common;
using Domain.Enums.OredrEnums;

namespace Domain.Entities
{
    public class Order: BaseAuditabelEntity
    {      
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Pending, Processing, Shipped, Delivered, Cancelled

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
