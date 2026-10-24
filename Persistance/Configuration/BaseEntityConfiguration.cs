
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configuration
{
    public class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseAuditabelEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Add a global query filter to exclude soft-deleted entities for all entities that inherit from BaseEntity
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
