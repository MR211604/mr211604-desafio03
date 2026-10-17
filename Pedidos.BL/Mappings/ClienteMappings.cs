using Pedidos.Entities.Dtos;
using Pedidos.Entities.Models;

namespace Pedidos.BL.Mappings
{
    public static class ClienteMappings
    {
        public static Cliente ToEntity(this ClienteRequestDto dto)
        {
            return new Cliente
            {
                Nombre = dto.Nombre?.Trim() ?? string.Empty,
                Apellido = dto.Apellido?.Trim() ?? string.Empty,
                Email = dto.Email?.Trim().ToLowerInvariant() ?? string.Empty
            };
        }

        public static ClienteResponseDto ToResponse(this Cliente cliente)
        {
            return new ClienteResponseDto(cliente.Id, cliente.Nombre, cliente.Apellido, cliente.Email);
        }
    }
}
