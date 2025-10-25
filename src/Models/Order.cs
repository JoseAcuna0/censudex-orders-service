using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace order_service.src.Models
{
    public class Order
    {
        public int Id { get; set; }


        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; } = 0.0m;

        public string Status { get; set; } = "Pendiente";

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}