using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace order_service.src.DTOs
{
    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}