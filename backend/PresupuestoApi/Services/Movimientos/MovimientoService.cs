using Microsoft.EntityFrameworkCore;
using PresupuestoApi.Data;
using PresupuestoApi.DTOs.Movimientos;
using PresupuestoApi.Models;

namespace PresupuestoApi.Services.Movimientos;

public interface IMovimientoService
{
    Task<MovimientoDto> CrearAsync(int usuarioId, CrearMovimientoDto dto);
    Task<MovimientoDto> ActualizarAsync(int usuarioId, int id, ActualizarMovimientoDto dto);
    Task EliminarAsync(int usuarioId, int id);
    Task<MovimientoDto?> ObtenerAsync(int usuarioId, int id);
}

public class MovimientoService : IMovimientoService
{
    private readonly AppDbContext _db;

    public MovimientoService(AppDbContext db) => _db = db;

    public async Task<MovimientoDto> CrearAsync(int usuarioId, CrearMovimientoDto dto)
    {
        // Validar que la quincena pertenezca al usuario
        var quincena = await _db.Quincenas
            .FirstOrDefaultAsync(q => q.Id == dto.QuincenaId && q.UsuarioId == usuarioId)
            ?? throw new KeyNotFoundException("Quincena no encontrada");

        // Validar categoría si viene
        if (dto.CategoriaId.HasValue)
        {
            var catValida = await _db.Categorias.AnyAsync(c =>
                c.Id == dto.CategoriaId.Value && c.UsuarioId == usuarioId && c.Tipo == dto.Tipo);
            if (!catValida)
                throw new InvalidOperationException("Categoría inválida o no coincide con el tipo de movimiento");
        }

        // Validar movimiento padre si viene
        if (dto.MovimientoPadreId.HasValue)
        {
            var padreValido = await _db.Movimientos.AnyAsync(m =>
                m.Id == dto.MovimientoPadreId.Value &&
                m.UsuarioId == usuarioId &&
                m.QuincenaId == dto.QuincenaId &&
                m.Tipo == dto.Tipo);
            if (!padreValido)
                throw new InvalidOperationException("Movimiento padre inválido");
        }

        var mov = new Movimiento
        {
            Descripcion = dto.Descripcion.Trim(),
            Monto = dto.Monto,
            Tipo = dto.Tipo,
            Fecha = dto.Fecha ?? DateTime.UtcNow,
            Notas = dto.Notas,
            QuincenaId = dto.QuincenaId,
            CategoriaId = dto.CategoriaId,
            MovimientoPadreId = dto.MovimientoPadreId,
            UsuarioId = usuarioId
        };

        _db.Movimientos.Add(mov);
        await _db.SaveChangesAsync();

        return await ObtenerAsync(usuarioId, mov.Id) ?? throw new Exception("Error al crear movimiento");
    }

    public async Task<MovimientoDto> ActualizarAsync(int usuarioId, int id, ActualizarMovimientoDto dto)
    {
        var mov = await _db.Movimientos.FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId)
            ?? throw new KeyNotFoundException("Movimiento no encontrado");

        if (dto.CategoriaId.HasValue)
        {
            var catValida = await _db.Categorias.AnyAsync(c =>
                c.Id == dto.CategoriaId.Value && c.UsuarioId == usuarioId && c.Tipo == mov.Tipo);
            if (!catValida)
                throw new InvalidOperationException("Categoría inválida");
        }

        mov.Descripcion = dto.Descripcion.Trim();
        mov.Monto = dto.Monto;
        mov.CategoriaId = dto.CategoriaId;
        mov.Notas = dto.Notas;

        await _db.SaveChangesAsync();

        return await ObtenerAsync(usuarioId, mov.Id) ?? throw new Exception("Error al actualizar");
    }

    public async Task EliminarAsync(int usuarioId, int id)
    {
        var mov = await _db.Movimientos
            .Include(m => m.SubMovimientos)
            .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId)
            ?? throw new KeyNotFoundException("Movimiento no encontrado");

        // Borrar primero los hijos para evitar conflicto de FK
        if (mov.SubMovimientos.Any())
            _db.Movimientos.RemoveRange(mov.SubMovimientos);

        _db.Movimientos.Remove(mov);
        await _db.SaveChangesAsync();
    }

    public async Task<MovimientoDto?> ObtenerAsync(int usuarioId, int id)
    {
        var mov = await _db.Movimientos
            .Include(m => m.Categoria)
            .Include(m => m.SubMovimientos).ThenInclude(s => s.Categoria)
            .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

        if (mov == null) return null;

        return MapearDto(mov);
    }

    public static MovimientoDto MapearDto(Movimiento m) => new()
    {
        Id = m.Id,
        Descripcion = m.Descripcion,
        Monto = m.Monto,
        Tipo = m.Tipo,
        Fecha = m.Fecha,
        Notas = m.Notas,
        QuincenaId = m.QuincenaId,
        CategoriaId = m.CategoriaId,
        CategoriaNombre = m.Categoria?.Nombre,
        MovimientoPadreId = m.MovimientoPadreId,
        SubMovimientos = m.SubMovimientos
            .OrderBy(s => s.Fecha)
            .Select(MapearDto)
            .ToList()
    };
}
