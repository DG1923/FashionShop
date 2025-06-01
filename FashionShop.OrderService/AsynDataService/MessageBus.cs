using FashionShop.OrderService.Model;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FashionShop.OrderService.AsynDataService
{
    public class MessageBus : IMessageBus
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBus(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"]),

            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(
                 exchange: "trigger",
                 type: ExchangeType.Fanout,
                 durable: true
                 );
                _channel.QueueDeclare(
                    queue: "product_quantity_update",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );
                _channel.QueueBind(
                    queue: "product_quantity_update",
                    exchange: "trigger",
                    routingKey: ""
                    );
                Console.WriteLine("--> RabbitMQ Connection Opened");
                _connection.ConnectionShutdown += (sender, e) =>
                {
                    Console.WriteLine("--> RabbitMQ Connection Shutdown");
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to RabbitMQ: {ex.Message}");
            }

        }

        public void PublishOrderCreated(OrderMessage message)
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
                exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body);
            Console.WriteLine($"--> Message Sent: {message}");
        }
        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }


    public class OrderMessage
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public List<OrderItem> Items { get; set; }
        public string Event { get; set; }
    }
}

