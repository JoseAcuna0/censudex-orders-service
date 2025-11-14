using Grpc.Core;
using order_service;
using order_service.src.Interface;
using order_service.src.DTOs;

namespace order_service.src.Grpc
{
    public class OrderGrpcServiceImpl : OrderGrpcService.OrderGrpcServiceBase
    {
        private readonly IOrderService _orderService;

        public OrderGrpcServiceImpl(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public override async Task<OrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            var dto = new OrderCreateDto
            {
                CustomerId = Guid.Parse(request.CustomerId),
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                Items = request.Items.Select(i => new OrderItemAppDto
                {
                    ProductId = Guid.Parse(i.ProductId),
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = (decimal)i.UnitPrice
                }).ToList()
            };

            var result = await _orderService.CreateOrderAsync(dto);


            var response = new OrderResponse
            {
                Id = result.Id.ToString(),
                CustomerId = result.CustomerId.ToString(),
                CustomerName = result.CustomerName,
                OrderDate = result.OrderDate.ToString("O"),
                TotalAmount = (double)result.TotalAmount,
                Status = result.Status
            };

            response.Items.AddRange(
                result.Items.Select(x => new OrderItemDto
                {
                    ProductId = x.ProductId.ToString(),
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    UnitPrice = (double)x.UnitPrice
                })
            );

            return response;
        }

        public override async Task<OrderResponse> GetOrderById(OrderByIdRequest request, ServerCallContext context)
        {
            var result = await _orderService.GetOrderByIdAsync(Guid.Parse(request.Id));

            if (result is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Orden {request.Id} no encontrada"));

            var response = new OrderResponse
            {
                Id = result.Id.ToString(),
                CustomerId = result.CustomerId.ToString(),
                CustomerName = result.CustomerName,
                OrderDate = result.OrderDate.ToString("O"),
                TotalAmount = (double)result.TotalAmount,
                Status = result.Status
            };

            response.Items.AddRange(
                result.Items.Select(x => new OrderItemDto
                {
                    ProductId = x.ProductId.ToString(),
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    UnitPrice = (double)x.UnitPrice
                })
            );

            return response;
        }

        public override async Task<OrdersListResponse> GetAllOrders(Empty request, ServerCallContext context)
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var response = new OrdersListResponse();

            foreach (var order in orders)
            {
                var orderResponse = new OrderResponse
                {
                    Id = order.Id.ToString(),
                    CustomerId = order.CustomerId.ToString(),
                    CustomerName = order.CustomerName,
                    OrderDate = order.OrderDate.ToString("O"),
                    TotalAmount = (double)order.TotalAmount,
                    Status = order.Status
                };

                orderResponse.Items.AddRange(
                    order.Items.Select(i => new OrderItemDto
                    {
                        ProductId = i.ProductId.ToString(),
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = (double)i.UnitPrice
                    })
                );

                response.Orders.Add(orderResponse);
            }

            return response;
        }

        public override async Task<OperationResult> UpdateOrderStatus(UpdateStatusRequest request, ServerCallContext context)
        {
            var success = await _orderService.UpdateOrderAsync(Guid.Parse(request.Id), request.NewStatus);

            return new OperationResult
            {
                Success = success,
                Message = success ? "Estado de la orden actualizado" : $"Orden {request.Id} no encontrada o estado inválido"
            };
        }

        public override async Task<OperationResult> CancelOrder(OrderByIdRequest request, ServerCallContext context)
        {
            var success = await _orderService.CancelOrderAsync(Guid.Parse(request.Id));

            return new OperationResult
            {
                Success = success,
                Message = success ? "Orden cancelada" : $"Orden {request.Id} no encontrada o no se puede cancelar (Orden ya enviada/entregada)"
            };
        }
    }
}