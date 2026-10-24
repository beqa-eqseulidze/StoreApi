using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configuration
{
    public class TaskItemConfigration : BaseEntityConfiguration<TaskItem>
    {
        
        public override void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            // add soft delete configuration from base class
            base.Configure(builder);

            //add table name configuration
            builder.ToTable("Tasks");

            // Configure the TaskItem entity
            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(150);
            builder.Property(t => t.Description)
                .HasMaxLength(1000);
            builder.Property(t => t.IsCompleted)
                .IsRequired();
            builder.Property(t => t.DueDate)
                .IsRequired();

        }
    }
}
