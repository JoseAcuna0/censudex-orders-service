using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using order_service.src.DTOs;

namespace order_service.src.Interface
{
    public interface IOrderService
    {
        //Crear una nueva orden
        Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto dto);

        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();

        Task<OrderResponseDto?> GetOrderByIdAsync(Guid id);

        Task<bool> UpdateOrderAsync(Guid id, string newStatus);

        Task<bool> CancelOrderAsync(Guid id);
    }
}