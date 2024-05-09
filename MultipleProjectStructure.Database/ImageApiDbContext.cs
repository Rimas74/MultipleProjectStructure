using Microsoft.EntityFrameworkCore;
using MultipleProjectStructure.Database.Entities;

namespace MultipleProjectStructure.Database
{
    public class ImageApiDbContext : DbContext
    {
        public DbSet<Image> Images { get; set; }
        public DbSet<Thumbnail> Thumbnails { get; set; }

        public ImageApiDbContext(DbContextOptions<ImageApiDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Thumbnail>()
               .HasOne(t => t.Image)
               .WithOne(i => i.Thumbnail)
               .HasForeignKey<Thumbnail>(t => t.ImageId);
            base.OnModelCreating(modelBuilder);
        }

    }
}
