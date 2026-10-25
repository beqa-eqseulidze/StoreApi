using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistance.Common;

namespace Persistance.Repositories
{
    public sealed class UserRepository(AppDbContext dbContext) : BaseRepository<User>(dbContext), IUserRepository
    {
        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken) =>        
           await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
 
    }
   
}   
