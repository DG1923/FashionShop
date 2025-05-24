
using FashionShop.InventoryService.AsynDataService;
using FashionShop.InventoryService.Data;
using FashionShop.InventoryService.Repository;
using FashionShop.InventoryService.Repository.Interface;
using FashionShop.InventoryService.Services;
using FashionShop.InventoryService.Services.Interface;
using FashionShop.InventoryService.Settings;
using FashionShop.InventoryService.SyncDataService.Grpc.GrpcClient;
using FashionShop.InventoryService.SyncDataService.Grpc.GrpcService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace FashionShop.InventoryService
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

            //add service GRPC
            builder.Services.AddGrpc();

            //add connection to db
            builder.Services.AddDbContext<InventoryDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            //add service
            builder.Services.AddScoped<IInventoryRepo, InventoryRepo>();
            builder.Services.AddScoped<IInventoryService, Services.InventoryService>();
            builder.Services.AddScoped<ISyncQuantityClient, SyncQuantityClient>();

            //add services for RabbitMQ 
            builder.Services.AddSingleton<IMessageBus, MessageBus>();

            //add configuration and service for jwt
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
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Authentication failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated for: " + context.Principal.Identity.Name);
                        return Task.CompletedTask;
                    }
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

            //add service for cache
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                string redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
                options.Configuration = redisConnectionString;
            });
            builder.Services.AddSingleton<RedisCacheManager>();
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                //add seed data to the in-memory database
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<InventoryDBContext>();
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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            //Configure the gRPC service
            app.MapGrpcService<ProductProtoService>();
            app.MapGrpcService<CartProtoServer>();
            app.MapGet("/Protos/ProductProto.proto", async context =>
            {
                await context.Response.WriteAsync(File.ReadAllText("Proto/ProductProto.proto"));
            });
            if (app.Environment.IsProduction())
            {
                PrepDb.PrepareQuantity(app);
            }

            app.MapControllers();

            app.Run();
        }
    }
}
