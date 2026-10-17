using Pedidos.DAL.Data;
using Pedidos.DAL.Interfaces;
using Pedidos.Entities.Models;

namespace Pedidos.DAL.Repositories
{
    public class ProductoRepository(PedidosDbContext context) : Repository<Producto>(context), IProductoRepository
    {
    }
}
