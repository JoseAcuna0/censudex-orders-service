using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace order_service.src.Models
{
    /// <summary>
    /// Representa un ítem dentro de una orden.
    /// Cada ítem corresponde a un producto comprado por el cliente,
    /// incluyendo cantidad, precio unitario y subtotal calculado.
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Identificador numérico del ítem dentro de la tabla.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador de la orden a la cual pertenece este ítem.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Referencia a la entidad Order para navegación en EF Core.
        /// </summary>
        public Order Order { get; set; } = null!;

        /// <summary>
        /// Identificador del producto asociado al ítem.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Nombre descriptivo del producto.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad solicitada del producto.
        /// </summary>
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Precio unitario del producto al momento de la compra.
        /// </summary>
        public decimal UnitPrice { get; set; } = 0.0m;

        /// <summary>
        /// Subtotal calculado: cantidad x precio unitario.
        /// </summary>
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}