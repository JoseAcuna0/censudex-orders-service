using order_service.src.Services;
using order_service.src.Interface;
using order_service.src.Grpc;
using order_service.src.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register gRPC
builder.Services.AddGrpc();

// Register your domain service (currently in-memory)
builder.Services.AddScoped<IOrderService, OrderService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

var app = builder.Build();

// Map the gRPC service
app.MapGrpcService<OrderGrpcServiceImpl>();

app.Run();
