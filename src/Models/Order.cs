using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace order_service.src.Models
{
    /// <summary>
    /// Representa una orden realizada por un cliente dentro del sistema.
    /// Incluye datos del cliente, productos solicitados, monto total
    /// y el estado actual del procesamiento de la orden.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Identificador único de la orden.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador único del cliente que realizó la orden.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Nombre del cliente asociado a la orden.
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Correo de contacto del cliente, utilizado para notificaciones (SendGrid).
        /// </summary>
        public string CustomerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en que la orden fue registrada.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Monto total de la orden, calculado a partir de los productos.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Estado actual de la orden.
        /// Valores típicos:
        /// pendiente, En Procesamiento, Enviado, Entregado, Cancelada.
        /// </summary>
        public string Status { get; set; } = "pendiente";

        /// <summary>
        /// Colección de productos incluidos en la orden.
        /// </summary>
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}