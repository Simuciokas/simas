using AsgardMarketplace.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AsgardMarketplace.Domain.DataModels
{
    public class AsgardContext : DbContext
    {
        public AsgardContext(DbContextOptions<AsgardContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .Property(p => p.Id)
                .ValueGeneratedNever();
            modelBuilder.Entity<Order>()
                .Property(p => p.Id)
                .ValueGeneratedNever();
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
