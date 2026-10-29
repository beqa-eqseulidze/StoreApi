using Domain.Entities;
using Domain.Interfaces;
using Persistance.Common;

namespace Persistance.Repositories
{
    public class ProductImageRepository(AppDbContext dbContext) : BaseRepository<ProductImage>(dbContext), IProductImageRepository
    {
    }
}
