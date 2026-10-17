using Pedidos.Entities.Models;

namespace Pedidos.DAL.Interfaces
{
    public interface IPedidoRepository : IRepository<Pedido>
    {
        Task<Pedido?> GetByIdConDetallesAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Pedido>> GetAllConDetallesAsync(CancellationToken cancellationToken = default);

        Task<bool> ExistenPedidosParaClienteAsync(int clienteId, CancellationToken cancellationToken = default);

        Task<bool> ExistenPedidosParaProductoAsync(int productoId, CancellationToken cancellationToken = default);
    }
}
