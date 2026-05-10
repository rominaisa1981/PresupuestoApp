using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PresupuestoApi.Models;

namespace PresupuestoApi.Services.Token;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public (string token, DateTime expira) GenerarToken(Usuario usuario)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = jwtSettings["Key"] ?? throw new InvalidOperationException("Jwt:Key no configurado");
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expireHours = int.Parse(jwtSettings["ExpireHours"] ?? "12");

        var expira = DateTime.UtcNow.AddHours(expireHours);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim("nombre", usuario.Nombre),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expira,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expira);
    }
}
