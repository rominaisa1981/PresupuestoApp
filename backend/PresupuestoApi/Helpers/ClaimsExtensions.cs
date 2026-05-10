using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PresupuestoApi.Helpers;

public static class ClaimsExtensions
{
    public static int GetUsuarioId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
               ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var id))
            throw new UnauthorizedAccessException("Token inválido");

        return id;
    }
}
