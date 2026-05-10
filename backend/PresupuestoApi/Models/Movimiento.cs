using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PresupuestoApi.Models.Enums;

namespace PresupuestoApi.Models;

/// <summary>
/// Representa cualquier movimiento de la quincena: ingreso, descuento, pago o gasto.
/// Usa auto-referencia (MovimientoPadreId) para manejar sub-items como
/// PQ1, PQ2... dentro de "Préstamo Quirografario", o "confiamed" dentro de "Tarjeta Diners".
/// </summary>
public class Movimiento
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Monto del movimiento. Siempre se almacena como POSITIVO.
    /// El signo lo da el TipoMovimiento (Descuento, Pago, Gasto restan; Ingreso suma).
    /// </summary>
    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal Monto { get; set; }

    [Required]
    public TipoMovimiento Tipo { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Notas { get; set; }

    // FKs
    public int QuincenaId { get; set; }
    public Quincena Quincena { get; set; } = null!;

    public int? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    // Auto-referencia para sub-items (jerarquía padre/hijo)
    public int? MovimientoPadreId { get; set; }
    public Movimiento? MovimientoPadre { get; set; }
    public ICollection<Movimiento> SubMovimientos { get; set; } = new List<Movimiento>();
}
