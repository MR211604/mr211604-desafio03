using Microsoft.EntityFrameworkCore;
using Pedidos.DAL.Data;
using Pedidos.DAL.Interfaces;
using Pedidos.Entities.Models;

namespace Pedidos.DAL.Repositories
{
    public class ClienteRepository(PedidosDbContext context) : Repository<Cliente>(context), IClienteRepository
    {
        public async Task<bool> ExisteEmailAsync(string email, int? excluirId = null, CancellationToken cancellationToken = default)
        {
            return await Set.AnyAsync(
                c => c.Email == email && (excluirId == null || c.Id != excluirId),
                cancellationToken);
        }
    }
}
