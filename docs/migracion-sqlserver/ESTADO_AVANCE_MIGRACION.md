# CONTROL DE AVANCE: MIGRACIÓN A SQL SERVER (JAEGERSOFT POS)

> **Documento de sincronización y estado entre Agentes de IA y Desarrolladores**  
> **Última actualización:** 2026-11-04  
> **Rama de trabajo obligatoria:** `feature/migracion-sqlserver-total`  
> **Regla de oro:** No cambiar nombres de tablas, columnas ni alterar el orden ordinal de los campos. No tocar `main`.

---

## 📌 INSTRUCCIONES PARA EL AGENTE DE IA ENTRANTE

Si eres un nuevo agente tomando esta tarea:
1. **Lee primero el plan maestro completo** en [PLAN_MIGRACION_SQLSERVER.md](./PLAN_MIGRACION_SQLSERVER.md).
2. **Revisa la sección "ESTADO ACTUAL DEL PROYECTO"** más abajo para saber exactamente en qué fase nos encontramos y cuál es la siguiente tarea inmediata.
3. **Verifica en qué rama estás parado**:
   ```powershell
   git branch --show-current
   ```
   *Debe ser `feature/migracion-sqlserver-total`. Si no existe o estás en `main`, NO hagas cambios en `main`.*
4. **Al terminar tu turno de trabajo**:
   * Actualiza los checkboxes `[x]` de las tareas que completaste en este archivo.
   * Agrega una entrada en la sección **"Bitácora de Sesiones"** con la fecha y el resumen de lo realizado.

---

## 🚦 ESTADO ACTUAL DEL PROYECTO

* **Fase Activa:** `Fase 5: Pruebas Integrales y Certificación`
* **Progreso Global Estimado:** `88%`
* **Bloqueos actuales:** Ninguno. Fase 4 completa y validada contra la base real; Fase 5 en pruebas manuales.

---

## 📋 CHECKLIST DETALLADO POR FASES

### [x] Fase 0: Aislamiento en Git y Respaldos
- [x] Crear y cambiar a la rama `feature/migracion-sqlserver-total` (`git checkout -b feature/migracion-sqlserver-total`).
- [x] Hacer copia de respaldo de `C:\Jaeger Soft\Jaeger.accdb` como `Jaeger_BACKUP_PRE_SQL.accdb`.
- [x] Validar que el servicio `MSSQL$SQLEXPRESS` esté activo y la base de datos `PuntoDeVenta` exista.

---

### [x] Fase 1: Creación del Esquema en SQL Server (57 Tablas)
- [x] Ejecutar script de introspección sobre `Jaeger.accdb` para extraer el esquema completo de las 57 tablas.
- [x] Generar script SQL con los `CREATE TABLE` preservando el orden ordinal exacto (`ORDINAL_POSITION`).
- [x] Aplicar tipos de datos compatibles (`VARCHAR`, `INT`, `DECIMAL`, `BIT`, `DATETIME`, etc.).
- [x] Crear índices primarios y campos `IDENTITY(1,1)` correspondientes.
- [x] Ejecutar el DDL en la base de datos `PuntoDeVenta` de SQL Server.
- [x] Validar mediante consulta a `INFORMATION_SCHEMA.COLUMNS` que las 57 tablas coinciden con Access en nombres y orden de columnas.

---

### [x] Fase 2: Migración de Datos (ETL)
- [x] Desarrollar script/herramienta de extracción y carga de datos desde Access hacia SQL Server.
- [x] Aplicar `SET IDENTITY_INSERT [Tabla] ON / OFF` en tablas con clave autonumérica para preservar IDs existentes.
- [x] Ejecutar la carga de datos de todas las tablas con registros (`Inventario`, `Kardex`, `Ventas`, `Clientes`, `Configuracion`, etc.).
- [x] Ejecutar script de validación cruzada: comparar conteo de filas `COUNT(*)` en Access vs SQL Server para las 57 tablas (100% coincidentes).
- [x] Ejecutar `DBCC CHECKIDENT` para resembrar los valores autonuméricos de todas las tablas con IDENTITY.
- [x] Reescritura del ETL como herramienta Python repetible (`herramientas/migrar_access_a_sql.py`): modos `verify` / `dry-run` / `load`, respaldo de tablas destino, `fast_executemany`, conversión de valores heredados, verificación de conteos y de contenido, resecuenciado de IDENTITY. Guía operativa en [COMO_MIGRAR_DATOS.md](./COMO_MIGRAR_DATOS.md).
  - Probado de punta a punta contra una base clon vacía (`herramientas/clonar_esquema_prueba.sql` → `PuntoDeVenta_ETL_Test`): 57/57 tablas, 453 filas, `--deep-check` sin diferencias, `IDENT_CURRENT` = `MAX(Id)`.

