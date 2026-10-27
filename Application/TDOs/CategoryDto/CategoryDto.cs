
namespace Application.TDOs.CategoryDto
{
    public record CategoryDto(int Id, string Name, string Description, List<Domain.Entities.Product>? Products = null);
}