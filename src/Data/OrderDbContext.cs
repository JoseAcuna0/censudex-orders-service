using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using order_service.src.Models;

namespace order_service.src.Data
{
    /// <summary>
    /// Representa el contexto de base de datos para el microservicio de órdenes.
    /// Este DbContext administra las entidades Order y OrderItem,
    /// y define la configuración de sus relaciones dentro del modelo de datos.
    /// </summary>
    public class OrderDbContext : DbContext
    {
        /// <summary>
        /// Constructor que recibe las opciones del DbContext (cadena de conexión, proveedor, etc.)
        /// y las pasa a la clase base de Entity Framework Core.
        /// </summary>
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Tabla que almacena las órdenes creadas por los clientes.
        /// </summary>
        public DbSet<Order> Orders { get; set; } = null!;

        /// <summary>
        /// Tabla que almacena los ítems asociados a cada orden.
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        /// <summary>
        /// Método utilizado para configurar las entidades y relaciones dentro del modelo.
        /// Se ejecuta automáticamente al construir el modelo de base de datos.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de entidad: ORDER
            modelBuilder.Entity<Order>(entity =>
            {
                // Clave primaria
                entity.HasKey(e => e.Id);

                // Nombre del cliente requerido, máx 100 caracteres
                entity.Property(e => e.CustomerName)
                      .IsRequired()
                      .HasMaxLength(100);

                // Estado de la orden requerido, máx 50 caracteres
                entity.Property(e => e.Status)
                      .IsRequired()
                      .HasMaxLength(50);
            });

            // Configuración de entidad: ORDER ITEM
            modelBuilder.Entity<OrderItem>(entity =>
            {
                // Clave primaria
                entity.HasKey(e => e.Id);

                // Nombre del producto requerido, máx 100 caracteres
                entity.Property(e => e.ProductName)
                      .IsRequired()
                      .HasMaxLength(100);

                // Precio configurado como decimal(18,2)
                entity.Property(e => e.UnitPrice)
                      .HasColumnType("decimal(18,2)");

                // Relación 1:N (Order -> OrderItem)
                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.Items)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade); 
                // Si se elimina una orden, se eliminan automáticamente sus ítems
            });
        }
    }
}
