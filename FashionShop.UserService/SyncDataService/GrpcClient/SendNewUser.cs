using FashionShop.CartService.Protos;
using Grpc.Net.Client;

namespace FashionShop.UserService.SyncDataService.GrpcClient
{
    public class SendNewUser : ISendNewUser
    {
        private readonly IConfiguration _configuration;

        public SendNewUser(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Sends a new user ID to the CartService via gRPC to create a new cart for the user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to send to the CartService.</param>
        /// <returns>A completed Task once the operation is finished.</returns>
        /// <remarks>
        /// This method retrieves the gRPC address from the configuration, creates a gRPC channel, 
        /// and sends a request to the CartService to create a new cart for the specified user.
        /// If the operation fails, it logs an error message to the console.
        /// </remarks>
        Task ISendNewUser.SendNewUser(Guid userId)
        {
            var grpcAddress = _configuration["GrpcSendNewUser"];
            var id = new UserRequestToCart
            {
                UserId = userId.ToString()
            };

            try
            {
                var channel = GrpcChannel.ForAddress(grpcAddress);
                var client = new CreateNewCartFromNewUser.CreateNewCartFromNewUserClient(channel);
                var request = client.CreateNewCart(id);
                var response = request.Result;
                if (response == false)
                {
                    Console.WriteLine($"--> Failed to create new cart for user: {userId}");
                }
                else
                {
                    Console.WriteLine($"--> Created new cart for user: {userId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error while sending new user to CartService: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
