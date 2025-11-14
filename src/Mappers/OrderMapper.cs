using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using order_service.src.DTOs;
using order_service.src.Models;

namespace order_service.src.Mappers
{
    /// <summary>
    /// Clase encargada de mapear entidades del dominio (Order, OrderItem)
    /// hacia DTOs y viceversa. Se utiliza para separar las capas internas
    /// del microservicio del contrato expuesto al cliente.
    /// </summary>
    public static class OrderMapper
    {
        /// <summary>
        /// Convierte una entidad <see cref="Order"/> en un DTO
        /// <see cref="OrderResponseDto"/> que será devuelto al cliente
        /// a través del servicio gRPC.
        /// </summary>
        /// <param name="order">Entidad Order proveniente de la base de datos.</param>
        /// <returns>Un objeto OrderResponseDto con datos listos para ser expuestos.</returns>
        public static OrderResponseDto ToResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,

                // Conversión de cada OrderItem → OrderItemAppDto
                Items = order.Items.Select(item => new OrderItemAppDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };
        }

        /// <summary>
        /// Convierte un DTO <see cref="OrderCreateDto"/> recibido desde gRPC
        /// en una entidad <see cref="Order"/> lista para ser almacenada en la BD.
        /// Calcula automáticamente el total de la orden basado en sus ítems.
        /// </summary>
        /// <param name="dto">DTO recibido desde el cliente</param>
        /// <returns>Entidad Order que será procesada y persistida.</returns>
        public static Order ToEntity(OrderCreateDto dto)
        {
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                Status = "pendiente",
                OrderDate = DateTime.UtcNow,

                // Mapeo de DTOs → entidades OrderItem
                Items = dto.Items.Select(itemDto => new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    ProductName = itemDto.ProductName,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice
                }).ToList()
            };

            // Cálculo del total de la orden (sumatoria de cantidad * precio)
            order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

            return order;
        }
    }
}
