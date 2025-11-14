using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using order_service.src.Data;
using order_service.src.DTOs;
using order_service.src.Interface;
using order_service.src.Mappers;
using order_service.src.Models;
using order_service.src.Messaging;

namespace order_service.src.Services
{
    /// <summary>
    /// Servicio encargado de toda la lógica de negocio relacionada con las órdenes.
    /// Gestiona creación, consulta, actualización y cancelación de órdenes.
    /// Además integra RabbitMQ para validación de stock y SendGrid para notificaciones por correo.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly SendGridService _emailManager;

        /// <summary>
        /// Constructor del OrderService.
        /// </summary>
        /// <param name="context">DbContext para acceso a la base de datos.</param>
        /// <param name="emailManager">Servicio encargado de enviar correos con SendGrid.</param>
        public OrderService(OrderDbContext context, SendGridService emailManager)
        {
            _context = context;
            _emailManager = emailManager;
        }


        /// <summary>
        /// Crea una nueva orden, la almacena en base de datos,
        /// y la envía al microservicio de Inventario vía RabbitMQ para validar stock.
        /// También envía un correo inicial al cliente notificando la creación.
        /// </summary>
        /// <param name="dto">Datos de la orden recibidos por gRPC.</param>
        /// <returns>Orden creada en formato DTO.</returns>
        public async Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto dto)
        {
            // Mapear DTO → Entidad
            var order = OrderMapper.ToEntity(dto);
            order.Id = Guid.NewGuid();
            order.OrderDate = DateTime.UtcNow;

            // Establecer relación correcta con los items
            foreach (var item in order.Items)
            {
                item.OrderId = order.Id;
            }

            // Guardar en la base de datos
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // Enviar mensaje a RabbitMQ para validar stock
            using var rabbit = new RabbitMqService();
            rabbit.SendOrderToInventory(order.Id.ToString());

            // Enviar correo de confirmación inicial
            var subject = "Tu orden ha sido creada";
            var html = $@"
                <h2>Hola {order.CustomerName}</h2>
                <p>Tu orden fue registrada exitosamente.</p>
                <p><b>ID:</b> {order.Id}</p>
                <p>Te avisaremos cuando confirme el inventario.</p>";

            await _emailManager.SendEmailAsync(order.CustomerEmail, subject, html);

            return OrderMapper.ToResponseDto(order);
        }

        /// <summary>
        /// Obtiene una orden mediante su ID y devuelve su información completa.
        /// </summary>
        public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order is null ? null : OrderMapper.ToResponseDto(order);
        }

        /// <summary>
        /// Retorna una lista con todas las órdenes existentes.
        /// </summary>
        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ToListAsync();

            return orders.Select(OrderMapper.ToResponseDto);
        }

        /// <summary>
        /// Actualiza el estado de una orden a “Enviado” o “Entregado”.
        /// Además envía un correo notificando el cambio de estado.
        /// </summary>
        /// <returns>true si se actualizó; false si no existe o estado inválido.</returns>
        public async Task<bool> UpdateOrderAsync(Guid id, string newStatus)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order is null) return false;

            if (newStatus == "Enviado" || newStatus == "Entregado")
            {
                order.Status = newStatus;
                await _context.SaveChangesAsync();

                if (newStatus == "Entregado")
                {
                    var subject = "Notificación de entrega de su orden";
                    var htmlContent =
                        $"<p>Estimado {order.CustomerName},</p>" +
                        $"<p>Su orden con ID {order.Id} ha sido entregada exitosamente.</p>" +
                        "<p>Gracias por comprar con nosotros.</p>";

                    await _emailManager.SendEmailAsync(order.CustomerEmail, subject, htmlContent);
                }

                if (newStatus == "Enviado")
                {
                    var subject = "Notificación de envío de su orden";
                    var htmlContent =
                        $"<p>Estimado {order.CustomerName},</p>" +
                        $"<p>Su orden con ID {order.Id} ha sido enviada.</p>" +
                        "<p>Pronto la recibirá.</p>";

                    await _emailManager.SendEmailAsync(order.CustomerEmail, subject, htmlContent);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Cancela una orden, siempre y cuando no haya sido enviada o entregada.
        /// Envía un correo notificando la cancelación al cliente.
        /// </summary>
        public async Task<bool> CancelOrderAsync(Guid id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order is null) return false;

            if (order.Status == "Enviado" || order.Status == "Entregado")
                return false;

            order.Status = "Cancelada";
            await _context.SaveChangesAsync();

            var subject = "Notificación de cancelación de su orden";
            var htmlContent =
                $"<p>Estimado {order.CustomerName},</p>" +
                $"<p>Su orden con ID {order.Id} ha sido cancelada.</p>" +
                "<p>Si tiene alguna pregunta, no dude en contactarnos.</p>";

            await _emailManager.SendEmailAsync(order.CustomerEmail, subject, htmlContent);

            return true;
        }
    }
}
