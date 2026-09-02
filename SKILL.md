---
name: jaegersoft-pos-dev
description: Instrucciones y contexto para desarrollar en el Punto de Venta (POS) Jaegersoft (Proyecto Leo).
---

# Jaegersoft POS (Proyecto Leo)

Esta "skill" contiene el contexto principal para trabajar en este repositorio. El proyecto es un sistema de Punto de Venta (POS) desarrollado en C# con Windows Forms.

## Conocimiento del Repositorio

**IMPORTANTE**: Antes de hacer cambios, consulta la base de conocimiento en [`.context/README.md`](.context/README.md) para obtener información completa y actualizada de la arquitectura del proyecto, módulos, y convenciones.

## Módulos del Proyecto

1. **BRUNO** (`JaegerSoft BreackPoint`): Aplicación de escritorio principal (Windows Forms, .NET Framework 4.8).
2. **PuntoVentaWeb** (Blazor Server, .NET 6.0-windows): Módulos web para red local. Referencia a BRUNO y TicketJaegersoft.
3. **TicketJaegersoft**: Librería de impresión de tickets (común a los proyectos).
4. **AgenteBasculaTorrey**: Agente de consola para comunicación con báscula (puerto serial, HTTP en localhost:8080).

## Reglas y Convenciones

1. **Estilos de UI**:
   - Usar la clase `frmBase` como plantilla para formularios nuevos cuando aplique.
   - Usar el método `EstilizarDataGridView()` en `frmBase` para el diseño oscuro de las tablas.
   - Usar `EstilizarBotonPrimario`, `EstilizarBotonPeligro`, etc., para mantener coherencia en los botones.

2. **Base de Datos**:
   - **Base principal**: Access 2007 (`C:\Jaeger Soft\Jaeger.accdb`, contraseña `75941232`).
     - OLEDB: `Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Jaeger Soft\Jaeger.accdb; Jet OLEDB:Database Password=75941232`
     - ODBC: `Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=C:\Jaeger Soft\Jaeger.accdb;Pwd=75941232`
   - **Base nueva**: SQL Server (`Data Source=.\SQLEXPRESS;Initial Catalog=PuntoDeVenta;Integrated Security=True`)
   - Siempre usar consultas parametrizadas u objetos de comando para evitar inyecciones.
   - Asegurarse de cerrar y liberar (`using`) las conexiones y recursos de base de datos.
   - Ambos motores se usan según el módulo: Access para los datos históricos/operativos, SQL Server para los nuevos módulos web (ej. Cotizaciones).

3. **Arquitectura**:
   - `BRUNO` contiene la mayoría de los formularios principales del sistema (Ventas, Inventario, Consignas, etc.).
   - `PuntoVentaWeb` escucha en puertos **5000 (HTTP)** y **5001 (HTTPS)** en toda la red local.
   - `AgenteBasculaTorrey` expone endpoint HTTP en `http://localhost:8080/peso/`.

## Instrucciones para el Agente
Eres un programador experto en C# donde trabajamos un programa que hemos usado por mucho tiempo para uso de ventas, inventarios, cortes de caja, etc.
- Implementando módulos nuevos creándolos en la web para uso local (red local, fungiendo como servidor).
- La base de datos principal es en Access 2007. Pensamos hacer los módulos nuevos ya en SQL Server para que sirvan en la web sin problema.

## Proyecto Activo: Cloudflare Tunnel Multi-Instalación

El plan de despliegue multi-instalación con Cloudflare Tunnel está detallado en [`.context/CLOUDFLARE_TUNNEL_PLAN.md`](.context/CLOUDFLARE_TUNNEL_PLAN.md). Consulta siempre este documento antes de trabajar en este tema.

### Decisiones tomadas (IMPORTANTE - estado actual)

- **Puerto**: La app ya escucha en **`http://localhost:5000`** (HTTP en toda la red vía `ListenAnyIP(5000)`). NO usar 7075; el plan original se ajustó a 5000.
- **Dominio real**: `jaeger-soft.com`. Cada negocio usa un subdominio único (ej. `app1.jaeger-soft.com/ventas`, `app2.jaeger-soft.com/ventas`).
- **Instalador de cloudflare**: **NO se desarrolla**. La creación del túnel y la instalación de `cloudflared` se hacen **manualmente** (`cloudflared tunnel create`, `service install`).
- **Cloudflare API token y `cloudflared.exe`**: Ya configurados manualmente.
- **Modelo**: El túnel `cloudflared` y la app Blazor corren como servicios de Windows que inician en boot, pero su **creación/instalación inicial es manual**.

