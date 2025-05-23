﻿using FashionShop.CartService.Protos;

namespace FashionShop.CartService.SyncDataService.Grpc
{
    public interface IGrpcCartClient
    {
        IEnumerable<UserResponseToCart> GetExitUserToCreateCart();
        Task<bool> isUserExit(Guid userId);
    }
}
