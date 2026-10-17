using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Pedidos.BL.Exceptions;
using Pedidos.BL.Services;
using Pedidos.DAL.Interfaces;
using Pedidos.Entities.Dtos;
using Pedidos.Entities.Models;

namespace Pedidos.Tests.Servicios
{
    public class PedidoServiceTests
    {
        private static readonly DateTimeOffset Ahora = new(2026, 10, 16, 12, 30, 0, TimeSpan.Zero);

        private readonly IPedidoRepository _pedidoRepository = Substitute.For<IPedidoRepository>();
        private readonly IClienteRepository _clienteRepository = Substitute.For<IClienteRepository>();
        private readonly IProductoRepository _productoRepository = Substitute.For<IProductoRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly FakeTimeProvider _timeProvider = new(Ahora);
        private readonly PedidoService _sut;

        private readonly Cliente _cliente = new() { Id = 1, Nombre = "Ana", Apellido = "García", Email = "ana@ejemplo.com" };

        public PedidoServiceTests()
        {
            _sut = new PedidoService(
                _pedidoRepository,
                _clienteRepository,
                _productoRepository,
                _unitOfWork,
                _timeProvider);
        }

        private Producto CrearProducto(int stock) =>
            new() { Id = 2, Nombre = "Teclado", Descripcion = "Teclado mecánico", Precio = 50m, Stock = stock };

        [Fact]
        public async Task Crear_PedidoValido_RegistraFechaYDescuentaStock()
        {
            var producto = CrearProducto(stock: 10);
            _clienteRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(_cliente);
            _productoRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(producto);

            var resultado = await _sut.CrearAsync(new PedidoRequestDto(ClienteId: 1, ProductoId: 2, Cantidad: 3));

            Assert.Equal(Ahora.UtcDateTime, resultado.FechaPedido);
            Assert.Equal(3, resultado.Cantidad);
            Assert.Equal("Ana García", resultado.ClienteNombreCompleto);
            Assert.Equal("Teclado", resultado.ProductoNombre);
            Assert.Equal(7, producto.Stock);
            await _pedidoRepository.Received(1).AddAsync(Arg.Any<Pedido>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Crear_PedidoConStockInsuficiente_LanzaReglaDeNegocioExceptionSinDescontarStock()
        {
            var producto = CrearProducto(stock: 2);
            _clienteRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(_cliente);
            _productoRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(producto);

            var excepcion = await Assert.ThrowsAsync<ReglaDeNegocioException>(() =>
                _sut.CrearAsync(new PedidoRequestDto(1, 2, 3)));

            Assert.Contains("Stock insuficiente", excepcion.Message);
            Assert.Equal(2, producto.Stock);
            await _pedidoRepository.DidNotReceive().AddAsync(Arg.Any<Pedido>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Crear_PedidoConCantidadCero_LanzaValidacionException()
        {
            var excepcion = await Assert.ThrowsAsync<ValidacionException>(() =>
                _sut.CrearAsync(new PedidoRequestDto(1, 2, 0)));

            Assert.True(excepcion.Errores.ContainsKey(nameof(Pedido.Cantidad)));
            await _pedidoRepository.DidNotReceive().AddAsync(Arg.Any<Pedido>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Crear_PedidoConClienteInexistente_LanzaEntidadNoEncontradaException()
        {
            _clienteRepository.GetByIdAsync(50, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

            var excepcion = await Assert.ThrowsAsync<EntidadNoEncontradaException>(() =>
                _sut.CrearAsync(new PedidoRequestDto(50, 2, 1)));

            Assert.Equal(nameof(Cliente), excepcion.Entidad);
            Assert.Equal(50, excepcion.Id);
        }

        [Fact]
        public async Task ObtenerPorId_PedidoExistente_DevuelveElPedidoConDetalles()
        {
            var producto = CrearProducto(stock: 5);
            var pedido = new Pedido
            {
                Id = 5,
                FechaPedido = Ahora.UtcDateTime,
                Cantidad = 2,
                ClienteId = _cliente.Id,
                ProductoId = producto.Id,
                Cliente = _cliente,
                Producto = producto
            };
            _pedidoRepository.GetByIdConDetallesAsync(5, Arg.Any<CancellationToken>()).Returns(pedido);

            var resultado = await _sut.ObtenerPorIdAsync(5);

            Assert.Equal(5, resultado.Id);
            Assert.Equal(2, resultado.Cantidad);
            Assert.Equal(1, resultado.ClienteId);
            Assert.Equal("Ana García", resultado.ClienteNombreCompleto);
            Assert.Equal("Teclado", resultado.ProductoNombre);
        }

        [Fact]
        public async Task ObtenerPorId_PedidoInexistente_LanzaEntidadNoEncontradaException()
        {
            _pedidoRepository.GetByIdConDetallesAsync(999, Arg.Any<CancellationToken>()).Returns((Pedido?)null);

            var excepcion = await Assert.ThrowsAsync<EntidadNoEncontradaException>(() => _sut.ObtenerPorIdAsync(999));

            Assert.Equal(nameof(Pedido), excepcion.Entidad);
            Assert.Equal(999, excepcion.Id);
        }

        [Fact]
        public async Task Eliminar_PedidoExistente_DevuelveElStockAlProducto()
        {
            var producto = CrearProducto(stock: 7);
            var pedido = new Pedido { Id = 8, Cantidad = 3, ClienteId = 1, ProductoId = producto.Id };
            _pedidoRepository.GetByIdAsync(8, Arg.Any<CancellationToken>()).Returns(pedido);
            _productoRepository.GetByIdAsync(producto.Id, Arg.Any<CancellationToken>()).Returns(producto);

            await _sut.EliminarAsync(8);

            Assert.Equal(10, producto.Stock);
            _pedidoRepository.Received(1).Remove(pedido);
            await _unitOfWork.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
        }
    }
}
