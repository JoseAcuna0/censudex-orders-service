using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using order_service.src.Interface;
using order_service.src.Models;

namespace order_service.src.Grpc
{
    public class OrderGrpcServiceImpl : OrderGrpcService.OrderGrpcServiceBase
    {
        private readonly IOrderService _orderService;

        public OrderGrpcServiceImpl(IOrderService orderService)
        {
            _orderService = orderService;
        }
    
        public override Task<OrderResponse> GetOrderById(OrderByIdRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, "Not implemented yet"));
        }

        public override Task<OrdersListResponse> GetAllOrders(Empty request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, "Not implemented yet"));
        }

        public override Task<OrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, "Not implemented yet"));
        }

        public override Task<OperationResult> UpdateOrderStatus(UpdateStatusRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, "Not implemented yet"));
        }

        public override Task<OperationResult> CancelOrder(OrderByIdRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, "Not implemented yet"));
        }  
    }
}