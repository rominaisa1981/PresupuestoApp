using Microsoft.AspNetCore.Mvc;
using PresupuestoApi.DTOs.Auth;
using PresupuestoApi.Services.Auth;

namespace PresupuestoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("registro")]
    public async Task<ActionResult<AuthResponseDto>> Registrar([FromBody] RegisterDto dto)
    {
        var resp = await _authService.RegistrarAsync(dto);
        return Ok(resp);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var resp = await _authService.LoginAsync(dto);
        return Ok(resp);
    }
}
