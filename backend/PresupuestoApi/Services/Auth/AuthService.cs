using Microsoft.EntityFrameworkCore;
using PresupuestoApi.Data;
using PresupuestoApi.DTOs.Auth;
using PresupuestoApi.Models;
using PresupuestoApi.Services.Token;

namespace PresupuestoApi.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegistrarAsync(RegisterDto dto)
    {
        var emailNormalizado = dto.Email.Trim().ToLower();

        if (await _db.Usuarios.AnyAsync(u => u.Email == emailNormalizado))
            throw new InvalidOperationException("Ya existe un usuario con ese email");

        var usuario = new Usuario
        {
            Email = emailNormalizado,
            Nombre = dto.Nombre.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        // Crear categorías iniciales para el usuario nuevo
        var categoriasIniciales = SeedData.CategoriasIniciales(usuario.Id);
        _db.Categorias.AddRange(categoriasIniciales);
        await _db.SaveChangesAsync();

        return ConstruirRespuesta(usuario);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var emailNormalizado = dto.Email.Trim().ToLower();

        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == emailNormalizado)
            ?? throw new UnauthorizedAccessException("Credenciales inválidas");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas");

        return ConstruirRespuesta(usuario);
    }

    private AuthResponseDto ConstruirRespuesta(Usuario usuario)
    {
        var (token, expira) = _tokenService.GenerarToken(usuario);

        return new AuthResponseDto
        {
            Token = token,
            Expira = expira,
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombre = usuario.Nombre
            }
        };
    }
}
