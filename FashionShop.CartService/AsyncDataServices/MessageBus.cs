using FashionShop.CartService.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FashionShop.CartService.AsyncDataServices
{
    public interface IMessageBus
    {
        void PublishCartUpdated(CartUpdateMessage message);
    }

    public class MessageBus : IMessageBus
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBus(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare("cart_exchange", ExchangeType.Fanout);

                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }

        public void PublishCartUpdated(CartUpdateMessage message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(jsonMessage);
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "cart_exchange",
                routingKey: "",
                basicProperties: null,
                body: body);
            Console.WriteLine($"--> Message Sent: {message}");
        }
    }

    public class CartUpdateMessage
    {
        public string UserId { get; set; }
        public List<CartItem> Items { get; set; }
    }
}