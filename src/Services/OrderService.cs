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

        public OrderService(OrderDbContext context)
        {
            _context = context;
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
                return true;
            }
            
            return false;
        }

        public async Task<bool> CancelOrderAsync(Guid id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order is null) return false;
            
            order.Status = "Cancelada";
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
