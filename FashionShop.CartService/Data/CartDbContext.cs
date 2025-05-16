using FashionShop.CartService.Models;
using Microsoft.EntityFrameworkCore;

namespace FashionShop.CartService.Data
{
    public class CartDbContext : DbContext
    {
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



        }
    }
}
