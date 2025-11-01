using order_service.src.Services;
using order_service.src.Interface;
using order_service.src.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Register gRPC
builder.Services.AddGrpc();

// Register your domain service (currently in-memory)
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Map the gRPC service
app.MapGrpcService<OrderGrpcServiceImpl>();

app.Run();
