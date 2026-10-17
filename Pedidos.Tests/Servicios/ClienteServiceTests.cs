using NSubstitute;
using Pedidos.BL.Exceptions;
using Pedidos.BL.Services;
using Pedidos.DAL.Interfaces;
using Pedidos.Entities.Dtos;
using Pedidos.Entities.Models;

namespace Pedidos.Tests.Servicios
{
    public class ClienteServiceTests
    {
        private readonly IClienteRepository _clienteRepository = Substitute.For<IClienteRepository>();
        private readonly IPedidoRepository _pedidoRepository = Substitute.For<IPedidoRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly ClienteService _sut;

        public ClienteServiceTests()
        {
            _sut = new ClienteService(_clienteRepository, _pedidoRepository, _unitOfWork);
        }

        [Fact]
        public async Task Crear_ClienteValido_GuardaYDevuelveElCliente()
        {
            var dto = new ClienteRequestDto("Ana", "García", "  Ana.Garcia@Ejemplo.com ");
            _clienteRepository.ExisteEmailAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
                .Returns(false);

            var resultado = await _sut.CrearAsync(dto);

            Assert.Equal("Ana", resultado.Nombre);
            Assert.Equal("García", resultado.Apellido);
            Assert.Equal("ana.garcia@ejemplo.com", resultado.Email);
            await _clienteRepository.Received(1).AddAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData("", "García", "ana@ejemplo.com", "Nombre")]
        [InlineData("A", "García", "ana@ejemplo.com", "Nombre")]
        [InlineData("Ana", "", "ana@ejemplo.com", "Apellido")]
        [InlineData("Ana", "García", "", "Email")]
        [InlineData("Ana", "García", "correo-invalido", "Email")]
        public async Task Crear_ClienteConDatosInvalidos_LanzaValidacionException(
            string nombre, string apellido, string email, string campoEsperado)
        {
            var dto = new ClienteRequestDto(nombre, apellido, email);

            var excepcion = await Assert.ThrowsAsync<ValidacionException>(() => _sut.CrearAsync(dto));

            Assert.True(excepcion.Errores.ContainsKey(campoEsperado));
            await _clienteRepository.DidNotReceive().AddAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Crear_ClienteConEmailExistente_LanzaReglaDeNegocioException()
        {
            var dto = new ClienteRequestDto("Ana", "García", "ana@ejemplo.com");
            _clienteRepository.ExisteEmailAsync("ana@ejemplo.com", null, Arg.Any<CancellationToken>())
                .Returns(true);

            await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _sut.CrearAsync(dto));

            await _clienteRepository.DidNotReceive().AddAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ObtenerPorId_ClienteInexistente_LanzaEntidadNoEncontradaException()
        {
            _clienteRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

            var excepcion = await Assert.ThrowsAsync<EntidadNoEncontradaException>(() => _sut.ObtenerPorIdAsync(99));

            Assert.Equal(99, excepcion.Id);
            Assert.Equal(nameof(Cliente), excepcion.Entidad);
        }

        [Fact]
        public async Task Eliminar_ClienteConPedidos_LanzaReglaDeNegocioException()
        {
            var cliente = new Cliente { Id = 1, Nombre = "Ana", Apellido = "García", Email = "ana@ejemplo.com" };
            _clienteRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(cliente);
            _pedidoRepository.ExistenPedidosParaClienteAsync(1, Arg.Any<CancellationToken>()).Returns(true);

            await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _sut.EliminarAsync(1));

            _clienteRepository.DidNotReceive().Remove(Arg.Any<Cliente>());
        }
    }
}
