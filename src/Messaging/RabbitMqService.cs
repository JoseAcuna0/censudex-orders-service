using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;

namespace order_service.src.Messaging
{
    public class RabbitMqService : IDisposable
    {

        private readonly IConnection _connection = null!;
        private readonly IModel _channel = null!;

        public RabbitMqService()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare("inventory_check", durable: true, exclusive: false, autoDelete: false);
        }

        public void SendOrderToInventory(string orderId)
        {
            var body = Encoding.UTF8.GetBytes(orderId);
            _channel.BasicPublish("", "inventory_check", null, body);
            Console.WriteLine($"Sent order to inventory check: {orderId}");
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
        
    }
}