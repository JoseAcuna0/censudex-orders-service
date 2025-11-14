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
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _context;

        private readonly SendGridService _emailManager;

        public OrderService(OrderDbContext context, SendGridService emailManager)
        {
            _context = context;
            _emailManager = emailManager;
        }

        

        public async Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto dto)
        {
            var order = OrderMapper.ToEntity(dto);
            order.Id = Guid.NewGuid();
            order.OrderDate = DateTime.UtcNow;

            foreach (var item in order.Items)
            {
                item.OrderId = order.Id;
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            using var rabbit = new RabbitMqService();
            rabbit.SendOrderToInventory(order.Id.ToString());

            var subject = "Tu orden ha sido creada";
            var html = $@"
                <h2>Hola {order.CustomerName}</h2>
                <p>Tu orden fue registrada exitosamente.</p>
                <p><b>ID:</b> {order.Id}</p>
                <p>Te avisaremos cuando confirme el inventario.</p>";

            await _emailManager.SendEmailAsync(order.CustomerEmail, subject, html);
            

            return OrderMapper.ToResponseDto(order);
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order is null ? null : OrderMapper.ToResponseDto(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ToListAsync();

            return orders.Select(OrderMapper.ToResponseDto);
        }

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
                    var htmlContent = $"<p>Estimado {order.CustomerName},</p>" +
                                      $"<p>Su orden con ID {order.Id} ha sido entregada exitosamente.</p>" +
                                      "<p>Gracias por comprar con nosotros.</p>";

                    await _emailManager.SendEmailAsync(order.CustomerEmail, subject, htmlContent);
                }

                if (newStatus == "Enviado")
                {
                    var subject = "Notificación de envío de su orden";
                    var htmlContent = $"<p>Estimado {order.CustomerName},</p>" +
                                      $"<p>Su orden con ID {order.Id} ha sido enviada.</p>" +
                                      "<p>Pronto la recibirá.</p>";

                    await _emailManager.SendEmailAsync(order.CustomerEmail, subject, htmlContent);
                }
                return true;
            }
            
            return false;
        }

        public async Task<bool> CancelOrderAsync(Guid id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order is null) return false;


            if (order.Status == "Enviado" || order.Status == "Entregado")
            {
                return false;
            }

            order.Status = "Cancelada";
            await _context.SaveChangesAsync();

            var subject = "Notificación de cancelación de su orden";
            var htmlContent = $"<p>Estimado {order.CustomerName},</p>" +
                              $"<p>Su orden con ID {order.Id} ha sido cancelada.</p>" +
                              "<p>Si tiene alguna pregunta, no dude en contactarnos.</p>";

            await _emailManager.SendEmailAsync(order.CustomerEmail, subject, htmlContent);

            return true;
        }
    }
}
