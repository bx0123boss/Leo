# System Architecture

This document describes the overall architecture of the Jaegersoft POS system.

## Modules Overview

```
┌─────────────────────────────────────────────────────────────┐
│                       Jaegersoft POS                         │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────────┐    ┌───────────────────────────────┐  │
│  │     BRUNO        │    │        PuntoVentaWeb           │  │
│  │  (WinForms)      │    │        (Blazor Server)        │  │
│  │  .NET 4.8        │    │        .NET 6.0-windows       │  │
│  │                  │    │                               │  │
│  │  Desktop POS     │    │  Web-based POS modules for    │  │
│  │  application     │    │  local network deployment     │  │
│  └────────┬─────────┘    └──────────┬────────────────────┘  │
│           │                        │                        │
│           │         ┌──────────────┴──────────────┐         │
│           └────────►│       TicketJaegersoft       │        │
│                     │       (Printing Lib)         │        │
│                     └──────────────┬──────────────┘         │
│                                    │                        │
│                     ┌──────────────┴──────────────┐         │
│                     │     AgenteBasculaTorrey      │        │
│                     │       (Scale Agent)          │        │
│                     └──────────────┬──────────────┘         │
│                                    │                        │
│   ┌────────────────────────────────┴────────────────────┐   │
│   │                     Databases                       │   │
│   │  ┌──────────────────┐         ┌──────────────────┐  │   │
│   │  │  Access 2007     │         │   SQL Server     │  │   │
│   │  │  Jaeger.accdb    │         │   PuntoDeVenta   │  │   │
│   │  │  (Primary)       │         │  (New modules)   │  │   │
│   │  └──────────────────┘         └──────────────────┘  │   │
│   └───────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## Module Dependencies

- **PuntoVentaWeb** references both `BRUNO/Jaegersoft.csproj` and `TicketJaegersoft.csproj`
- **BRUNO** references `TicketJaegersoft.csproj`
- **AgenteBasculaTorrey** is standalone (independent console app)

## Data Persistence Strategy

The system uses **dual-database architecture**:

1. **Microsoft Access 2007** (`Jaeger.accdb`) - Primary/legacy database
   - Stores: Inventory, Customers, Sales, Credits, Apartados, Purchases, Configuration
   - Location: `C:\Jaeger Soft\Jaeger.accdb`
   - Password protected: `75941232`

2. **SQL Server** (`PuntoDeVenta` on `.\SQLEXPRESS`) - New modules
   - Stores: Cotizaciones (Quotes), DetalleCotizacion
   - Used by web-based features (quotation module)

## Key Connection Details

### Access (OLE DB Provider)
```
Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Jaeger Soft\Jaeger.accdb; Jet OLEDB:Database Password=75941232
```

### Access (ODBC Driver)
```
Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=C:\Jaeger Soft\Jaeger.accdb;Pwd=75941232
```

### SQL Server
```
Data Source=.\SQLEXPRESS;Initial Catalog=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;
```

## Web Application Architecture

The PuntoVentaWeb Blazor Server application:
- Listens on ports **5000 (HTTP)** and **5001 (HTTPS)** across all network interfaces
- Uses a self-signed certificate `certificado_pv.pfx`
- Serves POS functionality via web interface for local network users
- Communicates with scale hardware via HTTP to localhost:8080 (AgenteBasculaTorrey)

## UI Conventions

- All Windows Forms inherit from `frmBase` where applicable
- Use `EstilizarDataGridView()` for dark-themed tables
- Use `EstilizarBotonPrimario`, `EstilizarBotonPeligro` for consistent buttons

## Security Considerations

- Always use parameterized queries (`SqlCommand` with parameters or `OdbcCommand` with parameters)
- Always dispose resources with `using` statements
- Database passwords are hardcoded in `Conexion.cs` (currently in `BRUNO`)
