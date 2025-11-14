using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using order_service.src.DTOs;

namespace order_service.src.Interface
{
    /// <summary>
    /// Define las operaciones principales para la gestión de órdenes
    /// dentro del microservicio Order Service.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Crea una nueva orden en el sistema, calcula el monto total,
        /// registra los ítems y envía la orden al servicio de inventario
        /// mediante RabbitMQ.
        /// </summary>
        /// <param name="dto">Datos necesarios para crear la orden.</param>
        /// <returns>Un DTO con la información de la orden creada.</returns>
        Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto dto);

        /// <summary>
        /// Obtiene la lista completa de órdenes registradas en el sistema.
        /// </summary>
        /// <returns>Una colección de órdenes en formato DTO.</returns>
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();

        /// <summary>
        /// Obtiene una orden específica según su identificador único.
        /// </summary>
        /// <param name="id">ID de la orden a consultar.</param>
        /// <returns>
        /// Un DTO con los datos de la orden, o null si no existe.
        /// </returns>
        Task<OrderResponseDto?> GetOrderByIdAsync(Guid id);

        /// <summary>
        /// Actualiza el estado de una orden. Solo se permiten ciertos estados,
        /// como "Enviado" y "Entregado". Si corresponde, envía notificación por email.
        /// </summary>
        /// <param name="id">ID de la orden a actualizar.</param>
        /// <param name="newStatus">Nuevo estado de la orden.</param>
        /// <returns>True si se actualizó, False si no existe o no es válido.</returns>
        Task<bool> UpdateOrderAsync(Guid id, string newStatus);

        /// <summary>
        /// Cancela una orden, siempre que no haya sido enviada o entregada.
        /// Envía una notificación por email al cliente.
        /// </summary>
        /// <param name="id">ID de la orden a cancelar.</param>
        /// <returns>True si se canceló correctamente, False en caso contrario.</returns>
        Task<bool> CancelOrderAsync(Guid id);
    }
}