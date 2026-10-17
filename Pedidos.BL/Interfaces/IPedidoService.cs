using Pedidos.Entities.Dtos;

namespace Pedidos.BL.Interfaces
{
    public interface IPedidoService
    {
        Task<IReadOnlyList<PedidoResponseDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);

        Task<PedidoResponseDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

        Task<PedidoResponseDto> CrearAsync(PedidoRequestDto dto, CancellationToken cancellationToken = default);

        Task EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}
