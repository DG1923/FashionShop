using FashionShop.OrderService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//add dbcontext
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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
