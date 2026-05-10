using PresupuestoApi.Models;

namespace PresupuestoApi.Services.Token;

public interface ITokenService
{
    (string token, DateTime expira) GenerarToken(Usuario usuario);
}
