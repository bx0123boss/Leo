# PLAN MAESTRO: MIGRACIÓN TOTAL DE ACCESS A SQL SERVER (PROYECTO LEO / JAEGERSOFT POS)

> **Documento Base para Agentes de IA y Desarrolladores**  
> **Fecha de creación:** Noviembre 2026  
> **Estado:** Fase 0 (Preparación)

---

## 1. REGLAS INNEGOCIABLES DEL PROYECTO

1. **NO CAMBIAR NOMBRES DE TABLAS:** Deben coincidir exactamente carácter por carácter con la base de datos de Access actual.
2. **NO CAMBIAR NOMBRES DE CAMPOS / COLUMNAS:** Deben conservar exactamente el mismo nombre (ej. `Id`, `FolioVenta`, `Fecha`, etc.).
3. **NO ALTERAR EL ORDEN DE LAS COLUMNAS:** El orden ordinal (`ORDINAL_POSITION` de 1 a N) de cada columna en SQL Server debe ser idéntico al de Access. El código heredado en WinForms hace lecturas directas por índice (`reader[0]`, `reader[1]`, `SELECT *`) y un cambio de orden romperá la asignación de datos.
4. **AISLAMIENTO TOTAL EN GIT:** Todo el desarrollo debe hacerse en la rama `feature/migracion-sqlserver-total`. No se hará merge ni commit directo a la rama `main` hasta que el sistema completo esté compilando, probado y validado al 100%.
5. **EXCEPCIÓN DE IMPORTACIÓN DE EXCEL:** Los formularios `frmCapturaInicial.cs` y `frmFisico.cs` utilizan `OleDbConnection` con el proveedor ACE OLEDB para leer hojas de cálculo de **Excel** (`Extended Properties='Excel 12.0 Xml'`). **ESTOS LECTORES DE EXCEL NO DEBEN CAMBIARSE A SQLCLIENT**, solo se modifican las conexiones hacia la base de datos del sistema.

---

## 2. INFORMACIÓN DE CONEXIÓN Y ENTORNO

### Origen (Access - A eliminar)
* **Ubicación:** `C:\Jaeger Soft\Jaeger.accdb`
* **Contraseña Jet OLEDB:** `75941232`
* **Cadena de conexión histórica (`Conexion.CadCon`):**
  ```text
  Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Jaeger Soft\Jaeger.accdb; Jet OLEDB:Database Password=75941232
  ```

### Destino (SQL Server - 100% Gestor Único)
* **Instancia:** `.\SQLEXPRESS` (Instalado y verificado localmente)
* **Base de Datos:** `PuntoDeVenta`
* **Autenticación:** Windows Authentication (Seguridad integrada)
* **Cadena de conexión estándar (`Conexion.CadSQL`):**
  ```csharp
  public static string CadSQL = @"Data Source=.\SQLEXPRESS;Initial Catalog=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;";
  ```
