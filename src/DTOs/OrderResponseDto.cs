using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace order_service.src.DTOs
{
    /// <summary>
    /// Representa la respuesta estándar que entrega el Order Service
    /// cuando una orden es consultada, creada o listada.
    /// </summary>
    public class OrderResponseDto
    {
        /// <summary>
        /// Identificador único de la orden.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del cliente que realizó la orden.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Nombre del cliente asociado a la orden.
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en que la orden fue registrada.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Monto total calculado en base a los ítems de la orden.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Estado actual de la orden 
        /// (pendiente, en procesamiento, confirmada, enviada, entregada, cancelada, etc.).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Lista de productos incluidos dentro de la orden.
        /// </summary>
        public List<OrderItemAppDto> Items { get; set; } = new List<OrderItemAppDto>();
    }
}