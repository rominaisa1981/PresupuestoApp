using Microsoft.EntityFrameworkCore;
using PresupuestoApi.Data;
using PresupuestoApi.DTOs.Movimientos;
using PresupuestoApi.DTOs.Quincenas;
using PresupuestoApi.Models.Enums;
using PresupuestoApi.Services.Movimientos;

namespace PresupuestoApi.Services.Quincenas;

public interface IQuincenaService
{
    Task<List<QuincenaDto>> ListarAsync(int usuarioId, int? anio = null, int? mes = null);
    Task<QuincenaDetalleDto?> ObtenerDetalleAsync(int usuarioId, int id);
    Task<QuincenaDto> CrearAsync(int usuarioId, CrearQuincenaDto dto);
    Task<QuincenaDto> CopiarAsync(int usuarioId, int quincenaId, CopiarQuincenaDto dto);
    Task EliminarAsync(int usuarioId, int id);
}

public class QuincenaService : IQuincenaService
{
    private readonly AppDbContext _db;

    public QuincenaService(AppDbContext db) => _db = db;

    public async Task<List<QuincenaDto>> ListarAsync(int usuarioId, int? anio = null, int? mes = null)
    {
        var query = _db.Quincenas.Where(q => q.UsuarioId == usuarioId);

        if (anio.HasValue) query = query.Where(q => q.Anio == anio.Value);
        if (mes.HasValue) query = query.Where(q => q.Mes == mes.Value);

        return await query
            .OrderByDescending(q => q.Anio)
            .ThenByDescending(q => q.Mes)
            .ThenByDescending(q => q.Numero)
            .Select(q => new QuincenaDto
            {
                Id = q.Id,
                FechaPago = q.FechaPago,
                Numero = q.Numero,
                Mes = q.Mes,
                Anio = q.Anio,
                Observaciones = q.Observaciones
            })
            .ToListAsync();
    }

    public async Task<QuincenaDetalleDto?> ObtenerDetalleAsync(int usuarioId, int id)
    {
        var quincena = await _db.Quincenas
            .Include(q => q.Movimientos.Where(m => m.MovimientoPadreId == null))
                .ThenInclude(m => m.Categoria)
            .Include(q => q.Movimientos.Where(m => m.MovimientoPadreId == null))
                .ThenInclude(m => m.SubMovimientos)
                    .ThenInclude(s => s.Categoria)
            .FirstOrDefaultAsync(q => q.Id == id && q.UsuarioId == usuarioId);

        if (quincena == null) return null;

        var todosMovs = await _db.Movimientos
            .Where(m => m.QuincenaId == id && m.UsuarioId == usuarioId)
            .ToListAsync();

        // Solo movimientos raíz (sin padre) en las listas, los hijos van anidados
        var raiz = quincena.Movimientos.Where(m => m.MovimientoPadreId == null);

        var ingresos = raiz.Where(m => m.Tipo == TipoMovimiento.Ingreso)
            .Select(MovimientoService.MapearDto).ToList();
        var descuentos = raiz.Where(m => m.Tipo == TipoMovimiento.Descuento)
            .Select(MovimientoService.MapearDto).ToList();
        var pagos = raiz.Where(m => m.Tipo == TipoMovimiento.Pago)
            .Select(MovimientoService.MapearDto).ToList();
        var gastos = raiz.Where(m => m.Tipo == TipoMovimiento.Gasto)
            .Select(MovimientoService.MapearDto).ToList();

        // Totales: SOLO movimientos raíz (sin padre) para no contar doble
        var totalIngresos = todosMovs.Where(m => m.Tipo == TipoMovimiento.Ingreso && m.MovimientoPadreId == null).Sum(m => m.Monto);
        var totalDescuentos = todosMovs.Where(m => m.Tipo == TipoMovimiento.Descuento && m.MovimientoPadreId == null).Sum(m => m.Monto);
        var totalPagos = todosMovs.Where(m => m.Tipo == TipoMovimiento.Pago && m.MovimientoPadreId == null).Sum(m => m.Monto);
        var totalGastos = todosMovs.Where(m => m.Tipo == TipoMovimiento.Gasto && m.MovimientoPadreId == null).Sum(m => m.Monto);

        var neto = totalIngresos - totalDescuentos;
        var saldo = neto - totalPagos - totalGastos;

        return new QuincenaDetalleDto
        {
            Id = quincena.Id,
            FechaPago = quincena.FechaPago,
            Numero = quincena.Numero,
            Mes = quincena.Mes,
            Anio = quincena.Anio,
            Observaciones = quincena.Observaciones,
            TotalIngresos = totalIngresos,
            TotalDescuentos = totalDescuentos,
            NetoRecibir = neto,
            TotalPagos = totalPagos,
            TotalGastos = totalGastos,
            Saldo = saldo,
            Ingresos = ingresos,
            Descuentos = descuentos,
            Pagos = pagos,
            Gastos = gastos
        };
    }

