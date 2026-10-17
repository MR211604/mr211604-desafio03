using Microsoft.EntityFrameworkCore;
using Pedidos.Entities.Models;

namespace Pedidos.DAL.Data
{
    public class PedidosDbContext(DbContextOptions<PedidosDbContext> options) : DbContext(options)
    {
        public DbSet<Cliente> Clientes => Set<Cliente>();

        public DbSet<Producto> Productos => Set<Producto>();

        public DbSet<Pedido> Pedidos => Set<Pedido>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes");
                entity.HasIndex(c => c.Email).IsUnique();
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.Property(p => p.Precio).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.ToTable("Pedidos");

                entity.HasOne(p => p.Cliente)
                    .WithMany(c => c.Pedidos)
                    .HasForeignKey(p => p.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Producto)
                    .WithMany(pr => pr.Pedidos)
                    .HasForeignKey(p => p.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
