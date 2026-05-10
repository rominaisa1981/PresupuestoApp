# Presupuesto API (Backend)

API REST en ASP.NET Core 8 + Entity Framework Core + SQL Server Express para llevar el presupuesto mensual por quincenas.

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express (ya lo tienes)
- (Opcional) Visual Studio 2022 / VS Code / Rider

## Instalación paso a paso

### 1. Crear el proyecto y copiar archivos

Abre una terminal y crea el proyecto vacío para tener el archivo `.csproj` correctamente registrado:

```bash
# Ve a tu carpeta de proyectos
cd C:\Proyectos

# Crea la solución y el proyecto
mkdir PresupuestoApp
cd PresupuestoApp
dotnet new sln -n PresupuestoApp
mkdir backend
cd backend
```

Luego, **copia toda la carpeta `PresupuestoApi` que te pasé dentro de `backend/`**, de modo que quede así:

```
PresupuestoApp/
└── backend/
    └── PresupuestoApi/
        ├── Controllers/
        ├── Models/
        ├── Data/
        ├── ...
        ├── Program.cs
        └── PresupuestoApi.csproj
```

Después agrega el proyecto a la solución:

```bash
cd ..   # vuelve a la raíz PresupuestoApp
dotnet sln add backend/PresupuestoApi/PresupuestoApi.csproj
```

### 2. Restaurar paquetes NuGet

```bash
cd backend/PresupuestoApi
dotnet restore
```

### 3. Configurar la cadena de conexión

Edita `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PresupuestoDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

> Si tu SQL Server Express tiene otro nombre de instancia, cambia `localhost\\SQLEXPRESS`. Puedes verificarlo en SSMS conectándote y mirando el nombre del servidor.

### 4. Cambiar la clave JWT

En `appsettings.json` reemplaza el valor de `Jwt:Key` por una cadena larga aleatoria (mínimo 32 caracteres). Puedes generar una con PowerShell:

```powershell
[Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
```

### 5. Crear la base de datos con migraciones EF Core

Instala la herramienta global de EF Core (solo la primera vez):

```bash
dotnet tool install --global dotnet-ef
```

Crea la migración inicial y aplícala:

```bash
dotnet ef migrations add InicialCreacion
dotnet ef database update
```

> El proyecto también está configurado para aplicar migraciones automáticamente al iniciar (`db.Database.Migrate()` en `Program.cs`), pero es buena idea correr `database update` la primera vez para confirmar que funciona.

### 6. Ejecutar la API

```bash
dotnet run
```

Se abrirá en `https://localhost:7100`. La interfaz de Swagger estará en `https://localhost:7100/swagger`.

## Cómo probarla

### 1. Registrar usuario

`POST /api/auth/registro`

```json
{
  "email": "tu@email.com",
  "nombre": "Romina",
  "password": "TuPassword123"
}
```

Esto te devuelve un `token` y crea automáticamente las **categorías iniciales** basadas en tu Excel (Sueldo, Décimo Tercero, Tarjetas, Gasolina con presupuesto $60, Comida con $200, etc.).

### 2. Autorizar en Swagger

En Swagger UI, click en "Authorize" arriba a la derecha y pega: `Bearer {tu_token}`.

### 3. Crear una quincena

`POST /api/quincenas`

```json
{
  "fechaPago": "2026-04-30",
  "numero": 2,
  "mes": 4,
  "anio": 2026,
  "observaciones": "Quincena fin de mes con décimo"
}
```

### 4. Listar tus categorías

`GET /api/categorias` te devuelve las 19 categorías predeterminadas.

### 5. Agregar movimientos

Primero el ingreso principal:

`POST /api/movimientos`

```json
{
  "descripcion": "Sueldo Romina",
  "monto": 1450.80,
  "tipo": "Ingreso",
  "quincenaId": 1,
  "categoriaId": 1
}
```

Para agregar el préstamo quirografario con sub-cuotas, primero el padre:

```json
{
  "descripcion": "Préstamo Quirografario",
  "monto": 573.55,
  "tipo": "Descuento",
  "quincenaId": 1,
  "categoriaId": 7
}
```

Y luego cada PQ como hijo (usa el id del padre que te devolvió):

```json
{
  "descripcion": "PQ 1",
  "monto": 133.52,
  "tipo": "Descuento",
  "quincenaId": 1,
  "movimientoPadreId": 5
}
```

### 6. Ver el resumen mensual

`GET /api/resumen/mensual/2026/4`

Te devuelve un objeto con totales del mes, ambas quincenas y resumen por categoría con porcentaje de uso del presupuesto.

## Endpoints disponibles

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/auth/registro` | Crear usuario |
| POST | `/api/auth/login` | Login |
| GET | `/api/categorias?tipo=Gasto` | Listar categorías (filtro opcional) |
| POST | `/api/categorias` | Crear categoría |
| PUT | `/api/categorias/{id}` | Actualizar categoría |
| DELETE | `/api/categorias/{id}` | Desactivar categoría |
| GET | `/api/quincenas?anio=2026&mes=4` | Listar quincenas |
| GET | `/api/quincenas/{id}` | Detalle con todos los movimientos y totales |
| POST | `/api/quincenas` | Crear quincena |
| DELETE | `/api/quincenas/{id}` | Eliminar quincena |
| GET | `/api/movimientos/{id}` | Obtener movimiento |
| POST | `/api/movimientos` | Crear movimiento |
| PUT | `/api/movimientos/{id}` | Actualizar |
| DELETE | `/api/movimientos/{id}` | Eliminar |
| GET | `/api/resumen/mensual/{anio}/{mes}` | Resumen mensual completo |

## Tipos de movimientos (enum)

- `Ingreso` (1) → Sueldo, décimo, fondo de reserva, encargo
- `Descuento` (2) → IESS, préstamos quirografarios (descuentos del rol)
- `Pago` (3) → Tarjetas, servicios, préstamos, pensiones (obligaciones fijas)
- `Gasto` (4) → Gasolina, comida, medicina (variables)

**Cálculos:**
- Neto a Recibir = Total Ingresos − Total Descuentos
- Saldo = Neto a Recibir − Total Pagos − Total Gastos

## Estructura del proyecto

```
PresupuestoApi/
├── Controllers/        # Endpoints REST
├── Models/             # Entidades de EF Core
│   └── Enums/
├── Data/               # DbContext y SeedData
├── DTOs/               # Objetos de transferencia
├── Services/           # Lógica de negocio
├── Middleware/         # Manejo global de excepciones
├── Helpers/            # Extensiones útiles
├── Program.cs          # Configuración de la app
└── appsettings.json    # Configuración
```

## Próximos pasos

1. ✅ Backend listo
2. 🔜 Frontend Angular conectado a este API
3. 🔜 Despliegue (Azure SQL + cualquier hosting de .NET, o IIS local)

---

**Notas importantes**

- Los montos se guardan siempre **positivos**. El signo lo determina el `tipo` del movimiento al hacer los cálculos.
- Cuando un movimiento tiene sub-items (movimientos hijos), **solo se suma el monto del padre** en los totales — los hijos son detalle informativo.
- El soft-delete de categorías (`Activa = false`) preserva tu histórico aunque ya no uses esa categoría.