    public async Task<QuincenaDto> CrearAsync(int usuarioId, CrearQuincenaDto dto)
    {
        var existe = await _db.Quincenas.AnyAsync(q =>
            q.UsuarioId == usuarioId &&
            q.Anio == dto.Anio &&
            q.Mes == dto.Mes &&
            q.Numero == dto.Numero);

        if (existe)
            throw new InvalidOperationException("Ya existe una quincena con esos datos");

        var q = new Models.Quincena
        {
            FechaPago = dto.FechaPago,
            Numero = dto.Numero,
            Mes = dto.Mes,
            Anio = dto.Anio,
            Observaciones = dto.Observaciones,
            UsuarioId = usuarioId
        };

        _db.Quincenas.Add(q);
        await _db.SaveChangesAsync();

        return new QuincenaDto
        {
            Id = q.Id,
            FechaPago = q.FechaPago,
            Numero = q.Numero,
            Mes = q.Mes,
            Anio = q.Anio,
            Observaciones = q.Observaciones
        };
    }

    public async Task EliminarAsync(int usuarioId, int id)
    {
        var q = await _db.Quincenas.FirstOrDefaultAsync(q => q.Id == id && q.UsuarioId == usuarioId)
            ?? throw new KeyNotFoundException("Quincena no encontrada");

        _db.Quincenas.Remove(q); // cascada elimina los movimientos
        await _db.SaveChangesAsync();
    }

    public async Task<QuincenaDto> CopiarAsync(int usuarioId, int quincenaId, CopiarQuincenaDto dto)
    {
        // Cargar quincena origen con todos los movimientos raíz y sus hijos
        var fuente = await _db.Quincenas
            .Include(q => q.Movimientos.Where(m => m.MovimientoPadreId == null))
                .ThenInclude(m => m.SubMovimientos)
            .FirstOrDefaultAsync(q => q.Id == quincenaId && q.UsuarioId == usuarioId)
            ?? throw new KeyNotFoundException("Quincena no encontrada");

        // Verificar que no exista ya la misma quincena en el período destino
        var existe = await _db.Quincenas.AnyAsync(q =>
            q.UsuarioId == usuarioId &&
            q.Anio == dto.Anio &&
            q.Mes == dto.Mes &&
            q.Numero == fuente.Numero);

        if (existe)
            throw new InvalidOperationException(
                $"Ya existe la {fuente.Numero}era quincena para {dto.Mes}/{dto.Anio}");

        // Crear la nueva quincena
        var nueva = new Models.Quincena
        {
            FechaPago = dto.FechaPago ?? fuente.FechaPago,
            Numero = fuente.Numero,
            Mes = dto.Mes,
            Anio = dto.Anio,
            Observaciones = fuente.Observaciones,
            UsuarioId = usuarioId
        };

        _db.Quincenas.Add(nueva);
        await _db.SaveChangesAsync();

        // Copiar movimientos raíz y sus sub-items
        foreach (var mov in fuente.Movimientos.Where(m => m.MovimientoPadreId == null))
        {
            var nuevoMov = new Models.Movimiento
            {
                Descripcion = mov.Descripcion,
                Monto = mov.Monto,
                Tipo = mov.Tipo,
                Fecha = nueva.FechaPago,
                Notas = mov.Notas,
                QuincenaId = nueva.Id,
                CategoriaId = mov.CategoriaId,
                UsuarioId = usuarioId
            };
            _db.Movimientos.Add(nuevoMov);
            await _db.SaveChangesAsync();

            // Copiar sub-movimientos (PQ1, PQ2, etc.)
            foreach (var sub in mov.SubMovimientos)
            {
                _db.Movimientos.Add(new Models.Movimiento
                {
                    Descripcion = sub.Descripcion,
                    Monto = sub.Monto,
                    Tipo = sub.Tipo,
                    Fecha = nueva.FechaPago,
                    Notas = sub.Notas,
                    QuincenaId = nueva.Id,
                    CategoriaId = sub.CategoriaId,
                    MovimientoPadreId = nuevoMov.Id,
                    UsuarioId = usuarioId
                });
            }
            await _db.SaveChangesAsync();
        }

        return new QuincenaDto
        {
            Id = nueva.Id,
            FechaPago = nueva.FechaPago,
            Numero = nueva.Numero,
            Mes = nueva.Mes,
            Anio = nueva.Anio,
            Observaciones = nueva.Observaciones
        };
    }
}
