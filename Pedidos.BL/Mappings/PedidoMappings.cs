using Pedidos.Entities.Dtos;
using Pedidos.Entities.Models;

namespace Pedidos.BL.Mappings
{
    public static class PedidoMappings
    {
        public static Pedido ToEntity(this PedidoRequestDto dto, DateTime fechaPedido)
        {
            return new Pedido
            {
                FechaPedido = fechaPedido,
                Cantidad = dto.Cantidad,
                ClienteId = dto.ClienteId,
                ProductoId = dto.ProductoId
            };
        }

        public static PedidoResponseDto ToResponse(this Pedido pedido)
        {
            return new PedidoResponseDto(
                pedido.Id,
                DateTime.SpecifyKind(pedido.FechaPedido, DateTimeKind.Utc),
                pedido.Cantidad,
                pedido.ClienteId,
                $"{pedido.Cliente.Nombre} {pedido.Cliente.Apellido}",
                pedido.ProductoId,
                pedido.Producto.Nombre);
        }
    }
}
