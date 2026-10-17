namespace Pedidos.Entities.Dtos
{
    public record PedidoRequestDto(int ClienteId, int ProductoId, int Cantidad);

    public record PedidoResponseDto(
        int Id,
        DateTime FechaPedido,
        int Cantidad,
        int ClienteId,
        string ClienteNombreCompleto,
        int ProductoId,
        string ProductoNombre);
}
