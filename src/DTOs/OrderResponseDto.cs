using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace order_service.src.DTOs
{
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemAppDto> Items { get; set; } = new List<OrderItemAppDto>();
    }
}