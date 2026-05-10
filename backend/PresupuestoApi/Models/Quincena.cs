using System.ComponentModel.DataAnnotations;

namespace PresupuestoApi.Models;

public class Quincena
{
    public int Id { get; set; }

    [Required]
    public DateTime FechaPago { get; set; }

    /// <summary>
    /// 1 = primera quincena del mes (15), 2 = segunda quincena (fin de mes)
    /// </summary>
    [Required, Range(1, 2)]
    public int Numero { get; set; }

    [Required, Range(1, 12)]
    public int Mes { get; set; }

    [Required]
    public int Anio { get; set; }

    [MaxLength(250)]
    public string? Observaciones { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // FK
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    // Navegación
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
