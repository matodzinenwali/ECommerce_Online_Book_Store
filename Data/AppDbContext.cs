using BookNexusAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookNexusAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(p => p.Title).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Price).HasPrecision(18, 2);
                entity.HasIndex(p => p.Title).IsUnique(); 
            });
        }

    }
}
