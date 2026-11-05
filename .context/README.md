# Jaegersoft POS — Knowledge Base

This directory contains structured documentation about the Jaegersoft POS project. All content is written in Markdown so both developers and AI agents can consume it.

## Project Overview

The Jaegersoft POS is a **Point of Sale system** built in **C#**, made of four projects:

| Project | Type | Target | Role |
|---------|------|--------|------|
| **BRUNO** (`JaegerSoft BreackPoint.exe`) | Windows Forms | .NET Framework **4.8** | Main desktop POS: sales, cash drawer, cuts, inventory, customers, layaways, credit, consignments |
| **PuntoVentaWeb** | Blazor Server | **net8.0-windows** | Web POS for the local network; references `BRUNO` and `TicketJaegersoft` |
| **TicketJaegersoft** | Class library | **net472** | Ticket/receipt printing used by both BRUNO and the web app |
| **AgenteBasculaTorrey** | Console app | **net9.0** | Reads Torrey scale hardware over the serial port and exposes it over HTTP |

**Database:** since the Access → SQL Server migration, **SQL Server is the single source of truth**
(`PuntoDeVenta` on `.\SQLEXPRESS`, Windows Authentication). `C:\Jaeger Soft\Jaeger.accdb` is kept
only as the legacy origin for the data migration.

## Repository Layout

```
BRUNO/                       WinForms desktop POS (.NET Framework 4.8)
PuntoVentaWeb/               Blazor Server web POS (net8.0-windows)
TicketJaegersoft/            Shared printing library (net472)
AgenteBasculaTorrey/         Scale agent (net9.0)
docs/migracion-sqlserver/    Migration plan, status, ETL tooling and operational guides
.context/                    This knowledge base
```

## Documentation Index

| File | Description |
|------|-------------|
| [ARCHITECTURE.md](./ARCHITECTURE.md) | System architecture, module dependencies, data access layer |
| [CLOUDFLARE_TUNNEL_PLAN.md](./CLOUDFLARE_TUNNEL_PLAN.md) | Multi-installation deployment plan using Cloudflare Tunnels |
| [../docs/migracion-sqlserver/PLAN_MIGRACION_SQLSERVER.md](../docs/migracion-sqlserver/PLAN_MIGRACION_SQLSERVER.md) | Master plan of the Access → SQL Server migration |
| [../docs/migracion-sqlserver/ESTADO_AVANCE_MIGRACION.md](../docs/migracion-sqlserver/ESTADO_AVANCE_MIGRACION.md) | Living status: phases, checklists, session log |
| [../docs/migracion-sqlserver/COMO_MIGRAR_DATOS.md](../docs/migracion-sqlserver/COMO_MIGRAR_DATOS.md) | How to run the ETL (dry-run, verify, production run) |
| [../docs/migracion-sqlserver/herramientas/](../docs/migracion-sqlserver/herramientas/) | ETL script, schema clone for tests, query validation and helper tools |

## Quick Reference

### Connection strings

SQL Server (used by everything):

```csharp
// BRUNO/Conexion.cs
public static string CadSQL = @"Data Source=.\SQLEXPRESS;Initial Catalog=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;";
```

```json
// PuntoVentaWeb/appsettings.json
"ConnectionStrings": {
  "SQL": "Server=.\\SQLEXPRESS;Database=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True;"
}
```

Access (legacy, only for the ETL and the Excel import readers):

```
Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=C:\Jaeger Soft\Jaeger.accdb;Pwd=75941232;
```

### Build

Use **Visual Studio 2022 MSBuild** (the .NET Framework 4.0 MSBuild rejects the SDK-style projects):

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" `
    "Jaegersoft Breackpoint.sln" /t:Rebuild /p:Configuration=Debug /m
```

> `dotnet build` also compiles the solution, but if it has to regenerate the WinForms `.resx`
> files it fails with `MSB3822`/`MSB3823`. Stick to the VS 2022 MSBuild for full rebuilds.

### Run

```powershell
# Desktop POS
.\BRUNO\bin\Debug\JaegerSoft BreackPoint.exe

# Web POS (http://0.0.0.0:5000, HTTP only)
dotnet run --project PuntoVentaWeb\PuntoVentaWeb.csproj
```

### Data migration (Access → SQL Server)

```powershell
# 1. Simulate (writes nothing)
python docs\migracion-sqlserver\herramientas\migrar_access_a_sql.py --mode dry-run

# 2. Compare Access against SQL (writes nothing)
python docs\migracion-sqlserver\herramientas\migrar_access_a_sql.py --mode verify --deep-check

# 3. Production run (POS closed, with per-table backup)
python docs\migracion-sqlserver\herramientas\migrar_access_a_sql.py --mode load --backup --yes --deep-check
```

Requires `pip install pyodbc` plus the 64-bit `Microsoft Access Driver (*.mdb, *.accdb)` and
`ODBC Driver 17 for SQL Server`. See [COMO_MIGRAR_DATOS.md](../docs/migracion-sqlserver/COMO_MIGRAR_DATOS.md).

## Non-negotiable conventions

1. **Never rename tables or columns** and **never change the ordinal order of the columns**.
   Legacy WinForms code reads rows by index (`reader[0]`, `SELECT *`), so a reordering silently
   corrupts data.
2. **Every SQL date must be formatted as `yyyy-MM-dd HH:mm:ss`.** The server runs with
   `us_english` (`mdy`), so `dd/MM/yyyy` either fails (error 242) or stores the wrong date.
   Never use `ToShortDateString()` for SQL.
3. **Parameterized queries only** (`SqlCommand` + named `@parameter`). SQL Server does not accept
   the Access positional `?` marker.
4. Some money columns (`Ventas.Monto`, `VentasPagos.Monto`, `histocortes.Monto`, …) are `nvarchar`
   because in Access they were text. Any `SUM`/arithmetic on them needs
   `CAST(REPLACE(col,'$','') AS DECIMAL(18,2))` or `TRY_CAST`.
5. Excel import readers (`frmCapturaInicial`, `frmFisico`) legitimately keep `OleDbConnection`
   against the ACE provider — that is file reading, not database access.
