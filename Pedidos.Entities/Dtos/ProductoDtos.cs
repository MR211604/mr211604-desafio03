namespace Pedidos.Entities.Dtos
{
    public record ProductoRequestDto(string Nombre, string Descripcion, decimal Precio, int Stock);

    public record ProductoResponseDto(int Id, string Nombre, string Descripcion, decimal Precio, int Stock);
}
