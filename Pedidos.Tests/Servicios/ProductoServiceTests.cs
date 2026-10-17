using NSubstitute;
using Pedidos.BL.Exceptions;
using Pedidos.BL.Services;
using Pedidos.DAL.Interfaces;
using Pedidos.Entities.Dtos;
using Pedidos.Entities.Models;

namespace Pedidos.Tests.Servicios
{
    public class ProductoServiceTests
    {
        private readonly IProductoRepository _productoRepository = Substitute.For<IProductoRepository>();
        private readonly IPedidoRepository _pedidoRepository = Substitute.For<IPedidoRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly ProductoService _sut;

        public ProductoServiceTests()
        {
            _sut = new ProductoService(_productoRepository, _pedidoRepository, _unitOfWork);
        }

        [Fact]
        public async Task Crear_ProductoValido_GuardaYDevuelveElProducto()
        {
            var dto = new ProductoRequestDto("Teclado mecánico", "Teclado con switches azules", 89.90m, 15);

            var resultado = await _sut.CrearAsync(dto);

            Assert.Equal("Teclado mecánico", resultado.Nombre);
            Assert.Equal("Teclado con switches azules", resultado.Descripcion);
            Assert.Equal(89.90m, resultado.Precio);
            Assert.Equal(15, resultado.Stock);
            await _productoRepository.Received(1).AddAsync(Arg.Any<Producto>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData(0, 10, "Precio")]
        [InlineData(-5, 10, "Precio")]
        [InlineData(10, -1, "Stock")]
        public async Task Crear_PrecioOStockInvalido_LanzaValidacionException(decimal precio, int stock, string campoEsperado)
        {
            var dto = new ProductoRequestDto("Mouse", "Mouse inalámbrico", precio, stock);

            var excepcion = await Assert.ThrowsAsync<ValidacionException>(() => _sut.CrearAsync(dto));

            Assert.True(excepcion.Errores.ContainsKey(campoEsperado));
            await _productoRepository.DidNotReceive().AddAsync(Arg.Any<Producto>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Actualizar_ProductoConStockNegativo_LanzaValidacionExceptionSinModificarElProducto()
        {
            var producto = new Producto { Id = 1, Nombre = "Mouse", Descripcion = "Inalámbrico", Precio = 25m, Stock = 4 };
            _productoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(producto);

            await Assert.ThrowsAsync<ValidacionException>(() =>
                _sut.ActualizarAsync(1, new ProductoRequestDto("Mouse", "Inalámbrico", 25m, -3)));

            Assert.Equal(4, producto.Stock);
            _productoRepository.DidNotReceive().Update(Arg.Any<Producto>());
        }

        [Fact]
        public async Task Eliminar_ProductoConPedidos_LanzaReglaDeNegocioException()
        {
            var producto = new Producto { Id = 3, Nombre = "Mouse", Descripcion = "Inalámbrico", Precio = 25m, Stock = 4 };
            _productoRepository.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns(producto);
            _pedidoRepository.ExistenPedidosParaProductoAsync(3, Arg.Any<CancellationToken>()).Returns(true);

            await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _sut.EliminarAsync(3));

            _productoRepository.DidNotReceive().Remove(Arg.Any<Producto>());
        }
    }
}
