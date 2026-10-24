using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Extentions
{
    public static class AuditingExtensions
    {
        public static void AddAuditing(this AppDbContext context)
        {
            var entries = context.ChangeTracker.Entries<BaseAuditabelEntity>();
            var utcNow = DateTimeOffset.UtcNow;

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = utcNow;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = utcNow;
                        break;

                    case EntityState.Deleted:
                        // Soft Delete ლოგიკა
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = utcNow;
                        entry.Entity.UpdatedAt = utcNow;
                        break;
                }
            }
        }
    }
}
