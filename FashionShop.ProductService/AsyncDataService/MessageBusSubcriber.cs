using FashionShop.ProductService.EventProcessing;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FashionShop.ProductService.AsyncDataService
{
    public class MessageBusSubcriber:BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessing;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubcriber(IConfiguration configuration, IEventProcessor eventProcessing)
        {
            _configuration = configuration;
            _eventProcessing = eventProcessing;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
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
                    type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(
                    queue: _queueName,
                    exchange: "trigger",
                    routingKey: "");
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

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                Console.WriteLine("-->Event Received");
                var body = ea.Body.ToArray();
                var notificationMessage = Encoding.UTF8.GetString(body);
                _eventProcessing.ProcessEvent(notificationMessage);
            };
            _channel.BasicConsume(
                queue: _queueName,
                autoAck: true,
                consumer: consumer);
            return Task.CompletedTask;  
        }
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
