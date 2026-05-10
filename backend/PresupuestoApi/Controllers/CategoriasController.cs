using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresupuestoApi.DTOs.Categorias;
using PresupuestoApi.Helpers;
using PresupuestoApi.Models.Enums;
using PresupuestoApi.Services.Categorias;

namespace PresupuestoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _service;

    public CategoriasController(ICategoriaService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<CategoriaDto>>> Listar([FromQuery] TipoMovimiento? tipo)
    {
        var lista = await _service.ListarAsync(User.GetUsuarioId(), tipo);
        return Ok(lista);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> Crear([FromBody] CrearCategoriaDto dto)
    {
        var cat = await _service.CrearAsync(User.GetUsuarioId(), dto);
        return CreatedAtAction(nameof(Listar), new { id = cat.Id }, cat);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoriaDto>> Actualizar(int id, [FromBody] ActualizarCategoriaDto dto)
    {
        var cat = await _service.ActualizarAsync(User.GetUsuarioId(), id, dto);
        return Ok(cat);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _service.EliminarAsync(User.GetUsuarioId(), id);
        return NoContent();
    }
}
