using Pedidos.BL.Exceptions;
using Pedidos.BL.Interfaces;
using Pedidos.BL.Mappings;
using Pedidos.BL.Validation;
using Pedidos.DAL.Interfaces;
using Pedidos.Entities.Dtos;
using Pedidos.Entities.Models;

namespace Pedidos.BL.Services
{
    public class PedidoService(
        IPedidoRepository pedidoRepository,
        IClienteRepository clienteRepository,
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider) : IPedidoService
    {
        public async Task<IReadOnlyList<PedidoResponseDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
        {
            var pedidos = await pedidoRepository.GetAllConDetallesAsync(cancellationToken);
            return pedidos.Select(p => p.ToResponse()).ToList();
        }

        public async Task<PedidoResponseDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var pedido = await pedidoRepository.GetByIdConDetallesAsync(id, cancellationToken)
                ?? throw new EntidadNoEncontradaException(nameof(Pedido), id);

            return pedido.ToResponse();
        }

        public async Task<PedidoResponseDto> CrearAsync(PedidoRequestDto dto, CancellationToken cancellationToken = default)
        {
            var pedido = dto.ToEntity(timeProvider.GetUtcNow().UtcDateTime);
            ModeloValidator.Validar(pedido);

            var cliente = await clienteRepository.GetByIdAsync(pedido.ClienteId, cancellationToken)
                ?? throw new EntidadNoEncontradaException(nameof(Cliente), pedido.ClienteId);

            var producto = await productoRepository.GetByIdAsync(pedido.ProductoId, cancellationToken)
                ?? throw new EntidadNoEncontradaException(nameof(Producto), pedido.ProductoId);

            if (producto.Stock < pedido.Cantidad)
            {
                throw new ReglaDeNegocioException(
                    $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {pedido.Cantidad}.");
            }

            producto.Stock -= pedido.Cantidad;
            productoRepository.Update(producto);

            pedido.Cliente = cliente;
            pedido.Producto = producto;

            await pedidoRepository.AddAsync(pedido, cancellationToken);
            await unitOfWork.GuardarCambiosAsync(cancellationToken);

            return pedido.ToResponse();
        }

        public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var pedido = await pedidoRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new EntidadNoEncontradaException(nameof(Pedido), id);

            var producto = await productoRepository.GetByIdAsync(pedido.ProductoId, cancellationToken);
            if (producto is not null)
            {
                producto.Stock += pedido.Cantidad;
                productoRepository.Update(producto);
            }

            pedidoRepository.Remove(pedido);
            await unitOfWork.GuardarCambiosAsync(cancellationToken);
        }
    }
}
