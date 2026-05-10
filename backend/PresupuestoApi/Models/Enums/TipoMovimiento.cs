namespace PresupuestoApi.Models.Enums;

public enum TipoMovimiento
{
    Ingreso = 1,    // Sueldo, décimo tercero, fondo de reserva, encargo
    Descuento = 2,  // IESS, préstamo quirografario (descuentos del rol)
    Pago = 3,       // Tarjetas, servicios, préstamos, pensiones (obligaciones fijas)
    Gasto = 4       // Gasolina, comida, medicina (variables del día a día)
}
