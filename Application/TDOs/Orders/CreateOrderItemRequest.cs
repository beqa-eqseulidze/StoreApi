using System.ComponentModel.DataAnnotations;

namespace Application.TDOs.Orders
{
    public class CreateOrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
