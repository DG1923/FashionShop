using FashionShop.ProductService.EventProcessing;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FashionShop.ProductService.AsyncDataService
{
    // Background service to subscribe to RabbitMQ messages and process events
    public class MessageBusSubcriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessing;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        // Constructor: inject configuration and event processor
        public MessageBusSubcriber(IConfiguration configuration, IEventProcessor eventProcessing)
        {
            _configuration = configuration;
            _eventProcessing = eventProcessing;
            InitializeRabbitMQ();
        }

        // Set up RabbitMQ connection, exchange, and queue
        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"]),
            };
            try
            {
                // Create connection and channel
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare a durable fanout exchange named "trigger"
                _channel.ExchangeDeclare(
                    exchange: "trigger",
                    type: ExchangeType.Fanout,
                    durable: true);

                // Declare a durable, non-exclusive, non-auto-delete queue
                _queueName = "product_quantity_update";
                _channel.QueueDeclare(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // Bind the queue to the exchange
                _channel.QueueBind(
                    queue: _queueName,
                    exchange: "trigger",
                    routingKey: "");

                // Handle connection shutdown event
                _connection.ConnectionShutdown += (sender, e) =>
                {
                    Console.WriteLine("Connection Shutdown");
                };
                Console.WriteLine("-->Listening on the Message Bus...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"-->Could not connect to the Message Bus: {ex.Message}");
            }
        }

        // Start consuming messages from the queue
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            // Create a consumer for the channel
            var consumer = new EventingBasicConsumer(_channel);

            // Event handler for received messages
            consumer.Received += (model, ea) =>
            {
                Console.WriteLine("-->Event Received");
                var body = ea.Body.ToArray();
                var notificationMessage = Encoding.UTF8.GetString(body);

                // Process the received event
                _eventProcessing.ProcessEvent(notificationMessage);
            };

            // Start consuming messages from the named queue
            _channel.BasicConsume(
                queue: _queueName,
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }

        // Clean up resources on dispose
        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }
    }
}
