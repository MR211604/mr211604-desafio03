using Pedidos.DAL.Data;
using Pedidos.DAL.Interfaces;

namespace Pedidos.DAL.Repositories
{
    public class UnitOfWork(PedidosDbContext context) : IUnitOfWork
    {
        public Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default)
        {
            return context.SaveChangesAsync(cancellationToken);
        }
    }
}
