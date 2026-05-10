using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresupuestoApi.DTOs.Movimientos;
using PresupuestoApi.Helpers;
using PresupuestoApi.Services.Movimientos;

namespace PresupuestoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MovimientosController : ControllerBase
{
    private readonly IMovimientoService _service;

    public MovimientosController(IMovimientoService service) => _service = service;

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovimientoDto>> Obtener(int id)
    {
        var m = await _service.ObtenerAsync(User.GetUsuarioId(), id);
        if (m == null) return NotFound();
        return Ok(m);
    }

    [HttpPost]
    public async Task<ActionResult<MovimientoDto>> Crear([FromBody] CrearMovimientoDto dto)
    {
        var m = await _service.CrearAsync(User.GetUsuarioId(), dto);
        return CreatedAtAction(nameof(Obtener), new { id = m.Id }, m);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MovimientoDto>> Actualizar(int id, [FromBody] ActualizarMovimientoDto dto)
    {
        var m = await _service.ActualizarAsync(User.GetUsuarioId(), id, dto);
        return Ok(m);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _service.EliminarAsync(User.GetUsuarioId(), id);
        return NoContent();
    }
}
