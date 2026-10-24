
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class User:BaseAuditabelEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Customer; // Admin = 1, Customer = 2

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
