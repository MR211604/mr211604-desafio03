using Microsoft.AspNetCore.Mvc;
using Pedidos.BL.Interfaces;
using Pedidos.Entities.Dtos;

namespace mr211604_desafio03.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController(IClienteService clienteService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ClienteResponseDto>>> ObtenerTodos(CancellationToken cancellationToken)
        {
            return Ok(await clienteService.ObtenerTodosAsync(cancellationToken));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClienteResponseDto>> ObtenerPorId(int id, CancellationToken cancellationToken)
        {
            return Ok(await clienteService.ObtenerPorIdAsync(id, cancellationToken));
        }

        [HttpPost]
        public async Task<ActionResult<ClienteResponseDto>> Crear([FromBody] ClienteRequestDto dto, CancellationToken cancellationToken)
        {
            var cliente = await clienteService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = cliente.Id }, cliente);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ClienteResponseDto>> Actualizar(int id, [FromBody] ClienteRequestDto dto, CancellationToken cancellationToken)
        {
            return Ok(await clienteService.ActualizarAsync(id, dto, cancellationToken));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
        {
            await clienteService.EliminarAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
