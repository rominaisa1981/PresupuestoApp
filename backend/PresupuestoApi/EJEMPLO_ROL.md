# Ejemplo: registrar tus quincenas de Abril 2026

Este documento muestra cómo cargar las quincenas reales basadas en tus roles del Municipio.

## 1era Quincena de Abril 2026 (15/04/2026)

### Crear la quincena
`POST /api/quincenas`
```json
{
  "fechaPago": "2026-04-15",
  "numero": 1,
  "mes": 4,
  "anio": 2026,
  "observaciones": "Anticipo quincenal LOSEP"
}
```

### Movimientos del rol
Solo hay un haber, sin descuentos:

`POST /api/movimientos` — guarda el id de la quincena que te devolvió arriba (asumamos `quincenaId: 1`)
```json
{
  "descripcion": "Sueldo Unificado Quincenal (COD 157)",
  "monto": 967.20,
  "tipo": "Ingreso",
  "quincenaId": 1,
  "categoriaId": 2
}
```
> El `categoriaId: 2` es el de "Sueldo Unificado Quincenal" — verifícalo con `GET /api/categorias?tipo=Ingreso`.

**Resultado esperado en `GET /api/quincenas/1`:**
- TotalIngresos: 967.20
- TotalDescuentos: 0
- NetoRecibir: 967.20
- Saldo: 967.20 (antes de pagos y gastos)

---

## 2da Quincena de Abril 2026 (30/04/2026)

### Crear la quincena
`POST /api/quincenas`
```json
{
  "fechaPago": "2026-04-30",
  "numero": 2,
  "mes": 4,
  "anio": 2026,
  "observaciones": "Sueldo completo con décimo y fondo de reserva"
}
```

Asumamos que el id resultante es `quincenaId: 2`.

### Ingresos (haberes del rol)

```json
{ "descripcion": "Sueldo Unificado",   "monto": 2418.00, "tipo": "Ingreso", "quincenaId": 2, "categoriaId": 1 }
{ "descripcion": "Décimo Tercer",      "monto":  270.58, "tipo": "Ingreso", "quincenaId": 2, "categoriaId": 3 }
{ "descripcion": "Encargo Funciones",  "monto":  829.00, "tipo": "Ingreso", "quincenaId": 2, "categoriaId": 6 }
{ "descripcion": "Fondo de Reserva",   "monto":  270.48, "tipo": "Ingreso", "quincenaId": 2, "categoriaId": 5 }
```

### Descuentos del rol

```json
{ "descripcion": "Anticipo Sueldo Unificado", "monto": 967.20, "tipo": "Descuento", "quincenaId": 2, "categoriaId": 8 }
{ "descripcion": "Aporte Individual IESS",    "monto": 371.78, "tipo": "Descuento", "quincenaId": 2, "categoriaId": 9 }
{ "descripcion": "Póliza de Fidelidad",       "monto":   0.15, "tipo": "Descuento", "quincenaId": 2, "categoriaId": 10 }
```

### Préstamo Quirografario con sub-cuotas (jerarquía padre/hijo)

Primero el padre — anota el `id` que devuelva, llamémoslo `prestamoId`:
```json
{
  "descripcion": "Préstamo Quirografario",
  "monto": 561.30,
  "tipo": "Descuento",
  "quincenaId": 2,
  "categoriaId": 11
}
```

Luego cada PQ como hijo (reemplaza `MOVIMIENTO_PADRE_ID` por el id real):
```json
{ "descripcion": "PQ 1", "monto": 133.52, "tipo": "Descuento", "quincenaId": 2, "movimientoPadreId": MOVIMIENTO_PADRE_ID }
{ "descripcion": "PQ 2", "monto":  38.80, "tipo": "Descuento", "quincenaId": 2, "movimientoPadreId": MOVIMIENTO_PADRE_ID }
{ "descripcion": "PQ 3", "monto": 324.70, "tipo": "Descuento", "quincenaId": 2, "movimientoPadreId": MOVIMIENTO_PADRE_ID }
{ "descripcion": "PQ 4", "monto":  29.75, "tipo": "Descuento", "quincenaId": 2, "movimientoPadreId": MOVIMIENTO_PADRE_ID }
{ "descripcion": "PQ 5", "monto":  22.87, "tipo": "Descuento", "quincenaId": 2, "movimientoPadreId": MOVIMIENTO_PADRE_ID }
{ "descripcion": "PQ 6", "monto":   7.70, "tipo": "Descuento", "quincenaId": 2, "movimientoPadreId": MOVIMIENTO_PADRE_ID }
{ "descripcion": "PQ 7", "monto":   9.11, "tipo": "Descuento", "quincenaId": 2, "movimientoPadreId": MOVIMIENTO_PADRE_ID }
```

> Como los hijos no se suman al total (solo el padre), el detalle queda registrado pero no afecta los cálculos.

### Pagos (obligaciones extra-rol que tú asignas a esta quincena)

```json
{ "descripcion": "Tarjeta Diners",        "monto": 694.00, "tipo": "Pago", "quincenaId": 2, "categoriaId": 14 }
{ "descripcion": "Tarjeta De Prati",      "monto":  55.86, "tipo": "Pago", "quincenaId": 2, "categoriaId": 17 }
{ "descripcion": "Tarjeta Banco Austro",  "monto": 496.00, "tipo": "Pago", "quincenaId": 2, "categoriaId": 16 }
{ "descripcion": "Agua",                  "monto":   9.70, "tipo": "Pago", "quincenaId": 2, "categoriaId": 21 }
{ "descripcion": "Internet",              "monto":  21.56, "tipo": "Pago", "quincenaId": 2, "categoriaId": 22 }
{ "descripcion": "Expreso",               "monto":  25.00, "tipo": "Pago", "quincenaId": 2, "categoriaId": 26 }
{ "descripcion": "Pensión Universidad",   "monto": 323.00, "tipo": "Pago", "quincenaId": 2, "categoriaId": 24 }
```

### Gastos variables

```json
{ "descripcion": "Mesada", "monto": 50.00, "tipo": "Gasto", "quincenaId": 2, "categoriaId": 31 }
```

---

## Resultado: GET /api/quincenas/2

Deberías obtener algo como:

```json
{
  "totalIngresos": 3788.06,
  "totalDescuentos": 1900.43,
  "netoRecibir": 1887.63,
  "totalPagos": 1625.12,
  "totalGastos": 50.00,
  "saldo": 212.51
}
```

> El **netoRecibir 1887.63** coincide exactamente con el "NETO A PAGAR" del PDF del rol. ✓

## Resumen mensual

`GET /api/resumen/mensual/2026/4`

Te dará el consolidado de las dos quincenas:
- TotalIngresos: 4,755.26 (967.20 + 3,788.06)
- TotalDescuentos: 1,900.43
- NetoRecibir: 2,854.83
- TotalPagos: 1,625.12
- TotalGastos: 50.00
- SaldoFinal: 1,179.71

---

## Notas

Los `categoriaId` que ves arriba son **referenciales** y dependen del orden en que se creen las categorías al registrar tu usuario. Después del registro, llama a `GET /api/categorias` para ver los ids reales y úsalos en lugar de los del ejemplo.
