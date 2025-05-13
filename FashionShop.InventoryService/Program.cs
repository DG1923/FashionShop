
using FashionShop.InventoryService.Data;
using FashionShop.InventoryService.Repository;
using FashionShop.InventoryService.Repository.Interface;
using FashionShop.InventoryService.Services.Interface;
using FashionShop.InventoryService.SyncDataService.Grpc.GrpcClient;
using FashionShop.InventoryService.SyncDataService.Grpc.GrpcService;
using FashionShop.ProductService.Protos;
using Microsoft.EntityFrameworkCore;

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


            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            //Configure the gRPC service
            app.MapGrpcService<ProductProtoService>();
            app.MapGet("/Protos/ProductProto.proto", async context =>
            {
                await context.Response.WriteAsync(File.ReadAllText("Proto/ProductProto.proto"));
            });

            PrepDb.PrepareQuantity(app);
            app.MapControllers();

            app.Run();
        }
    }
}
