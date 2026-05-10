using System.ComponentModel.DataAnnotations;
using PresupuestoApi.Models.Enums;

namespace PresupuestoApi.DTOs.Categorias;

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoMovimiento Tipo { get; set; }
    public string? Color { get; set; }
    public string? CodigoRol { get; set; }
    public decimal? PresupuestoMensual { get; set; }
    public bool Activa { get; set; }
}

public class CrearCategoriaDto
{
    [Required, MaxLength(80)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public TipoMovimiento Tipo { get; set; }

    [MaxLength(20)]
    public string? Color { get; set; }

    [MaxLength(10)]
    public string? CodigoRol { get; set; }

    public decimal? PresupuestoMensual { get; set; }
}

public class ActualizarCategoriaDto
{
    [Required, MaxLength(80)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Color { get; set; }

    [MaxLength(10)]
    public string? CodigoRol { get; set; }

    public decimal? PresupuestoMensual { get; set; }

    public bool Activa { get; set; } = true;
}
