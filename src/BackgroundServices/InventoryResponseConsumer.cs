using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using order_service.src.Data;
using order_service.src.Services;

namespace order_service.src.BackgroundServices
{
    /// <summary>
    /// Servicio en segundo plano encargado de consumir mensajes provenientes
    /// del microservicio de inventario a través de RabbitMQ.
    /// 
    /// Este servicio escucha la cola "inventory_response" y procesa la respuesta
    /// indicando si una orden tiene stock disponible. Según esta respuesta,
    /// actualiza el estado de la orden en la base de datos y envía un correo
    /// electrónico al cliente usando SendGrid.
    /// </summary>
    public class InventoryResponseConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        /// <summary>
        /// Constructor del consumidor que inicializa la conexión con RabbitMQ y
        /// declara la cola en la cual el microservicio de inventario publicará su respuesta.
        /// </summary>
        public InventoryResponseConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            // Configuración de la conexión a RabbitMQ
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            // Se establece la conexión y el canal
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declaración de la cola donde se recibirá la respuesta del inventario
            _channel.QueueDeclare(
                queue: "inventory_response",
                durable: true,
                exclusive: false,
                autoDelete: false
            );
        }

        /// <summary>
        /// Método principal que escucha y procesa los mensajes de inventario.
        /// Ejecutado automáticamente al iniciar el microservicio.
        /// </summary>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Consumidor del canal
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, ea) =>
            {
                // Se obtiene el mensaje desde RabbitMQ
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var data = JsonSerializer.Deserialize<InventoryResponseDto>(message)!;

                Console.WriteLine($"📩 Stock response received: {message}");

                // Creación de un scope para acceder al DbContext y SendGridService
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                // Buscar la orden correspondiente al OrderId recibido
                var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == Guid.Parse(data.OrderId));

                if (order != null)
                {
                    // Actualizar estado según stock disponible
                    order.Status = data.HasStock ? "En Procesamiento" : "Cancelada";
                    await db.SaveChangesAsync();

                    // Envío de correo al cliente notificando el resultado
                    var email = scope.ServiceProvider.GetRequiredService<SendGridService>();

                    string subject = data.HasStock
                        ? "Tu orden fue confirmada"
                        : "Tu orden fue rechazada por falta de stock";

                    string html = data.HasStock
                        ? $"<p>Hola {order.CustomerName}, tu orden <b>{order.Id}</b> fue confirmada. 🎉</p>"
                        : $"<p>Hola {order.CustomerName}, lamentamos informarte que tu orden <b>{order.Id}</b> fue rechazada por falta de stock.</p>";

                    await email.SendEmailAsync(order.CustomerEmail, subject, html);
                }

                // Confirmamos a RabbitMQ que el mensaje fue procesado correctamente
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            // Se inicia la escucha de la cola
            _channel.BasicConsume(
                queue: "inventory_response",
                autoAck: false,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        /// <summary>
        /// Libera recursos y cierra correctamente el canal y la conexión con RabbitMQ.
        /// </summary>
        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }

    /// <summary>
    /// DTO utilizado para deserializar la respuesta proveniente del microservicio de inventario.
    /// </summary>
    public class InventoryResponseDto
    {
        /// <summary>
        /// Id de la orden sobre la cual se verifica el stock.
        /// </summary>
        public string OrderId { get; set; } = string.Empty!;

        /// <summary>
        /// Indica si existe stock disponible para todos los productos de la orden.
        /// </summary>
        public bool HasStock { get; set; }
    }
}
