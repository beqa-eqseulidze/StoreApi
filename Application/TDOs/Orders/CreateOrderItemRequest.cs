using System.ComponentModel.DataAnnotations;

namespace Application.TDOs.Orders
{
    public class CreateOrderItemRequest
    {
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}
