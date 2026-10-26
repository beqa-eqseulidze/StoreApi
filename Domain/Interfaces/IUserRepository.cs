using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository: IBaseRepository<User>
    {
        public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
