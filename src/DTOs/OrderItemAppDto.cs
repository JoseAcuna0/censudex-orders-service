using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace order_service.src.DTOs
{
    /// <summary>
    /// Representa un ítem dentro de una orden, usado tanto para crear
    /// como para devolver datos de ítems asociados a una orden.
    /// </summary>
    public class OrderItemAppDto
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Nombre del producto incluido en la orden.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad solicitada del producto.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}
