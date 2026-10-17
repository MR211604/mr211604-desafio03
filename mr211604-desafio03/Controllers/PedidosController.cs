using Microsoft.AspNetCore.Mvc;
using Pedidos.BL.Interfaces;
using Pedidos.Entities.Dtos;

namespace mr211604_desafio03.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController(IPedidoService pedidoService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<PedidoResponseDto>>> ObtenerTodos(CancellationToken cancellationToken)
        {
            return Ok(await pedidoService.ObtenerTodosAsync(cancellationToken));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PedidoResponseDto>> ObtenerPorId(int id, CancellationToken cancellationToken)
        {
            return Ok(await pedidoService.ObtenerPorIdAsync(id, cancellationToken));
        }

        [HttpPost]
        public async Task<ActionResult<PedidoResponseDto>> Crear([FromBody] PedidoRequestDto dto, CancellationToken cancellationToken)
        {
            var pedido = await pedidoService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = pedido.Id }, pedido);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
        {
            await pedidoService.EliminarAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
