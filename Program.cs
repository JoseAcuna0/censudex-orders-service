using order_service.src.Services;
using order_service.src.Interface;
using order_service.src.Grpc;
using order_service.src.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using order_service.src.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SendGridService>();

builder.Services.AddHostedService<InventoryResponseConsumer>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5247, o => o.Protocols = HttpProtocols.Http2);
});

builder.Services.AddGrpc();

builder.Services.AddScoped<IOrderService, OrderService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

var app = builder.Build();

app.MapGrpcService<OrderGrpcServiceImpl>();

app.Run();
