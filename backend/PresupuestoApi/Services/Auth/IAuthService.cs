using PresupuestoApi.DTOs.Auth;

namespace PresupuestoApi.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegistrarAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
