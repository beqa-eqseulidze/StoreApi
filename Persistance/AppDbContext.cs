using Microsoft.EntityFrameworkCore;
using Persistance.Extentions;
using System.Reflection;


namespace Persistance
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ავტომატურად დარეგისტრირდება ყველა IEntityTypeConfiguration მიმდინარე assembly-დან
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());           
        }

        // SaveChanges-ის წინ ვიძახებთ AddAuditing მეთოდს, რომელიც უზრუნველყოფს soft delete და audit-ის ლოგიკას
        public override int SaveChanges()
        {
            this.AddAuditing();
            return base.SaveChanges();
        }

        // SaveChangesAsync-ის წინ ვიძახებთ AddAuditing მეთოდს, რომელიც უზრუნველყოფს soft delete და audit-ის ლოგიკას
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.AddAuditing();
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}