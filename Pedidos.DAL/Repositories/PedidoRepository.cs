using Microsoft.EntityFrameworkCore;
using Pedidos.DAL.Data;
using Pedidos.DAL.Interfaces;
using Pedidos.Entities.Models;

namespace Pedidos.DAL.Repositories
{
    public class PedidoRepository(PedidosDbContext context) : Repository<Pedido>(context), IPedidoRepository
    {
        public async Task<Pedido?> GetByIdConDetallesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await Set
                .AsNoTracking()
                .Include(p => p.Cliente)
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Pedido>> GetAllConDetallesAsync(CancellationToken cancellationToken = default)
        {
            return await Set
                .AsNoTracking()
                .Include(p => p.Cliente)
                .Include(p => p.Producto)
                .OrderBy(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistenPedidosParaClienteAsync(int clienteId, CancellationToken cancellationToken = default)
        {
            return await Set.AnyAsync(p => p.ClienteId == clienteId, cancellationToken);
        }

        public async Task<bool> ExistenPedidosParaProductoAsync(int productoId, CancellationToken cancellationToken = default)
        {
            return await Set.AnyAsync(p => p.ProductoId == productoId, cancellationToken);
        }
    }
}
