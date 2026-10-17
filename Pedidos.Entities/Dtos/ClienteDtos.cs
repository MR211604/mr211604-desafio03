namespace Pedidos.Entities.Dtos
{
    public record ClienteRequestDto(string Nombre, string Apellido, string Email);

    public record ClienteResponseDto(int Id, string Nombre, string Apellido, string Email);
}
