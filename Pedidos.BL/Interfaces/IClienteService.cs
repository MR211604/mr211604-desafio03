using Pedidos.Entities.Dtos;

namespace Pedidos.BL.Interfaces
{
    public interface IClienteService
    {
        Task<IReadOnlyList<ClienteResponseDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);

        Task<ClienteResponseDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ClienteResponseDto> CrearAsync(ClienteRequestDto dto, CancellationToken cancellationToken = default);

        Task<ClienteResponseDto> ActualizarAsync(int id, ClienteRequestDto dto, CancellationToken cancellationToken = default);

        Task EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}
