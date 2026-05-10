using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresupuestoApi.DTOs.Resumen;
using PresupuestoApi.Helpers;
using PresupuestoApi.Services.Resumen;

namespace PresupuestoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ResumenController : ControllerBase
{
    private readonly IResumenService _service;

    public ResumenController(IResumenService service) => _service = service;

    /// <summary>
    /// Obtiene el resumen completo de un mes con ambas quincenas y totales por categoría.
    /// </summary>
    [HttpGet("mensual/{anio:int}/{mes:int}")]
    public async Task<ActionResult<ResumenMensualDto>> ResumenMensual(int anio, int mes)
    {
        if (mes < 1 || mes > 12) return BadRequest("Mes inválido");
        var r = await _service.ObtenerResumenMensualAsync(User.GetUsuarioId(), anio, mes);
        return Ok(r);
    }
}
