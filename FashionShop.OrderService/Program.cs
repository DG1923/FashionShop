using FashionShop.OrderService.Data;
using FashionShop.OrderService.Model;
using FashionShop.OrderService.Repo;
using FashionShop.OrderService.Repo.Interface;
using FashionShop.OrderService.Service;
using FashionShop.OrderService.Service.Interface;
using FashionShop.ProductService.Repo.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add caching services
builder.Services.AddStackExchangeRedisCache(options =>
{
    string redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
    options.Configuration = redisConnectionString;
});

//add dbcontext
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//add service for repositories
builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderItemRepo, OrderItemRepo>();
builder.Services.AddScoped<IPaymentDetailRepo, PaymentDetailRepo>();
builder.Services.AddScoped(typeof
    (IRedisCacheManager<>), typeof(RedisCacheManager<>));


//add service for services
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IPaymentDetailService, PaymentDetailService>();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    //add seed data to the in-memory database
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<OrderDbContext>();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
