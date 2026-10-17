using Pedidos.Entities.Dtos;
using Pedidos.Entities.Models;

namespace Pedidos.BL.Mappings
{
    public static class ProductoMappings
    {
        public static Producto ToEntity(this ProductoRequestDto dto)
        {
            return new Producto
            {
                Nombre = dto.Nombre?.Trim() ?? string.Empty,
                Descripcion = dto.Descripcion?.Trim() ?? string.Empty,
                Precio = dto.Precio,
                Stock = dto.Stock
            };
        }

        public static ProductoResponseDto ToResponse(this Producto producto)
        {
            return new ProductoResponseDto(producto.Id, producto.Nombre, producto.Descripcion, producto.Precio, producto.Stock);
        }
    }
}
