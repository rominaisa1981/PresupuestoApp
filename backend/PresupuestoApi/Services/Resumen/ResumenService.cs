using System.Globalization;
using Microsoft.EntityFrameworkCore;
using PresupuestoApi.Data;
using PresupuestoApi.DTOs.Resumen;
using PresupuestoApi.Models.Enums;
using PresupuestoApi.Services.Quincenas;

namespace PresupuestoApi.Services.Resumen;

public interface IResumenService
{
    Task<ResumenMensualDto> ObtenerResumenMensualAsync(int usuarioId, int anio, int mes);
}

public class ResumenService : IResumenService
{
    private readonly AppDbContext _db;
    private readonly IQuincenaService _quincenaService;

    public ResumenService(AppDbContext db, IQuincenaService quincenaService)
    {
        _db = db;
        _quincenaService = quincenaService;
    }

    public async Task<ResumenMensualDto> ObtenerResumenMensualAsync(int usuarioId, int anio, int mes)
    {
        var quincenas = await _db.Quincenas
            .Where(q => q.UsuarioId == usuarioId && q.Anio == anio && q.Mes == mes)
            .OrderBy(q => q.Numero)
            .Select(q => q.Id)
            .ToListAsync();

        var detalles = new List<DTOs.Quincenas.QuincenaDetalleDto>();
        foreach (var qId in quincenas)
        {
            var det = await _quincenaService.ObtenerDetalleAsync(usuarioId, qId);
            if (det != null) detalles.Add(det);
        }

        var totalIngresos = detalles.Sum(d => d.TotalIngresos);
        var totalDescuentos = detalles.Sum(d => d.TotalDescuentos);
        var totalPagos = detalles.Sum(d => d.TotalPagos);
        var totalGastos = detalles.Sum(d => d.TotalGastos);
        var neto = totalIngresos - totalDescuentos;
        var saldo = neto - totalPagos - totalGastos;

        // Resumen por categoría (solo Pagos y Gastos, contando solo movimientos raíz)
        var resumenCat = await _db.Movimientos
            .Where(m => m.UsuarioId == usuarioId
                     && m.Quincena.Anio == anio
                     && m.Quincena.Mes == mes
                     && m.MovimientoPadreId == null
                     && (m.Tipo == TipoMovimiento.Pago || m.Tipo == TipoMovimiento.Gasto))
            .GroupBy(m => new { m.CategoriaId, CategoriaNombre = m.Categoria!.Nombre, Color = m.Categoria.Color, Presupuesto = m.Categoria.PresupuestoMensual })
            .Select(g => new ResumenCategoriaDto
            {
                CategoriaId = g.Key.CategoriaId,
                CategoriaNombre = g.Key.CategoriaNombre ?? "Sin categoría",
                Color = g.Key.Color,
                Total = g.Sum(m => m.Monto),
                PresupuestoMensual = g.Key.Presupuesto,
                PorcentajeUso = g.Key.Presupuesto.HasValue && g.Key.Presupuesto.Value > 0
                    ? Math.Round((g.Sum(m => m.Monto) / g.Key.Presupuesto.Value) * 100, 2)
                    : null
            })
            .OrderByDescending(r => r.Total)
            .ToListAsync();

        var nombreMes = CultureInfo.GetCultureInfo("es-EC")
            .DateTimeFormat.GetMonthName(mes);

        return new ResumenMensualDto
        {
            Mes = mes,
            Anio = anio,
            NombreMes = char.ToUpper(nombreMes[0]) + nombreMes[1..],
            TotalIngresos = totalIngresos,
            TotalDescuentos = totalDescuentos,
            NetoRecibir = neto,
            TotalPagos = totalPagos,
            TotalGastos = totalGastos,
            SaldoFinal = saldo,
            Quincenas = detalles,
            ResumenPorCategoria = resumenCat
        };
    }
}
