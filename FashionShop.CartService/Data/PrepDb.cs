namespace FashionShop.CartService.Data
{
    public class PrepDb
    {
        public static void AddSeedData(IApplicationBuilder applicationBuilder)
        {
            using (var scope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CartDbContext>();


            }

        }
    }
}
