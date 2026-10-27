
using Domain.Entities;
using Domain.Interfaces;
using Persistance.Common;

namespace Persistance.Repositories
{
    public class ProductRepository(AppDbContext dbContext) : BaseRepository<Product>(dbContext), IProductRepository
    {
    }
}
