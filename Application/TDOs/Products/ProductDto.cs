

namespace Application.TDOs.Products
{
    public record ProductDto(
            int Id,
            string Name, 
            string Description, 
            decimal Price, 
            int StockQuantity,
            int CategoryId,
            List<string>? ImageUrls, 
            List<string>? ProductTagNames,
            List<int>? OrderItemIds
        );

}
