using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using order_service.src.Models;

namespace order_service.src.Data
{
    public class OrderDbContext : DbContext 
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.HasMany(e => e.Items)
                      .WithOne()
                      .HasForeignKey("OrderId")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            });
        }
    }
}