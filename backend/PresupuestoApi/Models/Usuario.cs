using System.ComponentModel.DataAnnotations;

namespace PresupuestoApi.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Navegación
    public ICollection<Quincena> Quincenas { get; set; } = new List<Quincena>();
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
