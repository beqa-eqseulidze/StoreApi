
using Domain.Common;

namespace Domain.Entities
{
    public class OrderItem: BaseAuditabelEntity
    {       
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
