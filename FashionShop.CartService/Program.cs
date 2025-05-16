using FashionShop.CartService.AsyncDataServices;
using FashionShop.CartService.Data;
using FashionShop.CartService.Repo;
using FashionShop.CartService.Repo.Interface;
using FashionShop.CartService.Service;
using FashionShop.CartService.Service.Interface;
using FashionShop.CartService.SyncDataService.Grpc;
using FashionShop.CartService.SyncDataService.GrpcServer;
using FashionShop.InventoryService.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    string redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
    options.Configuration = redisConnectionString;
});

// Add gRPC
builder.Services.AddGrpc();
builder.Services.AddScoped<IGrpcCartClient, GrpcCartClient>();

// Add service for Repositories
builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
builder.Services.AddScoped<ICartRepo, CartRepo>();
builder.Services.AddScoped<ICartItemRepo, CartItemRepo>();

//Add service for Services
builder.Services.AddScoped(typeof(IBaseService<,,>), typeof(BaseService<,,>));
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartItemService, CartItemService>();

// Add RabbitMQ Message Bus
builder.Services.AddSingleton<IMessageBus, MessageBus>();

// Add Redis Cache Service
builder.Services.AddSingleton<IRedisCacheManager, RedisCacheManager>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
await PrepDb.AddSeedData(app);
app.MapGrpcService<GrpcCreateCart>();
app.MapControllers();

app.Run();