### Resumen ejecutivo del plan

- **Propósito**: Desplegar Blazor en cada negocio y dar acceso remoto vía URL única (`https://appN.jaeger-soft.com`) sin abrir puertos en el router.
- **Arquitectura**: `Internet -> Cloudflare -> Tunnel (por negocio, manual) -> localhost:5000 -> Blazor`.
- **Identificación**: Cada instalación tiene un `InstallationId` único almacenado en `C:\ProgramData\MiSistema\installation.json` (independiente de IP).
- **Tunnels**: Uno por negocio (manual), nunca compartidos.
- **Servicios Windows**: `MiSistemaBlazor` y `cloudflared` corren como servicios que inician en boot.
- **Seguridad**: El `Tunnel Token` es sensible — nunca en logs, pantalla, Git ni código. Cada negocio tiene credenciales independientes.
- **Health check**: Endpoint `/health` devuelve `{status, installationId, businessName, hostname}`.

### Reglas de implementación clave

- La app escucha en `http://0.0.0.0:5000` (ya configurado con `ListenAnyIP(5000)`).
- Cloudflare entrega el HTTPS público; el origen interno es HTTP hacia `localhost:5000`.
- NO usar Quick Tunnel para producción (solo túneles con nombre fijo).
- El alcance inicial es SOLO publicar Blazor (no acceso directo a toda la red LAN/local).
- Identificación interna de instalación vía `InstalacionService` (el `InstallationId` es configuración interna, no autorización basada solo en hostname).
- Actualizaciones independientes de Blazor, cloudflared y configuración (una actualización no destruye el Tunnel ni el InstallationId).

## Infraestructura Blazor ya implementada (Fase 1 - Opción A)

Estos archivos fueron creados para preparar la app para el túnel:

| Archivo | Descripción |
|---------|-------------|
| `PuntoVentaWeb/Models/Instalacion.cs` | Modelo que representa la identidad de la instalación (InstallationId, BusinessName, CloudflareHostname). |
| `PuntoVentaWeb/Services/InstalacionService.cs` | Lee el archivo `C:\ProgramData\MiSistema\installation.json` (JSON) y expone `Actual` con la identidad. Devuelve valores por defecto ("SIN_CONFIGURAR") si falta el archivo. |
| `PuntoVentaWeb/Program.cs` | Registra `InstalacionService` en DI y agrega el endpoint `GET /health`. |

### Cómo funciona la identidad de instalación

1. En cada PC se crea manualmente el archivo `C:\ProgramData\MiSistema\installation.json` (NO se crea solo):
   ```json
   {
     "installationId": "A7F3D9C2",
     "businessName": "Restaurante ABC",
     "cloudflareHostname": "app1.jaeger-soft.com"
   }
   ```
2. `InstalacionService` (registrado como Scoped) lo lee al primer acceso y lo cachea (`Lazy<Instalacion>`).
3. El endpoint `GET /health` responde:
   ```json
   {
     "status": "ok",
     "installationId": "A7F3D9C2",
     "businessName": "Restaurante ABC",
     "hostname": "app1.jaeger-soft.com"
   }
   ```
4. Si el archivo no existe o falla la lectura, se devuelve `installationId = "SIN_CONFIGURAR"`.

### Verificación (local antes de publicar túnel)

```bash
curl http://localhost:5000/health
```
Debe responder un JSON con `status: "ok"`.

### Pasos manuales pendientes (del lado de Cloudflare/PC)

1. Crear túnel: `cloudflared tunnel create <nombre-unico>`
2. Configurar `config.yml` apuntando a `http://localhost:5000` y al subdominio `appN.jaeger-soft.com`.
3. Registrar CNAME en Cloudflare.
4. Instalar servicio: `cloudflared service install` + `net start "Cloudflare Tunnel"`.
5. Instalar Blazor como servicio (`sc create` o NSSM).
6. Verificar: `https://appN.jaeger-soft.com/health`.

## Proceso Recomendado

1. **Antes de codificar**: Consulta `.context/` para conocer la arquitectura y convenciones.
2. **Durante el desarrollo**: Respeta los patrones existentes (conexiones `using`, consultas parametrizadas, estilos de UI).
3. **Al terminar**: Actualiza la documentación en `.context/` si agregaste módulos, páginas o servicios nuevos.
