using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace order_service.src.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; } = 1;

        public decimal UnitPrice { get; set; } = 0.0m;

        public decimal TotalPrice
        {
            get { return Quantity * UnitPrice; }
        }
    }
}