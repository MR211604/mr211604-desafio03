using Pedidos.BL.Exceptions;
using Pedidos.BL.Interfaces;
using Pedidos.BL.Mappings;
using Pedidos.BL.Validation;
using Pedidos.DAL.Interfaces;
using Pedidos.Entities.Dtos;
using Pedidos.Entities.Models;

namespace Pedidos.BL.Services
{
    public class ProductoService(
        IProductoRepository productoRepository,
        IPedidoRepository pedidoRepository,
        IUnitOfWork unitOfWork) : IProductoService
    {
        public async Task<IReadOnlyList<ProductoResponseDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
        {
            var productos = await productoRepository.GetAllAsync(cancellationToken);
            return productos.Select(p => p.ToResponse()).ToList();
        }

        public async Task<ProductoResponseDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var producto = await ObtenerEntidadAsync(id, cancellationToken);
            return producto.ToResponse();
        }

        public async Task<ProductoResponseDto> CrearAsync(ProductoRequestDto dto, CancellationToken cancellationToken = default)
        {
            var producto = dto.ToEntity();
            ModeloValidator.Validar(producto);

            await productoRepository.AddAsync(producto, cancellationToken);
            await unitOfWork.GuardarCambiosAsync(cancellationToken);

            return producto.ToResponse();
        }

        public async Task<ProductoResponseDto> ActualizarAsync(int id, ProductoRequestDto dto, CancellationToken cancellationToken = default)
        {
            var producto = await ObtenerEntidadAsync(id, cancellationToken);

            var datos = dto.ToEntity();
            ModeloValidator.Validar(datos);

            producto.Nombre = datos.Nombre;
            producto.Descripcion = datos.Descripcion;
            producto.Precio = datos.Precio;
            producto.Stock = datos.Stock;

            productoRepository.Update(producto);
            await unitOfWork.GuardarCambiosAsync(cancellationToken);

            return producto.ToResponse();
        }

        public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var producto = await ObtenerEntidadAsync(id, cancellationToken);

            if (await pedidoRepository.ExistenPedidosParaProductoAsync(id, cancellationToken))
            {
                throw new ReglaDeNegocioException("No se puede eliminar el producto porque tiene pedidos asociados.");
            }

            productoRepository.Remove(producto);
            await unitOfWork.GuardarCambiosAsync(cancellationToken);
        }

        private async Task<Producto> ObtenerEntidadAsync(int id, CancellationToken cancellationToken)
        {
            return await productoRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new EntidadNoEncontradaException(nameof(Producto), id);
        }
    }
}
