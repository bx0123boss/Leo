


# Cloudflare Tunnel Plan (Versión Manual + Automatización)

## 1. Objetivo

Implementar un sistema para desplegar la aplicación Blazor en múltiples negocios, donde:

1. La instalación de `cloudflared` y la creación del túnel se hacen **manualmente** (NO se desarrolla un instalador de cloudflare).
2. Una vez configurado, el túnel y la app Blazor se inician **automáticamente** con Windows.
3. Cada instalación tiene su propia URL única y su identificación.
4. La app Blazor puede identificar qué instalación la está sirviendo.

## 2. Arquitectura

Cada instalación funciona de forma independiente:

```text
Internet
   ↓
Cloudflare Network
   ↓
https://app1.jaeger-soft.com  (u otro subdominio)
   ↓
Cloudflare Tunnel (manual)
   ↓
PC del negocio
   ↓
cloudflared (servicio Windows)
   ↓
http://localhost:5000 (o el puerto que uses)
   ↓
Blazor Server (servicio Windows)
   ↓
Base de datos local
```

## 3. Identificación de la Instalación (Automático)

Cada instalación debe tener un `InstallationId` único. Este se almacena en un archivo local:

**Ruta:** `C:\ProgramData\MiSistema\installation.json`

```json
{
  "installationId": "A7F3D9C2",
  "businessName": "Restaurante ABC",
  "cloudflareHostname": "app1.jaeger-soft.com"
}
```

- Este archivo se crea **manualmente** (o con un script simple) al configurar la instalación.
- La app Blazor debe leer este archivo para saber quién es.

## 4. Estrategia con Cloudflare (Manual)

- Se compra un dominio (ej. `jaeger-soft.com`) y se configura en Cloudflare.
- Para cada negocio, se crea **un túnel manualmente**:
  ```cmd
  cloudflared tunnel create <nombre-unico>
  ```
- Se configura el archivo `config.yml` con el subdominio correspondiente.
- Se añade el registro CNAME en Cloudflare.
- **No se usa el túnel temporal** (`trycloudflare.com`), solo túneles con nombre fijo.

## 5. Configuración Automática (Servicios de Windows)

Una vez creado el túnel manualmente, se convierte en automático:

### 5.1. `cloudflared` como servicio (Automático)

```cmd
cloudflared service install
net start "Cloudflare Tunnel"
```

- Esto asegura que el túnel arranque con Windows.
- Verificación: `sc query "Cloudflare Tunnel"` debe mostrar `RUNNING`.

### 5.2. Blazor como servicio (Automático)

El Blazor también debe correr como servicio. Opciones:

**Opción A (nativa):**
```cmd
sc create "MiSistemaBlazor" binPath="C:\MiSistema\MiSistema.exe" start=auto
```

**Opción B (con NSSM):**
```cmd
nssm install MiSistemaBlazor C:\MiSistema\MiSistema.exe
nssm start MiSistemaBlazor
```

- Ambos servicios (Blazor y `cloudflared`) deben arrancar con el sistema y reiniciarse si fallan.

## 6. Endpoint de Salud (`/health`) (Automático)

La app Blazor debe tener un endpoint `/health` que devuelva:

```json
{
  "status": "ok",
  "installationId": "A7F3D9C2"
}
```

Esto permite verificar remotamente que la instalación está viva y es la correcta.

## 7. Heartbeat (Opcional, pero recomendado)

Si tienes un servidor central, la instalación puede enviar un heartbeat periódico:

```text
POST /api/installations/heartbeat
{
  "installationId": "A7F3D9C2",
  "version": "1.0.0",
  "timestamp": "2026-09-02T12:00:00Z"
}
```

## 8. Diagnóstico Manual (Script)

Crea un script `.bat` para verificar el estado:

```batch
@echo off
echo === SERVICIOS ===
sc query "Cloudflare Tunnel" | findstr RUNNING
sc query "MiSistemaBlazor" | findstr RUNNING

echo === PRUEBA LOCAL ===
curl -s -o nul -w "%%{http_code}" http://localhost:5000/health

echo === PRUEBA REMOTA ===
curl -s -o nul -w "%%{http_code}" https://app1.jaeger-soft.com/health
```

## 9. Seguridad y Buenas Prácticas

- **No guardar el token del túnel en logs ni en el código.**
- El túnel se autentica con el archivo de credenciales (`.json`) que genera Cloudflare.
- La app Blazor debe tener su propio sistema de autenticación (login).
- No se abren puertos en el router ni en el firewall de Windows.

## 10. Actualizaciones y Mantenimiento

- Para actualizar `cloudflared`: descargar el nuevo `.exe`, detener el servicio, reemplazar y reiniciar.
- Para actualizar Blazor: detener el servicio, reemplazar los archivos y reiniciar.
- El archivo `installation.json` **no se modifica** con las actualizaciones.

## 11. Desinstalación (Manual)

Para dar de baja una instalación:

1. Detener servicios:
   ```cmd
   net stop "Cloudflare Tunnel"
   net stop "MiSistemaBlazor"
   ```
2. Eliminar los servicios:
   ```cmd
   sc delete "Cloudflare Tunnel"
   sc delete "MiSistemaBlazor"
   ```
3. Eliminar la carpeta `C:\ProgramData\MiSistema\`.
4. (Opcional) Eliminar el túnel desde el panel de Cloudflare.

## 12. Criterios de Aceptación (Actualizados)

La implementación se considera completa cuando:

- [ ] Se puede crear un túnel manualmente para un nuevo negocio.
- [ ] El túnel y Blazor arrancan automáticamente al encender la PC.
- [ ] La URL pública funciona sin abrir puertos.
- [ ] La app Blazor puede leer su `InstallationId`.
- [ ] El endpoint `/health` responde correctamente.
- [ ] La instalación puede ser monitoreada remotamente.
- [ ] Se puede actualizar la app sin perder la configuración.

---

## ✅ Resumen de lo que es Manual vs. Automático

| Tarea | Manual / Automático |
|-------|----------------------|
| Comprar dominio y configurar en Cloudflare | Manual |
| Crear el túnel (`cloudflared tunnel create`) | Manual |
| Crear el archivo `config.yml` | Manual |
| Añadir el registro CNAME en Cloudflare | Manual |
| Instalar `cloudflared` como servicio | Automático (se hace una vez) |
| Instalar Blazor como servicio | Automático (se hace una vez) |
| Arrancar servicios con Windows | Automático |
| Leer `installation.json` | Automático (desde la app) |
| Endpoint `/health` | Automático (desde la app) |
| Heartbeat (si se implementa) | Automático |
| Diagnóstico con script | Manual (o programado) |

---