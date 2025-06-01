using FashionShop.OrderService.Model;
using Microsoft.EntityFrameworkCore;

namespace FashionShop.OrderService.Data
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrderItem>()
                .Property(orderItem => orderItem.BasePrice)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Order>()
                .Property(order => order.Total)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<PaymentDetail>()
                .Property(paymentDetail => paymentDetail.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .HasMany(order => order.OrderItems)
                .WithOne(orderItem => orderItem.Order)
                .HasForeignKey(orderItem => orderItem.OrderId);

            modelBuilder.Entity<Order>()
                .HasOne(order => order.PaymentDetail)
                .WithOne(payment => payment.Order)
                .HasForeignKey<PaymentDetail>(payment => payment.OrderId); // Fixed the issue by specifying the correct type for the foreign key
        }
    }
}
