
using FashionShop.CartService.Data;
using FashionShop.CartService.Models;
using FashionShop.CartService.Repo.Interface;
using Microsoft.Extensions.Caching.Distributed;

namespace FashionShop.CartService.Repo
{
    public class CartRepo : GenericRepo<Cart>, ICartRepo
    {
        public CartRepo(CartDbContext context, IDistributedCache cache) : base(context, cache)
        {
        }
    }

}
