using Pedidos.Entities.Dtos;

namespace Pedidos.BL.Interfaces
{
    public interface IProductoService
    {
        Task<IReadOnlyList<ProductoResponseDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);

        Task<ProductoResponseDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ProductoResponseDto> CrearAsync(ProductoRequestDto dto, CancellationToken cancellationToken = default);

        Task<ProductoResponseDto> ActualizarAsync(int id, ProductoRequestDto dto, CancellationToken cancellationToken = default);

        Task EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}
