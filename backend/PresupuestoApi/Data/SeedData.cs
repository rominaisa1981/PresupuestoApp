using PresupuestoApi.Models;
using PresupuestoApi.Models.Enums;

namespace PresupuestoApi.Data;

/// <summary>
/// Crea categorías predeterminadas para un usuario nuevo.
/// Los códigos (CodigoRol) corresponden a los conceptos del rol de pagos
/// del Municipio de Guayaquil (LOSEP), útiles para importación automática
/// desde el PDF del rol.
/// </summary>
public static class SeedData
{
    public static List<Categoria> CategoriasIniciales(int usuarioId) => new()
    {
        // ============= INGRESOS (haberes del rol) =============
        new() { Nombre = "Sueldo Unificado", Tipo = TipoMovimiento.Ingreso, Color = "#10b981", CodigoRol = "80", UsuarioId = usuarioId },
        new() { Nombre = "Sueldo Unificado Quincenal", Tipo = TipoMovimiento.Ingreso, Color = "#10b981", CodigoRol = "157", UsuarioId = usuarioId },
        new() { Nombre = "Décimo Tercer Sueldo", Tipo = TipoMovimiento.Ingreso, Color = "#059669", CodigoRol = "18", UsuarioId = usuarioId },
        new() { Nombre = "Décimo Cuarto Sueldo", Tipo = TipoMovimiento.Ingreso, Color = "#059669", CodigoRol = "19", UsuarioId = usuarioId },
        new() { Nombre = "Fondo de Reserva", Tipo = TipoMovimiento.Ingreso, Color = "#047857", CodigoRol = "37", UsuarioId = usuarioId },
        new() { Nombre = "Encargo de Funciones", Tipo = TipoMovimiento.Ingreso, Color = "#34d399", CodigoRol = "35", UsuarioId = usuarioId },
        new() { Nombre = "Otros Ingresos", Tipo = TipoMovimiento.Ingreso, Color = "#6ee7b7", UsuarioId = usuarioId },

        // ============= DESCUENTOS (descuentos del rol) =============
        new() { Nombre = "Anticipo Sueldo Unificado", Tipo = TipoMovimiento.Descuento, Color = "#f59e0b", CodigoRol = "91", UsuarioId = usuarioId },
        new() { Nombre = "Aporte Individual IESS", Tipo = TipoMovimiento.Descuento, Color = "#f59e0b", CodigoRol = "100", UsuarioId = usuarioId },
        new() { Nombre = "Póliza de Fidelidad", Tipo = TipoMovimiento.Descuento, Color = "#fbbf24", CodigoRol = "148", UsuarioId = usuarioId },
        new() { Nombre = "Préstamo Quirografario", Tipo = TipoMovimiento.Descuento, Color = "#d97706", CodigoRol = "152", UsuarioId = usuarioId },
        new() { Nombre = "Préstamo Hipotecario", Tipo = TipoMovimiento.Descuento, Color = "#d97706", CodigoRol = "153", UsuarioId = usuarioId },
        new() { Nombre = "Otros Descuentos", Tipo = TipoMovimiento.Descuento, Color = "#fbbf24", UsuarioId = usuarioId },

        // ============= PAGOS (obligaciones fijas extra-rol) =============
        new() { Nombre = "Tarjeta Diners", Tipo = TipoMovimiento.Pago, Color = "#ef4444", UsuarioId = usuarioId },
        new() { Nombre = "Tarjeta Produbanco", Tipo = TipoMovimiento.Pago, Color = "#ef4444", UsuarioId = usuarioId },
        new() { Nombre = "Tarjeta Banco del Austro", Tipo = TipoMovimiento.Pago, Color = "#ef4444", UsuarioId = usuarioId },
        new() { Nombre = "Tarjeta De Prati", Tipo = TipoMovimiento.Pago, Color = "#ef4444", UsuarioId = usuarioId },
        new() { Nombre = "Tarjeta Visa", Tipo = TipoMovimiento.Pago, Color = "#ef4444", UsuarioId = usuarioId },
        new() { Nombre = "Luz", Tipo = TipoMovimiento.Pago, Color = "#fbbf24", UsuarioId = usuarioId },
        new() { Nombre = "Agua", Tipo = TipoMovimiento.Pago, Color = "#3b82f6", UsuarioId = usuarioId },
        new() { Nombre = "Internet", Tipo = TipoMovimiento.Pago, Color = "#06b6d4", UsuarioId = usuarioId },
        new() { Nombre = "Préstamos Personales", Tipo = TipoMovimiento.Pago, Color = "#dc2626", UsuarioId = usuarioId },
        new() { Nombre = "Pensión Universidad", Tipo = TipoMovimiento.Pago, Color = "#8b5cf6", UsuarioId = usuarioId },
        new() { Nombre = "Alícuota", Tipo = TipoMovimiento.Pago, Color = "#84cc16", UsuarioId = usuarioId },
        new() { Nombre = "Expreso/Transporte", Tipo = TipoMovimiento.Pago, Color = "#06b6d4", UsuarioId = usuarioId },
        new() { Nombre = "Deuda Familiar", Tipo = TipoMovimiento.Pago, Color = "#a855f7", UsuarioId = usuarioId },

        // ============= GASTOS (variables, con presupuesto) =============
        new() { Nombre = "Gasolina", Tipo = TipoMovimiento.Gasto, Color = "#f97316", PresupuestoMensual = 60m, UsuarioId = usuarioId },
        new() { Nombre = "Comida", Tipo = TipoMovimiento.Gasto, Color = "#eab308", PresupuestoMensual = 200m, UsuarioId = usuarioId },
        new() { Nombre = "Medicina", Tipo = TipoMovimiento.Gasto, Color = "#ec4899", UsuarioId = usuarioId },
        new() { Nombre = "Mesada", Tipo = TipoMovimiento.Gasto, Color = "#a855f7", UsuarioId = usuarioId },
        new() { Nombre = "Otros Gastos", Tipo = TipoMovimiento.Gasto, Color = "#64748b", UsuarioId = usuarioId },
    };
}
