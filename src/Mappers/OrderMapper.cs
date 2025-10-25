using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using order_service.src.DTOs;
using order_service.src.Models;

namespace order_service.src.Mappers
{
    public static class OrderMapper
    {
        public static OrderResponseDto ToDto(this Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };
        }

        public static Order ToEntity(OrderCreateDto dto)
        {
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                CustomerName = dto.CustomerName,
                Status = "pendiente",
                OrderDate = DateTime.UtcNow,
                Items = dto.Items.Select(itemDto => new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    ProductName = itemDto.ProductName,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice
                }).ToList()
            };

            order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

            return order;
        }
    }
}