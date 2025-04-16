
using FashionShop.ProductService.Data;
using FashionShop.ProductService.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FashionShop.ProductService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ProductDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
                dbContext.Database.Migrate();
                // Seed data if necessary
                if (!dbContext.ProductCategories.Any() || !dbContext.Discounts.Any())
                {
                    SeedData(dbContext);
                }
                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
        }

        private static void SeedData(ProductDbContext dbContext)
        {
            // Seed categories
            dbContext.ProductCategories.AddRange(
                new ProductCategory { Id = Guid.NewGuid(), Name = "T-Shirts", Description = "Casual t-shirts for all seasons" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Jeans", Description = "Durable and stylish jeans" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Dresses", Description = "Elegant dresses for all occasions" }
            );

            // Seed discounts
            dbContext.Discounts.AddRange(
                new Discount { Id = Guid.NewGuid(), Name = "Summer Sale", Description = "20% off for summer collection", DiscountPercent = 20, Active = true },
                new Discount { Id = Guid.NewGuid(), Name = "Flash Sale", Description = "50% off for limited time", DiscountPercent = 50, Active = false }
            );
        }
    }
}