using Grpc.Core;
using order_service;
using order_service.src.Interface;
using order_service.src.DTOs;

namespace order_service.src.Grpc
{
    /// <summary>
    /// Implementación de los servicios gRPC expuestos por el microservicio de Órdenes.
    /// Esta clase recibe solicitudes desde otros servicios o clientes,
    /// convierte los mensajes protobuf a DTOs de la aplicación, delega la lógica al OrderService
    /// y devuelve las respuestas mapeadas nuevamente al formato protobuf.
    /// </summary>
    public class OrderGrpcServiceImpl : OrderGrpcService.OrderGrpcServiceBase
    {
        private readonly IOrderService _orderService;

        /// <summary>
        /// Constructor que recibe la interfaz del servicio de órdenes
        /// mediante inyección de dependencias.
        /// </summary>
        public OrderGrpcServiceImpl(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Crea una nueva orden en el sistema.
        /// Convierte el mensaje gRPC en un DTO interno y delega al OrderService,
        /// además de formatear el resultado nuevamente a una respuesta protobuf.
        /// </summary>
        public override async Task<OrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            // Convertir request gRPC → DTO de aplicación
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

            // Convertir DTO → respuesta gRPC
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


        /// <summary>
        /// Obtiene una orden según su identificador único (GUID).
        /// Si la orden no existe, lanza una excepción RPC con código NOT_FOUND.
        /// </summary>
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


        /// <summary>
        /// Retorna todas las órdenes registradas en el sistema.
        /// Devuelve una lista de OrderResponse dentro de OrdersListResponse.
        /// </summary>
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


        /// <summary>
        /// Actualiza el estado de una orden según su ID.
        /// Permitido solo para estados válidos como "Enviado" o "Entregado".
        /// </summary>
        public override async Task<OperationResult> UpdateOrderStatus(UpdateStatusRequest request, ServerCallContext context)
        {
            var success = await _orderService.UpdateOrderAsync(Guid.Parse(request.Id), request.NewStatus);

            return new OperationResult
            {
                Success = success,
                Message = success 
                    ? "Estado de la orden actualizado" 
                    : $"Orden {request.Id} no encontrada o estado inválido"
            };
        }


        /// <summary>
        /// Cancela una orden, siempre que no haya sido enviada o entregada.
        /// </summary>
        public override async Task<OperationResult> CancelOrder(OrderByIdRequest request, ServerCallContext context)
        {
            var success = await _orderService.CancelOrderAsync(Guid.Parse(request.Id));

            return new OperationResult
            {
                Success = success,
                Message = success
                    ? "Orden cancelada"
                    : $"Orden {request.Id} no encontrada o no se puede cancelar"
            };
        }
    }
}