* **Cadena para appsettings.json en PuntoVentaWeb:**
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=PuntoDeVenta;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
  ```

---

## 3. INVENTARIO DE TABLAS (57 TABLAS EN TOTAL)

A continuación se listan las 57 tablas existentes en `Jaeger.accdb` que deben ser creadas y replicadas en SQL Server:

1. `Abonos`
2. `AbonosApartado`
3. `AbonosProveedores`
4. `Almacenes`
5. `Apartados`
6. `Cambio`
7. `Categorias`
8. `Clientes`
9. `Compras`
10. `Compras2`
11. `ComprasC`
12. `ComprasMovimientos`
13. `Configuracion`
14. `ConsignaCliente`
15. `ConsignaMovimientos`
16. `Corte`
17. `Cortes`
18. `CortesB`
19. `Credito`
20. `Errores de pegado`
21. `Fech`
22. `Fisico`
23. `FisicoDetallado`
24. `Folios`
25. `GastosDetallados`
26. `GastosGeneral`
27. `histocortes`
28. `histocortesB`
29. `Inventario`
30. `InventarioAlmacenes`
31. `InventarioSusp`
32. `Kardex`
33. `Linea`
34. `MovimientosAlmacenes`
35. `PermisosUsuario`
36. `Poliza`
37. `Poliza2`
38. `Porcentaje`
39. `ProduccionLotes`
40. `ProductoReceta`
41. `ProductosPoliza`
42. `ProductosPoliza2`
43. `Proveedores`
44. `RespaldoInventario`
45. `Servicios`
46. `Tarjeta`
47. `TmpVentasPeriodo`
48. `Unidades`
49. `Usuarios`
50. `Ventas`
51. `Ventas2`
52. `VentasApartados`
53. `VentasB`
54. `VentasCajero`
55. `VentasContado`
56. `VentasCredito`
57. `VentasPagos`

> **Nota:** La base `PuntoDeVenta` en SQL Server ya posee algunas tablas nuevas de la versión Web (`Consignaciones`, `Cotizaciones`, etc.). Para tablas compartidas (como `VentasPagos` o `ConsignaCliente`), se debe respetar estrictamente la compatibilidad con el sistema de escritorio.

---

## 4. MAPEO DE TIPOS DE DATOS (ACCESS OLEDB → SQL SERVER)

Para generar el DDL automáticamente sin alterar tipos:

| Tipo en Access (OLEDB) | Código DBTYPE | Tipo destino en SQL Server | Consideraciones |
| :--- | :--- | :--- | :--- |
| **Texto Corto** | 130 (WChar) | `VARCHAR(255)` / `NVARCHAR(255)` | Conservar longitud especificada |
| **Texto Largo / Memo** | 130 (WChar len 0) | `NVARCHAR(MAX)` | Para descripciones largas, logs, xml |
| **Autonumérico / Entero largo** | 3 (I4) | `INT IDENTITY(1,1)` | Si es clave primaria autonumérica |
| **Entero corto** | 2 (I2) | `SMALLINT` o `INT` | |
| **Moneda** | 6 (CY) | `DECIMAL(18,2)` o `MONEY` | Se recomienda `DECIMAL(18,4)` |
| **Doble / Simple** | 5 (R8) / 4 (R4) | `FLOAT` / `DECIMAL(18,4)` | Stock, kilogramos, porcentajes |
| **Sí / No (Booleano)** | 11 (BOOL) | `BIT` | En SQL Server usa 1 y 0 |
| **Fecha / Hora** | 7 (DATE) | `DATETIME` | Formato 'YYYY-MM-DD HH:mm:ss' |
| **Datos adjuntos / Ole** | 128 (BYTES) | `VARBINARY(MAX)` | Imágenes o binarios |

---

## 5. GUÍA DE REFACTORIZACIÓN DE CÓDIGO (C# Y SQL)

### A. Reemplazo de Namespaces y Clases ADO.NET
En el proyecto de escritorio `BRUNO` (.NET Framework 4.8):
* Quitar: `using System.Data.OleDb;`
* Agregar: `using System.Data.SqlClient;`
* Reemplazos directos:
  * `OleDbConnection` → `SqlConnection`
  * `OleDbCommand` → `SqlCommand`
  * `OleDbDataAdapter` → `SqlDataAdapter`
  * `OleDbDataReader` → `SqlDataReader`
  * `OleDbTransaction` → `SqlTransaction`
  * `OleDbParameter` → `SqlParameter`
  * `Conexion.CadCon` → `Conexion.CadSQL`

### B. Diccionario de Sintaxis SQL (Access Jet → SQL Server T-SQL)

1. **Función de Fecha/Hora Actual:**
   * Access: `NOW()`
   * SQL Server: `GETDATE()`
   * *Acción:* Reemplazar en todas las consultas de inserción/actualización de inventario, ventas, kárdex.

2. **Valores Booleanos en Consultas:**
   * Access: `WHERE ManejaInventario = True`, `WHERE EsReceta = Yes`
   * SQL Server: `WHERE ManejaInventario = 1`, `WHERE EsReceta = 1` (y `0` para False/No).
   * *Motivo:* T-SQL marca error `Invalid column name 'True'` si no se usan enteros `1` o `0`.

3. **Delimitadores de Fecha:**
   * Access: `WHERE Fecha >= #2026/01/01#`
   * SQL Server: `WHERE Fecha >= '2026-01-01'` (cadenas con comillas simples).

4. **Identificador Autoincremental Recién Insertado:**
   * En ambos gestores funciona `SELECT @@IDENTITY;`. No requiere cambios.

5. **Parámetros Posicionales (`?`) a Nombrados (`@p0, @p1...`):**
   * En Access OLEDB/ODBC, los parámetros se agregan con `?` y se evalúan por orden de agregado:
     ```csharp
     cmd.CommandText = "UPDATE Inventario SET Existencia = ? WHERE Id = ?";
     cmd.Parameters.AddWithValue("?", 10);
     cmd.Parameters.AddWithValue("?", "PROD1");
     ```
   * En `SqlCommand` de SQL Server, los `?` no son válidos directamente. Se debe utilizar parámetros con nombre:
     ```csharp
     cmd.CommandText = "UPDATE Inventario SET Existencia = @Existencia WHERE Id = @Id";
     cmd.Parameters.AddWithValue("@Existencia", 10);
     cmd.Parameters.AddWithValue("@Id", "PROD1");
     ```
   * *Solución recomendada para acelerar la migración:* Se creará un método de extensión helper `cmd.ConvertirParametrosPosicionales()` o se reescribirán las consultas puntuales para que reemplacen los `?` por `@p0, @p1...` automáticamente.

---

