namespace FashionShop.UserService.SyncDataService.GrpcClient
{
    public interface ISendNewUser
    {
        Task SendNewUser(Guid userId);
    }
}
