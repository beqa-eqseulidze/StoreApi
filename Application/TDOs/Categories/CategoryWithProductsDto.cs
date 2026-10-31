

using Application.TDOs.Products;

namespace Application.TDOs.Categories
{
    public record CategoryWithProductsDto(
    int Id,
    string Name,
    string Description,
    List<ProductSummaryDto>? Products = null)
    {
        public List<ProductSummaryDto> Products { get; init; } = Products ?? [];
    }
}