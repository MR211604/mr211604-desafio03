namespace Pedidos.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default);
    }
}
