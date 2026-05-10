using System.ComponentModel.DataAnnotations;
using PresupuestoApi.Models.Enums;

namespace PresupuestoApi.DTOs.Movimientos;

public class MovimientoDto
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public DateTime Fecha { get; set; }
    public string? Notas { get; set; }
    public int QuincenaId { get; set; }
    public int? CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
    public int? MovimientoPadreId { get; set; }
    public List<MovimientoDto> SubMovimientos { get; set; } = new();
}

public class CrearMovimientoDto
{
    [Required, MaxLength(200)]
    public string Descripcion { get; set; } = string.Empty;

    [Required, Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Monto { get; set; }

    [Required]
    public TipoMovimiento Tipo { get; set; }

    [Required]
    public int QuincenaId { get; set; }

    public int? CategoriaId { get; set; }

    public int? MovimientoPadreId { get; set; }

    public DateTime? Fecha { get; set; }

    [MaxLength(500)]
    public string? Notas { get; set; }
}

public class ActualizarMovimientoDto
{
    [Required, MaxLength(200)]
    public string Descripcion { get; set; } = string.Empty;

    [Required, Range(0.01, double.MaxValue)]
    public decimal Monto { get; set; }

    public int? CategoriaId { get; set; }

    [MaxLength(500)]
    public string? Notas { get; set; }
}
