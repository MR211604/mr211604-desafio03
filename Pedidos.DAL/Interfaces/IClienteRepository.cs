using Pedidos.Entities.Models;

namespace Pedidos.DAL.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        Task<bool> ExisteEmailAsync(string email, int? excluirId = null, CancellationToken cancellationToken = default);
    }
}
