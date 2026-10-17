using Pedidos.BL.Exceptions;
using Pedidos.BL.Interfaces;
using Pedidos.BL.Mappings;
using Pedidos.BL.Validation;
using Pedidos.DAL.Interfaces;
using Pedidos.Entities.Dtos;
using Pedidos.Entities.Models;

namespace Pedidos.BL.Services
{
    public class ClienteService(
        IClienteRepository clienteRepository,
        IPedidoRepository pedidoRepository,
        IUnitOfWork unitOfWork) : IClienteService
    {
        public async Task<IReadOnlyList<ClienteResponseDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
        {
            var clientes = await clienteRepository.GetAllAsync(cancellationToken);
            return clientes.Select(c => c.ToResponse()).ToList();
        }

        public async Task<ClienteResponseDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var cliente = await ObtenerEntidadAsync(id, cancellationToken);
            return cliente.ToResponse();
        }

        public async Task<ClienteResponseDto> CrearAsync(ClienteRequestDto dto, CancellationToken cancellationToken = default)
        {
            var cliente = dto.ToEntity();
            ModeloValidator.Validar(cliente);

            await AsegurarEmailDisponibleAsync(cliente.Email, null, cancellationToken);

            await clienteRepository.AddAsync(cliente, cancellationToken);
            await unitOfWork.GuardarCambiosAsync(cancellationToken);

            return cliente.ToResponse();
        }

        public async Task<ClienteResponseDto> ActualizarAsync(int id, ClienteRequestDto dto, CancellationToken cancellationToken = default)
        {
            var cliente = await ObtenerEntidadAsync(id, cancellationToken);

            var datos = dto.ToEntity();
            ModeloValidator.Validar(datos);

            await AsegurarEmailDisponibleAsync(datos.Email, id, cancellationToken);

            cliente.Nombre = datos.Nombre;
            cliente.Apellido = datos.Apellido;
            cliente.Email = datos.Email;

            clienteRepository.Update(cliente);
            await unitOfWork.GuardarCambiosAsync(cancellationToken);

            return cliente.ToResponse();
        }

        public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var cliente = await ObtenerEntidadAsync(id, cancellationToken);

            if (await pedidoRepository.ExistenPedidosParaClienteAsync(id, cancellationToken))
            {
                throw new ReglaDeNegocioException("No se puede eliminar el cliente porque tiene pedidos asociados.");
            }

            clienteRepository.Remove(cliente);
            await unitOfWork.GuardarCambiosAsync(cancellationToken);
        }

        private async Task<Cliente> ObtenerEntidadAsync(int id, CancellationToken cancellationToken)
        {
            return await clienteRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new EntidadNoEncontradaException(nameof(Cliente), id);
        }

        private async Task AsegurarEmailDisponibleAsync(string email, int? idExcluido, CancellationToken cancellationToken)
        {
            if (await clienteRepository.ExisteEmailAsync(email, idExcluido, cancellationToken))
            {
                throw new ReglaDeNegocioException($"Ya existe un cliente registrado con el email '{email}'.");
            }
        }
    }
}
