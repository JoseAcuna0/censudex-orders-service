using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace order_service.src.DTOs
{
    /// <summary>
    /// DTO utilizado para recibir los datos necesarios
    /// para la creación de una nueva orden.
    /// </summary>
    public class OrderCreateDto
    {
        /// <summary>
        /// Identificador único del cliente que realiza la orden.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Nombre del cliente que realiza la orden.
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del cliente.
        /// Utilizado para enviar notificaciones por SendGrid.
        /// </summary>
        public string CustomerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Lista de productos incluidos en la orden.
        /// Cada ítem contiene información de cantidad y precio.
        /// </summary>
        public List<OrderItemAppDto> Items { get; set; } = new List<OrderItemAppDto>();
    }
}