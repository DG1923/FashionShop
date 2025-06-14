﻿using FashionShop.InventoryService.DTOs;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FashionShop.InventoryService.AsynDataService
{
    public class MessageBus : IMessageBus
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;   //user rabbitmq.client 6.2.2 , if it is latest which doesn't work

        public MessageBus(IConfiguration configuration)
        {
            _configuration = configuration;
            //setup and connect to the message bus
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"]),

            };
            //create a connection
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
                _connection.ConnectionShutdown += (sender, e) =>
                {
                    Console.WriteLine("Connection shutdown");
                };
                Console.WriteLine("Connected to RabbitMQ");

            }
            catch (Exception ex)
            {
                throw new Exception("Could not connect to RabbitMQ", ex);
            }
        }
        public void PublishUpdateQuantity(PublishInventoryDto publishInventoryDto)
        {
            var message = JsonSerializer.Serialize(publishInventoryDto);
            if (_connection.IsOpen)
            {
                Console.WriteLine("RabbitMQ connection is open,sending message ...");
                SendMessage(message);
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true; // Make message persistent

            _channel.BasicPublish(
                exchange: "trigger",
                routingKey: "",
                basicProperties: properties,
                body: body
            );
            Console.WriteLine("Message sent to RabbitMQ");
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
}
