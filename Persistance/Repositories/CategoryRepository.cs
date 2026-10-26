
using Domain.Entities;
using Domain.Interfaces;
using Persistance.Common;

namespace Persistance.Repositories
{
    public class CategoryRepository(AppDbContext dbContext) : BaseRepository<Category>(dbContext), ICategoryRepository
    {
    }
}
