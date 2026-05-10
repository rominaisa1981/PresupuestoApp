using PresupuestoApi.DTOs.Quincenas;

namespace PresupuestoApi.DTOs.Resumen;

public class ResumenMensualDto
{
    public int Mes { get; set; }
    public int Anio { get; set; }
    public string NombreMes { get; set; } = string.Empty;

    public decimal TotalIngresos { get; set; }
    public decimal TotalDescuentos { get; set; }
    public decimal NetoRecibir { get; set; }
    public decimal TotalPagos { get; set; }
    public decimal TotalGastos { get; set; }
    public decimal SaldoFinal { get; set; }

    public List<QuincenaDetalleDto> Quincenas { get; set; } = new();
    public List<ResumenCategoriaDto> ResumenPorCategoria { get; set; } = new();
}

public class ResumenCategoriaDto
{
    public int? CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public string? Color { get; set; }
    public decimal Total { get; set; }
    public decimal? PresupuestoMensual { get; set; }
    /// <summary>
    /// Porcentaje de uso del presupuesto (si tiene presupuesto asignado)
    /// </summary>
    public decimal? PorcentajeUso { get; set; }
}
