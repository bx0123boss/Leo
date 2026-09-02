# Jaegersoft POS - Knowledge Base

This directory contains structured documentation about the Jaegersoft POS project. All content is written in Markdown format to facilitate easy consumption by AI agents and developers.

## Project Overview

The Jaegersoft POS is a **Point of Sale (POS) system** built primarily in **C#** with multiple components:

- **BRUNO** (`JaegerSoft BreackPoint`): Legacy Windows Forms desktop application (.NET Framework 4.8)
- **PuntoVentaWeb**: Modern Blazor Server web application (.NET 6.0-windows) for local network deployment
- **TicketJaegersoft**: Ticket printing library used across projects
- **AgenteBasculaTorrey**: Console application that communicates with scale hardware (Torrey brand) over serial port

The system uses **Microsoft Access 2007** as its primary database, with an active migration path toward **SQL Server**.

## Repository Index

### 1. Main Documentation

| File | Description |
|------|-------------|
| [ARCHITECTURE.md](./ARCHITECTURE.md) | Overall system architecture and module relationships |
| [DATABASE.md](./DATABASE.md) | Database configuration, connection strings, and data persist strategies |
| [CLOUDFLARE_TUNNEL_PLAN.md](./CLOUDFLARE_TUNNEL_PLAN.md) | Multi-installation deployment plan using Cloudflare Tunnels (Blazor remote access per business) |

### 2. BRUNO (WinForms Desktop Application)

| File | Description |
|------|-------------|
| [BRUNO/BRUNO_OVERVIEW.md](./BRUNO/BRUNO_OVERVIEW.md) | Overview of the legacy desktop POS application |
| [BRUNO/BRUNO_FORMS.md](./BRUNO/BRUNO_FORMS.md) | Complete list and categorization of all WinForms |
| [BRUNO/BRUNO_UI_CONVENTIONS.md](./BRUNO/BRUNO_UI_CONVENTIONS.md) | UI styling conventions and helper methods |
| [BRUNO/BRUNO_SERVICES.md](./BRUNO/BRUNO_SERVICES.md) | Core services, utilities, and configuration |

### 3. PuntoVentaWeb (Blazor Web App)

| File | Description |
|------|-------------|
| [PuntoVentaWeb/WEB_OVERVIEW.md](./PuntoVentaWeb/WEB_OVERVIEW.md) | Overview of the web-based POS modules |
| [PuntoVentaWeb/WEB_PAGES.md](./PuntoVentaWeb/WEB_PAGES.md) | All pages/routes in the web application |
| [PuntoVentaWeb/WEB_SERVICES.md](./PuntoVentaWeb/WEB_SERVICES.md) | Service layer details |
| [PuntoVentaWeb/WEB_MODELS.md](./PuntoVentaWeb/WEB_MODELS.md) | Data models used in the web app |
| [PuntoVentaWeb/WEB_CONFIGURATION.md](./PuntoVentaWeb/WEB_CONFIGURATION.md) | App configuration, ports, and deployment settings |

### 4. TicketJaegersoft (Printing Library)

| File | Description |
|------|-------------|
| [TicketJaegersoft/TICKET_PRINTER.md](./TicketJaegersoft/TICKET_PRINTER.md) | Ticket printing library documentation |

### 5. AgenteBasculaTorrey (Scale Agent)

| File | Description |
|------|-------------|
| [AgenteBasculaTorrey/BAScula_AGENT.md](./AgenteBasculaTorrey/BAScula_AGENT.md) | Scale hardware communication agent documentation |

### 6. Skills & Configuration

| File | Description |
|------|-------------|
| [SKILL.md](../SKILL.md) | Development skill/context file for AI agents |
| [skills-lock.json](../skills-lock.json) | Locked skills configuration |

## Quick Reference

### Connection Strings


