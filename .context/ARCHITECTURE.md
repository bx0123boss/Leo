# System Architecture

This document describes the architecture of the Jaegersoft POS system as it stands after the
Access → SQL Server migration (branch `feature/migracion-sqlserver-total`).

## Modules Overview

```
┌───────────────────────────────────────────────────────────────────┐
│                        Jaegersoft POS                             │
├───────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────────┐          ┌───────────────────────────────┐  │
│  │      BRUNO       │◄────────►│        PuntoVentaWeb          │  │
│  │   (WinForms)     │          │      (Blazor Server)          │  │
│  │   .NET 4.8       │          │      net8.0-windows           │  │
│  │                  │          │                               │  │
│  │  Sales, cash,    │          │  Web POS for the local        │  │
│  │  cuts, stock,    │          │  network (quotes, sales,      │  │
│  │  credit, layaway │          │  consignments)                │  │
│  └────────┬─────────┘          └──────────┬────────────────────┘  │
│           │                               │                       │
│           │      ┌────────────────────────┴───────────┐           │
│           └─────►│        TicketJaegersoft            │           │
│                  │        (printing, net472)          │           │
│                  └────────────────┬───────────────────┘           │
│                                   │                               │
│                  ┌────────────────┴───────────────────┐           │
│                  │      AgenteBasculaTorrey           │           │
│                  │   (scale agent, net9.0, HTTP)      │           │
│                  └────────────────┬───────────────────┘           │
│                                   │                               │
│                  ┌────────────────┴───────────────────┐           │
│                  │            SQL Server 2017         │           │
│                  │      PuntoDeVenta  (.\SQLEXPRESS)  │           │
│                  │      Single source of truth        │           │
│                  └────────────────────────────────────┘           │
│                                                                   │
│   Access 2007 (Jaeger.accdb) ── legacy ──► ETL ──► SQL Server     │
└───────────────────────────────────────────────────────────────────┘
```

## Module dependencies

- **PuntoVentaWeb** → references `BRUNO/Jaegersoft.csproj` and `TicketJaegersoft.csproj`
  (it reuses BRUNO services and the printing library).
- **BRUNO** → references `TicketJaegersoft.csproj`.
- **AgenteBasculaTorrey** → standalone console app, exposed over HTTP (`localhost:8080`).

## Data access layer

All database access goes through **`System.Data.SqlClient`**:

| Concern | Where |
|---------|-------|
| Connection string | `BRUNO/Conexion.cs` → `public static string CadSQL` |
| Configuration bootstrap | `Conexion.CargarConfiguracion(id)` reads the `Configuracion` table |
| Web connection string | `PuntoVentaWeb/appsettings.json` → `ConnectionStrings:SQL` |
| Data access | `SqlConnection` / `SqlCommand` / `SqlDataAdapter` / `SqlDataReader` / `SqlTransaction` |

**Exceptions (intentional):**

- `frmCapturaInicial.cs` and `frmFisico.cs` keep `OleDbConnection` with the ACE provider to read
  **Excel files** (`Extended Properties='Excel 12.0 Xml'`). That is file reading, not database access.
- `PuntoVentaWeb/appsettings.json` still carries an `Access` connection string for the legacy
  import paths that have not been removed yet.

There is **no ODBC** (`System.Data.Odbc`) left in the codebase.

## Database schema

`PuntoDeVenta` contains:

- **57 tables migrated 1:1 from Access** (`Inventario`, `Ventas`, `Ventas2`, `Kardex`, `Cortes`,
  `histocortes`, `Clientes`, `Abonos`, `ConsignaCliente`, …), with **identical names, column names
  and ordinal order**.
- **Web-only tables** added by `PuntoVentaWeb` (`Consignaciones`, `Cotizaciones`,
  `DetalleCotizacion`, `HistorialLiquidaciones`, …), which the ETL never touches.

Rules that must keep holding:

1. **No renaming and no reordering.** Legacy code reads by ordinal (`reader[0]`, `SELECT *`).
2. **Migration is data-only.** The ETL (`docs/migracion-sqlserver/herramientas/migrar_access_a_sql.py`)
   never creates or alters schema; it truncates, loads with `IDENTITY_INSERT`, verifies and reseeds.
3. **Legacy types are preserved.** Some money columns are `nvarchar` because they were text in
   Access (`Ventas.Monto`, `VentasPagos.Monto`, `histocortes.Monto/Utilidad`, `Kardex.Precio`,
   `Abonos.Abono`, `VentasCajero.Total`, `Proveedores.Adeudo`, …). SQL-side arithmetic on them
   requires `CAST`/`TRY_CAST`; Jet used to coerce implicitly, SQL Server does not.
4. **Dates go as ISO.** The server runs `us_english` (`mdy`); every SQL date literal must be
   `yyyy-MM-dd HH:mm:ss`, otherwise it fails (error 242) or stores a wrong date.

## Desktop application (BRUNO)

- WinForms, .NET Framework 4.8, single-threaded UI.
- `frmBase` is the visual base class: `EstilizarDataGridView()`, `EstilizarBotonPrimario()`,
  `EstilizarBotonPeligro()`, dark theme.
- Forms talk to SQL directly (no repository layer): each screen builds its own `SqlCommand`,
  usually with `using` blocks and transactions for multi-table writes (sales, cuts, consignments).
- Business flows that write several tables run inside a `SqlTransaction`
  (`frmVentas`, `frmVentaDetallada`, `frmCorte`, `frmConsigna*`, `frmVentasCredito`).

## Web application (PuntoVentaWeb)

- Blazor Server, `net8.0-windows`, Kestrel listening on **HTTP `0.0.0.0:5000`**
  (`Program.cs` → `ListenAnyIP(5000)`; `appsettings.json` → `Kestrel:Endpoints:Http`).
- HTTPS endpoint `0.0.0.0:5001` is configured in `launchSettings.json` with a self-signed
  certificate (`certificado_pv.pfx`) for development.
- Services (`VentaService`, `ClienteService`, `CotizacionService`, `ProduccionService`,
  `ImpresionPosService`) use `Microsoft.Data.SqlClient` and named parameters.
- Talks to the scale through HTTP to `AgenteBasculaTorrey` on `localhost:8080`.

## Build and deployment

- Solution: `Jaegersoft Breackpoint.sln` (4 projects).
- Build with **Visual Studio 2022 MSBuild**; the .NET Framework 4.0 MSBuild cannot load the
  SDK-style projects (`MSB4041`) and `dotnet build` fails when it must regenerate WinForms
  resources (`MSB3822`/`MSB3823`).
- `AgenteBasculaTorrey` targets `net9.0`, `PuntoVentaWeb` targets `net8.0-windows`: the machine
  needs both the .NET 8 and .NET 9 runtimes.
- Release builds and the final Access decommission are Phase 6 of the migration plan
  (see `docs/migracion-sqlserver/PLAN_MIGRACION_SQLSERVER.md`).

## Security considerations

- Use **parameterized queries with named parameters**; never concatenate user input.
- Dispose connections/commands with `using`; keep the current pattern of one connection per form.
- Windows Authentication is used for SQL Server (`Integrated Security=True`); there is no SQL
  login with a stored password.
- The Access Jet password (`75941232`) is still hardcoded in `appsettings.json`/`Conexion.cs`
  legacy paths; it disappears when Access is retired (Phase 6).
- `TrustServerCertificate=True` is required only because the local instance uses a self-signed
  certificate.