## 6. FASES DETALLADAS DE EJECUCIÓN

### Fase 0: Aislamiento en Git y Respaldos
1. Crear y cambiarse a la rama `feature/migracion-sqlserver-total`:
   ```powershell
   git checkout -b feature/migracion-sqlserver-total
   ```
2. Realizar copia de seguridad del archivo `C:\Jaeger Soft\Jaeger.accdb` y del estado actual de `PuntoDeVenta` en SQL Server.

### Fase 1: Creación del Esquema en SQL Server
1. Ejecutar el script extractor de esquema en C# o PowerShell que conecta a `Jaeger.accdb` mediante OLEDB.
2. Generar el script DDL `.sql` respetando:
   * Nombre de tabla idéntico.
   * Lista de columnas y tipos en el orden ordinal exacto.
   * Definición de claves primarias e `IDENTITY(1,1)` cuando corresponda.
3. Ejecutar el script contra `SQLEXPRESS` en la base de datos `PuntoDeVenta`.
4. Verificar mediante `INFORMATION_SCHEMA.COLUMNS` que todas las 57 tablas existen y coinciden columna por columna.

### Fase 2: Migración de Datos (ETL)
1. Script de migración tabla por tabla desde Access hacia SQL Server.
2. Para cada tabla con campos autonuméricos (`Id` autonumérico):
   ```sql
   SET IDENTITY_INSERT [Tabla] ON;
   -- INSERT de datos respetando los IDs originales
   SET IDENTITY_INSERT [Tabla] OFF;
   ```
3. Comprobación cruzada de número de filas entre Access y SQL Server.

### Fase 3: Migración de `PuntoVentaWeb`
1. Reemplazar `OdbcConnection` y `System.Data.Odbc` por `Microsoft.Data.SqlClient` en:
   * `PuntoVentaWeb/Services/VentaService.cs`
   * `PuntoVentaWeb/Services/ClienteService.cs`
   * `PuntoVentaWeb/Services/CotizacionService.cs`
   * `PuntoVentaWeb/Services/ProduccionService.cs`
   * `PuntoVentaWeb/Services/ImpresionPosService.cs`
2. Ajustar sintaxis en consultas (`NOW()` → `GETDATE()`, `True` → `1`, `?` → parámetros nombrados).
3. Compilar `PuntoVentaWeb` y verificar ejecución en el navegador.

### Fase 4: Migración del Proyecto Desktop `BRUNO`
1. Actualizar `Conexion.cs` para inicializar y cargar configuración desde SQL Server.
2. Migración masiva de referencias ADO.NET:
   * Reemplazar `OleDbConnection`, `OleDbCommand`, `OleDbDataAdapter`, `OleDbDataReader` por sus equivalentes `Sql...`.
   * Reemplazar `Conexion.CadCon` por `Conexion.CadSQL`.
   * Mantener los lectores Excel en `frmCapturaInicial.cs` y `frmFisico.cs`.
3. Ajuste de consultas SQL:
   * Reemplazar `NOW()` por `GETDATE()`.
   * Reemplazar `True/False` por `1/0`.
   * Convertir consultas con `?` a parámetros válidos de SQL Server.
4. Compilación del proyecto `BRUNO` hasta lograr 0 errores y 0 advertencias críticas de compilación.

### Fase 5: Pruebas Integrales y Certificación
1. **Flujo de Ventas:**
   * Venta de contado (con y sin IVA).
   * Venta a crédito y asignación a cliente.
   * Venta en apartado y abonos subsecuentes.
   * Cobro mixto (Efectivo + Tarjeta + Transferencia).
2. **Caja y Cortes:**
   * Apertura de caja (`frmAbrirCaja.cs`).
   * Consulta y cálculo de corte (`frmCorte.cs`).
   * Cierre y guardado en `Cortes` y `histocortes`.
3. **Inventario y Kárdex:**
   * Alta de producto (`frmAgregarInventario.cs`).
   * Modificación de stock y registro en `Kardex`.
   * Recetas e insumos (`ProduccionService.cs` / `frmRecetas.cs`).
4. **Clientes y Proveedores:**
   * Abonos a cuenta y consulta de saldos.
5. **Configuración del Ticket:**
   * Impresión de ticket de prueba asegurando que lee la configuración de SQL Server.

### Fase 6: Retiro Definitivo de Access
1. Realizar una corrida final de sincronización de datos para mover cualquier transacción reciente generada en Access.
2. Desconectar o renombrar el archivo `C:\Jaeger Soft\Jaeger.accdb` a `Jaeger.accdb.OLD` para verificar que el sistema no dependa en lo absoluto de Access.
3. Generar la versión Release ejecutable.
4. Solo cuando el usuario dé su aprobación explícita, fusionar (`merge`) la rama `feature/migracion-sqlserver-total` a `main`.
