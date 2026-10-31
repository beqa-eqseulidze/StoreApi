using Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistance.Extentions
{
    public static class GlobalSoftDeletedQueryFilter
    {
        public static ModelBuilder ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var isSoftDeleteRootEntity =
                    entityType.BaseType is null &&
                    typeof(BaseAuditabelEntity).IsAssignableFrom(entityType.ClrType);

                if (!isSoftDeleteRootEntity)
                {
                    continue;
                }

                var entity = Expression.Parameter(entityType.ClrType, "entity");
                var isDeleted = Expression.Property(entity, nameof(BaseAuditabelEntity.IsDeleted));
                var filter = Expression.Lambda(Expression.Not(isDeleted), entity);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }

            return modelBuilder;
        }
    }
}
