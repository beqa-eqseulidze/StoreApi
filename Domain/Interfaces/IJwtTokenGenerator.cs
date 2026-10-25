
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(User user);      
    }
}
