using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresupuestoApi.DTOs.Quincenas;
using PresupuestoApi.Helpers;
using PresupuestoApi.Services.Quincenas;

namespace PresupuestoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class QuincenasController : ControllerBase
{
    private readonly IQuincenaService _service;

    public QuincenasController(IQuincenaService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<QuincenaDto>>> Listar([FromQuery] int? anio, [FromQuery] int? mes)
    {
        var lista = await _service.ListarAsync(User.GetUsuarioId(), anio, mes);
        return Ok(lista);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<QuincenaDetalleDto>> ObtenerDetalle(int id)
    {
        var det = await _service.ObtenerDetalleAsync(User.GetUsuarioId(), id);
        if (det == null) return NotFound();
        return Ok(det);
    }

    [HttpPost]
    public async Task<ActionResult<QuincenaDto>> Crear([FromBody] CrearQuincenaDto dto)
    {
        var q = await _service.CrearAsync(User.GetUsuarioId(), dto);
        return CreatedAtAction(nameof(ObtenerDetalle), new { id = q.Id }, q);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _service.EliminarAsync(User.GetUsuarioId(), id);
        return NoContent();
    }

    /// <summary>
    /// Copia una quincena (con todos sus movimientos) a otro mes/año.
    /// Útil para replicar quincenas cuyo detalle cambia poco mes a mes.
    /// </summary>
    [HttpPost("{id:int}/copiar")]
    public async Task<ActionResult<QuincenaDto>> Copiar(int id, [FromBody] CopiarQuincenaDto dto)
    {
        var nueva = await _service.CopiarAsync(User.GetUsuarioId(), id, dto);
        return CreatedAtAction(nameof(ObtenerDetalle), new { id = nueva.Id }, nueva);
    }
}
