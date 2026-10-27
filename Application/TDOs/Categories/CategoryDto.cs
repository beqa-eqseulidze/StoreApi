

using Application.TDOs.Products;

namespace Application.TDOs.Categories
{
    public record CategoryDto(
    int Id,
    string Name,
    string Description,
    List<ProductDto>? Products = null)
    {
        public List<ProductDto> Products { get; init; } = Products ?? [];
    }
}