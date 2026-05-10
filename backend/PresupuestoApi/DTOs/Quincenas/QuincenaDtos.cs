using System.ComponentModel.DataAnnotations;
using PresupuestoApi.DTOs.Movimientos;

namespace PresupuestoApi.DTOs.Quincenas;

public class QuincenaDto
{
    public int Id { get; set; }
    public DateTime FechaPago { get; set; }
    public int Numero { get; set; }
    public int Mes { get; set; }
    public int Anio { get; set; }
    public string? Observaciones { get; set; }
}

public class QuincenaDetalleDto : QuincenaDto
{
    public decimal TotalIngresos { get; set; }
    public decimal TotalDescuentos { get; set; }
    public decimal NetoRecibir { get; set; }
    public decimal TotalPagos { get; set; }
    public decimal TotalGastos { get; set; }
    public decimal Saldo { get; set; }

    public List<MovimientoDto> Ingresos { get; set; } = new();
    public List<MovimientoDto> Descuentos { get; set; } = new();
    public List<MovimientoDto> Pagos { get; set; } = new();
    public List<MovimientoDto> Gastos { get; set; } = new();
}

public class CrearQuincenaDto
{
    [Required]
    public DateTime FechaPago { get; set; }

    [Required, Range(1, 2)]
    public int Numero { get; set; }

    [Required, Range(1, 12)]
    public int Mes { get; set; }

    [Required, Range(2020, 2100)]
    public int Anio { get; set; }

    [MaxLength(250)]
    public string? Observaciones { get; set; }
}

public class CopiarQuincenaDto
{
    [Required, Range(1, 12)]
    public int Mes { get; set; }

    [Required, Range(2020, 2100)]
    public int Anio { get; set; }

    /// <summary>
    /// Fecha de pago de la nueva quincena. Si no se envía, se usa la misma del original.
    /// </summary>
    public DateTime? FechaPago { get; set; }
}
