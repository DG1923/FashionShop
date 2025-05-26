
using FashionShop.ProductService.AsyncDataService;
using FashionShop.ProductService.Data;
using FashionShop.ProductService.EventProcessing;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Repo;
using FashionShop.ProductService.Repo.Interface;
using FashionShop.ProductService.Service;
using FashionShop.ProductService.Service.Interface;
using FashionShop.ProductService.Settings;
using FashionShop.ProductService.SyncDataService.GrpcClient;
using FashionShop.ProductService.SyncDataService.GrpcService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
            builder.Services.AddGrpc();

            //Register repo
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            builder.Services.AddScoped<IProductCategoryRepo, ProductCategoryRepo>();
            builder.Services.AddScoped<IProductRepo, ProductRepo>();
            builder.Services.AddScoped<IDiscountRepo, DiscountRepo>();
            builder.Services.AddScoped<IProductVariationRepo, ProductVariationRepo>();

            //Register services
            builder.Services.AddScoped(typeof(IBaseService<,,>), typeof(BaseService<,,>));
            builder.Services.AddScoped<IProductService, ProductsService>();
            builder.Services.AddScoped<IProductVariationService, ProductVariationService>();


            // Register gRPC client
            builder.Services.AddScoped<IProductProtoClient, ProductProtoClient>();

            //Add host service subcriber event
            builder.Services.AddHostedService<MessageBusSubcriber>();
            //add service to process event
            builder.Services.AddSingleton<IEventProcessor, EventProcessor>();




            //// Configure authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            builder.Services.Configure<JwtSettings>(jwtSettings);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
                    };
                });
            // Add Swagger for API documentation
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Service API", Version = "v1" });

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                });
            });


            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                //add seed data to the in-memory database
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ProductDbContext>();
                if (builder.Environment.IsProduction())
                {
                    //Try migration
                    try
                    {
                        Console.WriteLine("Migrating database...");

                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not run migrations: " + ex.Message);
                    }
                }
            }
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

                dbContext.Database.Migrate();
                // Seed data if necessary
                if (!dbContext.ProductCategories.Any() || !dbContext.Discounts.Any())
                {
                    SeedData(dbContext);
                }

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

            app.MapGrpcService<SyncQuantityService>();
            app.MapGet("/Protos/SyncQuantity.proto", async context =>
            {
                await context.Response.WriteAsync("gRPC service is running");
            });
            app.MapControllers();

            app.Run();
        }

        private static void SeedData(ProductDbContext dbContext)
        {
            // Seed Product Categories
            var category1 = new ProductCategory { Id = Guid.NewGuid(), Name = "T-Shirts", Description = "Casual t-shirts for all seasons", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            var category2 = new ProductCategory { Id = Guid.NewGuid(), Name = "Jeans", Description = "Durable and stylish jeans", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            var category3 = new ProductCategory { Id = Guid.NewGuid(), Name = "Shoes", Description = "Comfortable and stylish shoes", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

            dbContext.ProductCategories.AddRange(category1, category2, category3);

            // Seed Discounts
            var discount1 = new Discount { Id = Guid.NewGuid(), Name = "Summer Sale", Description = "20% off for summer collection", DiscountPercent = 20, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            var discount2 = new Discount { Id = Guid.NewGuid(), Name = "Flash Sale", Description = "50% off for limited time", DiscountPercent = 50, IsActive = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            var discount3 = new Discount { Id = Guid.NewGuid(), Name = "Winter Sale", Description = "30% off for winter collection", DiscountPercent = 30, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

            dbContext.Discounts.AddRange(discount1, discount2, discount3);

            // Seed Products
            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Basic White T-Shirt",
                Description = "A plain white t-shirt made from 100% cotton.",
                BasePrice = 19.99m,
                SKU = "TSHIRT-WHITE-001",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ProductCategoryId = category1.Id,
                DiscountId = discount1.Id
            };
            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Slim Fit Jeans",
                Description = "Stylish slim fit jeans with a modern look.",
                BasePrice = 49.99m,
                SKU = "JEANS-SLIM-001",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ProductCategoryId = category2.Id,
                DiscountId = discount2.Id
            };
            var product3 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Running Shoes",
                Description = "Lightweight running shoes for daily use.",
                BasePrice = 69.99m,
                SKU = "SHOES-RUN-001",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ProductCategoryId = category3.Id,
                DiscountId = discount3.Id
            };

            dbContext.Products.AddRange(product1, product2, product3);

            // Seed Product Colors
            var color1 = new ProductColor
            {
                Id = Guid.NewGuid(),
                ColorName = "White",
                ColorCode = "#FFFFFF",
                ImageUrlColor = "https://example.com/images/white.png",
                TotalQuantityColor = 100,
                ProductId = product1.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var color2 = new ProductColor
            {
                Id = Guid.NewGuid(),
                ColorName = "Blue",
                ColorCode = "#0000FF",
                ImageUrlColor = "https://example.com/images/blue.png",
                TotalQuantityColor = 50,
                ProductId = product2.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var color3 = new ProductColor
            {
                Id = Guid.NewGuid(),
                ColorName = "Black",
                ColorCode = "#000000",
                ImageUrlColor = "https://example.com/images/black.png",
                TotalQuantityColor = 30,
                ProductId = product3.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            dbContext.ProductColors.AddRange(color1, color2, color3);

            // Seed Product Variations
            var variation1 = new ProductVariation
            {
                Id = Guid.NewGuid(),

                Size = "M",
                Description = "Medium size white t-shirt",
                Quantity = 50,
                ProductColorId = color1.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var variation2 = new ProductVariation
            {
                Id = Guid.NewGuid(),

                Size = "32",
                Description = "Blue slim fit jeans, size 32",
                Quantity = 30,
                ProductColorId = color2.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var variation3 = new ProductVariation
            {
                Id = Guid.NewGuid(),

                Size = "10",
                Description = "Black running shoes, size 10",
                Quantity = 20,
                ProductColorId = color3.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            dbContext.ProductVariations.AddRange(variation1, variation2, variation3);

            // Save changes to the database
            dbContext.SaveChanges();
        }
    }
}