---

### [x] Fase 3: Migración de PuntoVentaWeb (.NET 8 Blazor)
- [x] Migrar `PuntoVentaWeb/Services/VentaService.cs` (eliminar `OdbcConnection`, implementar `SqlConnection`).
- [x] Migrar `PuntoVentaWeb/Services/ClienteService.cs` (eliminar `OdbcConnection`, implementar `SqlConnection`).
- [x] Migrar `PuntoVentaWeb/Services/CotizacionService.cs` (eliminar remanentes de ODBC/OleDb).
- [x] Migrar `PuntoVentaWeb/Services/ProduccionService.cs` (eliminar `OdbcConnection` y `Conexion.CadCon`).
- [x] Migrar `PuntoVentaWeb/Services/ImpresionPosService.cs` (eliminar `OdbcConnection`).
- [x] Reemplazar sintaxis Access (`NOW()` → `GETDATE()`, `True` → `1`, parámetros `?` → nombrados).
- [x] Compilar y probar `PuntoVentaWeb` en el navegador; verificar guardado de venta y consulta de catálogo (0 errores de compilación).

---

### [x] Fase 4: Migración del Core Desktop BRUNO (.NET Framework 4.8)
- [x] Actualizar [Conexion.cs](file:///c:/Users/Brandon/Documents/Jaegersoft/ntm/Leo/BRUNO-PERLA-RED-clean/BRUNO/Conexion.cs) para cargar configuración desde `CadSQL`.
- [x] Confirmar que los lectores Excel en `frmCapturaInicial.cs` y `frmFisico.cs` permanezcan con OleDb intactos.
- [x] Migrar clases de datos: cambiar `OleDbConnection`, `OleDbCommand`, `OleDbDataAdapter`, `OleDbDataReader`, `OleDbTransaction` por `Sql...` (232 archivos; ya no queda ningún `OleDb` fuera de los lectores de Excel).
- [x] Convertir los parámetros posicionales (`?`) a nombrados: **120 parámetros en 8 archivos** (`frmConsigna.cs` 29, `frmConsignaEntregar.cs` 24, `frmEditarProducto.cs` 21, `frmConsignaDevolver.cs` 18, `frmConsignaCobrar.cs` 11, `frmVentaDetallada.cs` 10, `frmVentasCredito.cs` 10 —incluye un `Add("?", SqlDbType.DateTime)`—, `frmAgregarInventario.cs` 2). Se reescribieron las consultas con nombres semánticos en lugar de crear un helper.
- [x] Reemplazar llamadas a `NOW()` por `GETDATE()` (0 pendientes).
- [x] Reemplazar literales booleanos (`True/False`, `Yes/No`) por `1/0` en cadenas SQL (0 pendientes).
- [x] Reemplazar delimitadores de fecha `#fecha#` por comillas simples con formato ISO: **26 consultas en 15 archivos**. Se normalizó además el formato a `yyyy-MM-dd` (el `M/D/YYYY` de Access es ambiguo según el idioma del login). Herramienta: `docs/migracion-sqlserver/herramientas/convertir_fechas_access.py`.
- [x] Reparar la corrupción de encoding (`U+FFFD`) que la migración masiva introdujo en 46 archivos (286 caracteres acentuados). Herramienta: `docs/migracion-sqlserver/herramientas/reparar_encoding.py` (alinea contra los blobs de `git HEAD`).
- [x] Reparar el mojibake (doble codificación UTF-8/Windows-1252) en 4 archivos y 62 líneas (`frmVentasCredito.cs`, `frmVentaDetallada.cs`, `frmEditarProducto.cs`, `frmPrincipal.cs`). Herramienta: `docs/migracion-sqlserver/herramientas/reparar_mojibake.py`.
- [x] Corregir comodines de Access `LIKE '*texto*'` → `LIKE '%texto%'` en `frmReporteVentas.cs`.
- [x] Verificar que no queda ningún `?` posicional, ninguna fecha `#...#` ni parámetro SQL sin declarar (auditoría sobre todo `BRUNO`).
- [x] Compilar la solución completa con **0 errores** (4 proyectos: `Jaegersoft`, `PuntoVentaWeb`, `TicketJaegersoft` y `AgenteBasculaTorrey`).
- [x] Commit del avance de la Fase 4 en `feature/migracion-sqlserver-total`.

> **Cómo compilar (importante):** usar el MSBuild de Visual Studio 2022, no el de .NET Framework 4.0 ni `dotnet build`:
> ```powershell
> & "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "Jaegersoft Breackpoint.sln" /t:Rebuild /p:Configuration=Debug /m
> ```
> * El MSBuild 4.0 (`C:\Windows\Microsoft.NET\Framework64\v4.0.30319`) rechaza los proyectos SDK-style con `MSB4041`.
> * `dotnet build` compila los 4 proyectos, pero si le toca regenerar los `.resx` de WinForms falla con `MSB3822`/`MSB3823` (recursos no-string). El MSBuild de VS 2022 sí los procesa.
> * Se usa `dotnet build` solo para chequeos rápidos de `PuntoVentaWeb`.
>
> **Aclaración (revisado):** el mapeo de `frmEditarProducto.cs` que parecía cruzado **es correcto**: los nombres de los controles están mal puestos pero las etiquetas confirman la asignación. `cmbCategoria` (ítems "NO INCLUYE IVA" / "IVA(16)") es el combo rotulado **"Impuesto:"** → columna `IVA`; `comboBox1` (Ferretería/Materiales y luego las categorías de la BD) es el rotulado **"Categoria:"** → columna `Categoria`; `txtLimite` es el precio de compra → columna `Especial`. No requiere cambios.

---

### [ ] Fase 5: Pruebas Integrales y Certificación
- [x] Validación estática contra la base real: las consultas migradas compilan en SQL Server (`docs/migracion-sqlserver/validacion_consultas_fase4.sql` con `SET NOEXEC ON`, 0 errores).
- [x] Verificado el estado de la base: servicio `MSSQL$SQLEXPRESS` activo, 65 tablas, datos presentes (`Inventario` 83, `Ventas` 34, `Clientes` 1, `Kardex` 79).
- [x] Corregidos 3 `?` residuales en `frmPrincipal.cs` (recálculo de adeudos de clientes: `SELECT Sum(Saldo)… WHERE IdCliente = ?` y `UPDATE Clientes SET Adeudo = ? WHERE Id = ?`).
- [x] Corregidas funciones de Access que SQL Server no tiene en `frmInventario.cs` (rotación de ventas): `Date()` → `GETDATE()`, `CInt()`/`Val()` → `CAST`/`TRY_CAST`, y conversión de `VentasApartados.Cantidad` (nvarchar) a número.
- [x] Corregida aritmética sobre columnas de dinero que en Access eran texto (nvarchar): `SUM(Monto)` en `frmReporteVentas.cs` y la división de IVA en `frmCorte.cs` (además `16 / 100` daba 0 por división entera → `16.0 / 100`).
- [ ] Probar inicio de sesión y validación de usuarios/permisos (`frmLogin.cs`).
- [ ] Probar apertura de caja (`frmAbrirCaja.cs`).
- [ ] Probar venta de contado, cálculo de cambio e impresión de ticket.
- [ ] Probar venta a crédito y verificación de saldo del cliente.
- [ ] Probar venta en apartado y cobro de abonos.
- [ ] Probar generación de corte de caja X y Z (`frmCorte.cs`).
- [ ] Probar altas y modificaciones en catálogo de inventario (`frmInventario.cs`).
- [ ] Probar consultas de kardex y movimientos de almacén.
- [ ] Revisar el resto de reportes con `SUM`/aritmética sobre columnas nvarchar (ver nota de tipos, abajo).

> **Nota de tipos (heredado de Access, no es un defecto del ETL):** columnas como `Ventas.Monto`, `VentasPagos.Monto`, `histocortes.Monto/Utilidad`, `Cortes.Monto`, `Kardex.Precio`, `Abonos.Abono`, `VentasCajero.Total`, `Proveedores.Adeudo`, `Servicios.Abono`, `Apartados.MontoTotal` y `VentasApartados.MontoTotal` son `nvarchar` porque en Access eran texto (incluso con valores como `"$917.00"`). Jet convertía solo; SQL Server **no**. Regla para lo que resta de la Fase 5: toda aritmética/`SUM` sobre esas columnas necesita `CAST(REPLACE(col,'$','') AS DECIMAL(18,2))` o `TRY_CAST`, o bien normalizar los tipos (decisión pendiente con el usuario).

---

### [ ] Fase 6: Retiro de Access y Entrega
- [ ] Realizar una sincronización final de registros transaccionales desde Access a SQL Server.
- [ ] Renombrar `C:\Jaeger Soft\Jaeger.accdb` temporalmente para certificar que el sistema no lo requiere.
- [ ] Compilar ejecutables en configuración `Release`.
- [ ] Esperar aprobación explícita del usuario para fusionar la rama a `main`.

---

## 📝 BITÁCORA DE SESIONES

| Fecha | Agente / Autor | Fase Abordada | Resumen de Cambios Realizados |
| :--- | :--- | :--- | :--- |
| **2026-11-04** | Antigravity AI | Fases 0, 1 y 2 | Creación de rama Git `feature/migracion-sqlserver-total`. Respaldos de Access y SQL Server. Creación de las 57 tablas en SQL Server con nombres y columnas 100% idénticos en orden. Ejecución de ETL con SqlBulkCopy + IDENTITY_INSERT. Verificación de conteo (todas las tablas 100% coincidentes). Resiembra de IDENTITY con DBCC CHECKIDENT. |
| **2026-11-04** | Antigravity AI | Fase 3 | Migración 100% de PuntoVentaWeb a SQL Server: eliminados todos los usos de ODBC y OleDb en `VentaService`, `ClienteService`, `CotizacionService`, `ProduccionService` e `ImpresionPosService`. Parámetros nombrados, `GETDATE()`, `BIT=1`. Compilación exitosa con 0 errores. |
| **2026-11-04** | Agente activo | Fase 4 (parcial) | Verificado el estado real: la migración masiva OleDb→SqlClient ya está aplicada en el árbol de trabajo (232 archivos sin commitear) y `Conexion.cs` ya apunta a `CadSQL`. Detectado y reparado un daño de encoding: la migración convirtió 286 caracteres acentuados en `U+FFFD` en 46 archivos, lo que rompía la compilación (20 errores en `frmLogin.cs`). Se repararon todos con `herramientas/reparar_encoding.py` (referencia: `git HEAD`) + 7 correcciones manuales. `BRUNO` y `PuntoVentaWeb` compilan ahora con 0 errores. Pendientes de Fase 4: 120 parámetros `?` en 8 archivos y 26 consultas con fechas `#fecha#` en 15 archivos. |
| **2026-11-04** | Agente activo | Fase 4 (cierre) | Conversión completa de parámetros posicionales `?` a nombrados: 120 en 8 archivos (`frmConsigna`, `frmConsignaEntregar`, `frmEditarProducto`, `frmConsignaDevolver`, `frmConsignaCobrar`, `frmVentaDetallada`, `frmVentasCredito`, `frmAgregarInventario`), respetando el orden original de cada `AddWithValue`. Conversión de 26 consultas con fechas `#...#` a comillas simples con formato ISO en 15 archivos (incluye `frmHistorialCortes` y `frmReporteVentas`, que usaban interpolación). Extras: reparado el mojibake de 4 archivos (62 líneas) y el comodín `LIKE '*TARJETA*'`. Auditoría final: 0 `?` posicionales, 0 fechas `#...#`, 0 parámetros SQL sin declarar. `dotnet build "Jaegersoft Breackpoint.sln"` → 4 proyectos, **0 errores**. Commit del avance en la rama. |
| **2026-11-04** | Agente activo | Fase 5 (pre-flight) | Validación estática contra la base real con `validacion_consultas_fase4.sql` (`SET NOEXEC ON`): 0 errores de compilación en las consultas migradas. La validación destapó 4 defectos que el build no ve: (1) 3 `?` residuales en `frmPrincipal.cs` (recálculo de adeudos), (2) `Date()`/`CInt()`/`Val()` de Access en la rotación de ventas de `frmInventario.cs`, (3) `SUM(Monto)` sobre `VentasPagos.Monto` que es nvarchar (`frmReporteVentas.cs`), (4) división de IVA sobre `Ventas.Monto` nvarchar y `16/100` con división entera en `frmCorte.cs`. Los 4 corregidos y recompilados (0 errores). Se aclaró que el mapeo de `frmEditarProducto` era correcto (los nombres de los controles engañan, las etiquetas confirman). Nota para el resto de la Fase 5: las columnas de dinero son nvarchar (heredadas de Access como texto) y requieren CAST/TRY_CAST en cualquier aritmética SQL. |
| **2026-11-04** | Agente activo | Fase 2 (herramienta) | ETL reescrito en Python (`herramientas/migrar_access_a_sql.py`) para poder repetirlo en producción y en la Fase 6. Modos `verify` / `dry-run` / `load`, `--backup`, `--deep-check`, filtros `--tables`/`--skip`, respaldo previo de cada tabla y resecuenciado de IDENTITY. Validado de punta a punta sobre una base clon vacía: 57/57 tablas, 453 filas, sin diferencias de contenido e `IDENT_CURRENT` = `MAX(Id)`. La base de prueba se eliminó al terminar; `PuntoDeVenta` no se tocó (conteos idénticos antes y después). Guía: [COMO_MIGRAR_DATOS.md](./COMO_MIGRAR_DATOS.md). |
| *Siguiente* | *Agente Activo* | Fase 5 | Pruebas manuales con el POS abierto contra `.\SQLEXPRESS`: login, apertura de caja, venta de contado/crédito/apartado, corte X/Z, inventario y kardex. |
