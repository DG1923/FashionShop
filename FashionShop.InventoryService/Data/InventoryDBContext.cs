using FashionShop.InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace FashionShop.InventoryService.Data
{
    public class InventoryDBContext:DbContext
    {
        public DbSet<Inventory> Inventories { get; set; }
        public InventoryDBContext(DbContextOptions<InventoryDBContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Inventory>()
                .ToTable("Inventory");
        }
    }
}
