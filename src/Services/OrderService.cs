using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using order_service.src.DTOs;
using order_service.src.Interface;
using order_service.src.Mappers;
using order_service.src.Models;

namespace order_service.src.Services
{
    public class OrderService : IOrderService
    {
        private readonly List<Order> _orders = new();

        public async Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto dto)
        {
            var order = OrderMapper.ToEntity(dto);

            order.Id = Guid.NewGuid();

            _orders.Add(order);

            await Task.CompletedTask;

            return OrderMapper.ToResponseDto(order);
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);

            await Task.CompletedTask;

            return order is null ? null : OrderMapper.ToResponseDto(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {

            await Task.CompletedTask;

            return _orders.Select(OrderMapper.ToResponseDto);
        }

        public async Task<bool> UpdateOrderAsync(Guid id, string newStatus)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order is null)
            {
                return false;
            }

            order.Status = newStatus;

            await Task.CompletedTask;

            return true;
        }

        public async Task<bool> CancelOrderAsync(Guid id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order is null)
            {
                return false;
            }

            order.Status = "cancelada";

            await Task.CompletedTask;

            return true;
        }
    }
}