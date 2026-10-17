using Microsoft.AspNetCore.Mvc;
using Pedidos.BL.Interfaces;
using Pedidos.Entities.Dtos;

namespace mr211604_desafio03.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController(IProductoService productoService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductoResponseDto>>> ObtenerTodos(CancellationToken cancellationToken)
        {
            return Ok(await productoService.ObtenerTodosAsync(cancellationToken));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoResponseDto>> ObtenerPorId(int id, CancellationToken cancellationToken)
        {
            return Ok(await productoService.ObtenerPorIdAsync(id, cancellationToken));
        }

        [HttpPost]
        public async Task<ActionResult<ProductoResponseDto>> Crear([FromBody] ProductoRequestDto dto, CancellationToken cancellationToken)
        {
            var producto = await productoService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductoResponseDto>> Actualizar(int id, [FromBody] ProductoRequestDto dto, CancellationToken cancellationToken)
        {
            return Ok(await productoService.ActualizarAsync(id, dto, cancellationToken));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
        {
            await productoService.EliminarAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
