
using FashionShop.ProductService.Data;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Reflection.Emit;
using System.Text;

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
            //add caching services
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                string redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
                options.Configuration = redisConnectionString;
            });
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            builder.Services.AddScoped<IProductCategoryRepo, ProductCategoryRepo>();
            builder.Services.AddScoped<IProductRepo, ProductRepo>();
            builder.Services.AddScoped<IDiscountRepo, DiscountRepo>();
            builder.Services.AddScoped<IProductVariationRepo, ProductVariationRepo>();
            //// Configure authentication
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //                    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //            ValidAudience = builder.Configuration["Jwt:Audience"],
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            //        };
            //    });

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
                app.UseAuthentication();    
                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
        }

        private static void SeedData(ProductDbContext dbContext)
        {
            // Seed categories
            var category1 = new ProductCategory { Id = Guid.NewGuid(), Name = "T-Shirts", Description = "Casual t-shirts for all seasons", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            var category2 = new ProductCategory { Id = Guid.NewGuid(), Name = "Jeans", Description = "Durable and stylish jeans", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

            dbContext.ProductCategories.AddRange(category1, category2);

            // Seed discounts
            var discount1 = new Discount { Id = Guid.NewGuid(), Name = "Summer Sale", Description = "20% off for summer collection", DiscountPercent = 20, Active = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            var discount2 = new Discount { Id = Guid.NewGuid(), Name = "Flash Sale", Description = "50% off for limited time", DiscountPercent = 50, Active = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

            dbContext.Discounts.AddRange(discount1, discount2);

            // Seed products
            dbContext.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Basic White T-Shirt",
                    Description = "A plain white t-shirt made from 100% cotton.",
                    Price = 19.99m,
                    SKU = "TSHIRT-WHITE-001",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCategoryId = category1.Id,
                    DiscountId = discount1.Id
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Slim Fit Jeans",
                    Description = "Stylish slim fit jeans with a modern look.",
                    Price = 49.99m,
                    SKU = "JEANS-SLIM-001",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCategoryId = category2.Id,
                    DiscountId = discount2.Id
                }
            );

            dbContext.SaveChanges();
        }
    }
}