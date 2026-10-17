using Microsoft.EntityFrameworkCore;
using Pedidos.DAL.Data;
using Pedidos.DAL.Interfaces;

namespace Pedidos.DAL.Repositories
{
    public class Repository<T>(PedidosDbContext context) : IRepository<T> where T : class
    {
        protected readonly PedidosDbContext Context = context;

        protected DbSet<T> Set => Context.Set<T>();

        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await Set.FindAsync([id], cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Set.AsNoTracking().ToListAsync(cancellationToken);
        }

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await Set.AddAsync(entity, cancellationToken);
        }

        public virtual void Update(T entity)
        {
            Set.Update(entity);
        }

        public virtual void Remove(T entity)
        {
            Set.Remove(entity);
        }
    }
}
