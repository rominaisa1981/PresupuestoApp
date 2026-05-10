using Microsoft.EntityFrameworkCore;
using PresupuestoApi.Data;
using PresupuestoApi.DTOs.Categorias;
using PresupuestoApi.Models;
using PresupuestoApi.Models.Enums;

namespace PresupuestoApi.Services.Categorias;

public interface ICategoriaService
{
    Task<List<CategoriaDto>> ListarAsync(int usuarioId, TipoMovimiento? tipo = null);
    Task<CategoriaDto> CrearAsync(int usuarioId, CrearCategoriaDto dto);
    Task<CategoriaDto> ActualizarAsync(int usuarioId, int id, ActualizarCategoriaDto dto);
    Task EliminarAsync(int usuarioId, int id);
}

public class CategoriaService : ICategoriaService
{
    private readonly AppDbContext _db;

    public CategoriaService(AppDbContext db) => _db = db;

    public async Task<List<CategoriaDto>> ListarAsync(int usuarioId, TipoMovimiento? tipo = null)
    {
        var query = _db.Categorias.Where(c => c.UsuarioId == usuarioId);

        if (tipo.HasValue)
            query = query.Where(c => c.Tipo == tipo.Value);

        return await query
            .OrderBy(c => c.Tipo)
            .ThenBy(c => c.Nombre)
            .Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Tipo = c.Tipo,
                Color = c.Color,
                CodigoRol = c.CodigoRol,
                PresupuestoMensual = c.PresupuestoMensual,
                Activa = c.Activa
            })
            .ToListAsync();
    }

    public async Task<CategoriaDto> CrearAsync(int usuarioId, CrearCategoriaDto dto)
    {
        var cat = new Categoria
        {
            Nombre = dto.Nombre.Trim(),
            Tipo = dto.Tipo,
            Color = dto.Color,
            CodigoRol = dto.CodigoRol,
            PresupuestoMensual = dto.PresupuestoMensual,
            UsuarioId = usuarioId
        };

        _db.Categorias.Add(cat);
        await _db.SaveChangesAsync();

        return MapearDto(cat);
    }

    public async Task<CategoriaDto> ActualizarAsync(int usuarioId, int id, ActualizarCategoriaDto dto)
    {
        var cat = await _db.Categorias.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId)
            ?? throw new KeyNotFoundException("Categoría no encontrada");

        cat.Nombre = dto.Nombre.Trim();
        cat.Color = dto.Color;
        cat.CodigoRol = dto.CodigoRol;
        cat.PresupuestoMensual = dto.PresupuestoMensual;
        cat.Activa = dto.Activa;

        await _db.SaveChangesAsync();

        return MapearDto(cat);
    }

    public async Task EliminarAsync(int usuarioId, int id)
    {
        var cat = await _db.Categorias.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId)
            ?? throw new KeyNotFoundException("Categoría no encontrada");

        // Soft delete: solo desactivamos para no perder histórico
        cat.Activa = false;
        await _db.SaveChangesAsync();
    }

    private static CategoriaDto MapearDto(Categoria c) => new()
    {
        Id = c.Id,
        Nombre = c.Nombre,
        Tipo = c.Tipo,
        Color = c.Color,
        CodigoRol = c.CodigoRol,
        PresupuestoMensual = c.PresupuestoMensual,
        Activa = c.Activa
    };
}
