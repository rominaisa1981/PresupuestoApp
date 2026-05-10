using System.ComponentModel.DataAnnotations;
using PresupuestoApi.Models.Enums;

namespace PresupuestoApi.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de movimiento al que aplica esta categoría
    /// </summary>
    [Required]
    public TipoMovimiento Tipo { get; set; }

    [MaxLength(20)]
    public string? Color { get; set; }

    /// <summary>
    /// Código del concepto en el rol de pagos (ej: 80 = Sueldo Unificado, 100 = IESS).
    /// Útil para importar automáticamente desde el PDF del rol.
    /// </summary>
    [MaxLength(10)]
    public string? CodigoRol { get; set; }

    /// <summary>
    /// Presupuesto mensual asignado (útil para gastos como gasolina $60, comida $200)
    /// </summary>
    public decimal? PresupuestoMensual { get; set; }

    public bool Activa { get; set; } = true;

    // FK
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    // Navegación
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
