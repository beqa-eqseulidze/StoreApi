using Domain.Entities;

namespace Application.TDOs.Category
{
    public record CategoryDto(int Id, string Name, string Description, List<Product>? Products = null);
}