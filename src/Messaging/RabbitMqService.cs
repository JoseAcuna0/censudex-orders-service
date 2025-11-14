using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;

namespace order_service.src.Messaging
{
    /// <summary>
    /// Servicio responsable de manejar la comunicación del Order Service
    /// hacia RabbitMQ. Este servicio envía mensajes a la cola
    /// <c>inventory_check</c>, utilizados para solicitar validación de stock
    /// al microservicio de inventario.
    /// </summary>
    public class RabbitMqService : IDisposable
    {
        /// <summary>
        /// Conexión activa hacia el broker RabbitMQ.
        /// </summary>
        private readonly IConnection _connection = null!;

        /// <summary>
        /// Canal lógico utilizado para publicar mensajes en RabbitMQ.
        /// </summary>
        private readonly IModel _channel = null!;

        /// <summary>
        /// Inicializa una nueva instancia del servicio, configurando la conexión
        /// y declarando la cola <c>inventory_check</c> si no existe.
        /// </summary>
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

            // Declaración de la cola donde se enviarán las solicitudes de stock
            _channel.QueueDeclare(
                queue: "inventory_check",
                durable: true,
                exclusive: false,
                autoDelete: false
            );
        }

        /// <summary>
        /// Publica el ID de la orden en la cola <c>inventory_check</c>.
        /// Este mensaje será consumido por el microservicio de inventario,
        /// quien determinará si existe stock disponible.
        /// </summary>
        /// <param name="orderId">ID de la orden recién creada.</param>
        public void SendOrderToInventory(string orderId)
        {
            var body = Encoding.UTF8.GetBytes(orderId);

            _channel.BasicPublish(
                exchange: "",
                routingKey: "inventory_check",
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"Sent order to inventory check: {orderId}");
        }

        /// <summary>
        /// Libera la conexión y el canal asociados a RabbitMQ.
        /// </summary>
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}