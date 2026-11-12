# Cómo migrar los datos de Access a SQL Server

Guía operativa para ejecutar el ETL (`herramientas/migrar_access_a_sql.py`) en producción.
El esquema de las 57 tablas **ya está creado** en `PuntoDeVenta`; este proceso solo mueve datos.

---

## 1. Requisitos (máquina donde se ejecuta)

| Componente | Detalle |
| :--- | :--- |
| Python 3.9+ | 64 bits (el driver de Access debe coincidir en arquitectura) |
| `pyodbc` | `pip install pyodbc` |
| Driver Access | `Microsoft Access Driver (*.mdb, *.accdb)` 64 bits |
| Driver SQL | `ODBC Driver 17 for SQL Server` (o 18) |
| Acceso al archivo | `C:\Jaeger Soft\Jaeger.accdb` y la contraseña Jet |

Verificar los drivers antes de empezar:

```powershell
python -c "import pyodbc; print(pyodbc.drivers())"
```

---

## 2. Ensayo obligatorio en una base de prueba

Nunca estrenes el script contra producción. Primero se clona el esquema vacío y se migra ahí:

```powershell
# Crea PuntoDeVenta_ETL_Test con las 57 tablas (sin datos)
sqlcmd -S .\SQLEXPRESS -E -i docs\migracion-sqlserver\herramientas\clonar_esquema_prueba.sql

# Migra Access -> base de prueba y compara contenido
python docs\migracion-sqlserver\herramientas\migrar_access_a_sql.py `
    --mode load --database PuntoDeVenta_ETL_Test --yes --deep-check

# Segunda pasada solo de verificación (no escribe nada)
python docs\migracion-sqlserver\herramientas\migrar_access_a_sql.py `
    --mode verify --database PuntoDeVenta_ETL_Test --deep-check
```

Si el ensayo sale `Tablas OK: 57/57`, el script está listo para producción.

---

## 3. Ejecución en producción

**Antes:**

1. Cerrar el POS en todas las terminales (si alguien escribe mientras se recarga una tabla, se pierden esos movimientos).
2. Respaldar `C:\Jaeger Soft\Jaeger.accdb` (copiar el archivo).
3. Respaldar la base: `BACKUP DATABASE PuntoDeVenta TO DISK='C:\Jaeger Soft\PuntoDeVenta_antes_de_ETL.bak'`
4. Si hay varias terminales, avisar que el sistema estará detenido unos minutos.

**Paso 1 — simular (no escribe):**

```powershell
python docs\migracion-sqlserver\herramientas\migrar_access_a_sql.py --mode dry-run
```

Muestra tabla por tabla: filas en Access, filas en SQL, columnas detectadas y avisos (columnas
faltantes, diferencias de orden ordinal, etc.).

**Paso 2 — migrar con respaldo de cada tabla:**

```powershell
python docs\migracion-sqlserver\herramientas\migrar_access_a_sql.py `
    --mode load --backup --yes --deep-check
```

* `--backup` copia cada tabla destino a `<tabla>_bkp_AAAAMMDD_HHMM` **antes** de vaciarla.
  Para volver atrás: `INSERT INTO Ventas SELECT * FROM Ventas_bkp_20261104_1731` (con `IDENTITY_INSERT`).
* `--deep-check` compara el contenido completo, no solo los conteos.
* El script se detiene con código de salida ≠ 0 si alguna tabla no cuadra, y deja el detalle en el log.

**Paso 3 — verificar:**

```powershell
python docs\migracion-sqlserver\herramientas\migrar_access_a_sql.py --mode verify --deep-check
```

Debe terminar con `Tablas OK: 57/57 ... Migracion completada sin errores`.

---

## 4. Qué hace y qué no hace

**Hace**

* Lee las 57 tablas de Access respetando **nombres y orden ordinal** de columnas.
* Preserva los IDs originales con `SET IDENTITY_INSERT` y luego re-siembra con `DBCC CHECKIDENT`.
* Convierte valores heredados: `"$1,234.00"` → `1234.00`, `"12%"` → `12`, `(50)` → `-50`,
  `Sí/No` (`-1/0`) → `bit 1/0`, cadenas vacías en fechas → `NULL`.
* Inserta por lotes (`fast_executemany`, 2000 filas) dentro de transacción por tabla.
* Nunca toca tablas que existan en SQL pero no en Access (`Consignaciones`, `Cotizaciones`, etc.).

**No hace**

* No crea ni modifica el esquema (las tablas deben existir).
* No migra índices, llaves foráneas ni permisos (el esquema destino ya los tiene).
* No resuelve los tipos heredados: hay columnas de dinero que en Access eran texto
  (`Ventas.Monto`, `VentasPagos.Monto`, `histocortes.Monto`, …) y siguen siendo `nvarchar`
  en SQL. El script las copia tal cual; el código que hace `SUM`/aritmética sobre ellas
  necesita `CAST`/`TRY_CAST` (ver la nota de tipos en `ESTADO_AVANCE_MIGRACION.md`).

---

## 5. Opciones útiles

| Opción | Para qué |
| :--- | :--- |
| `--list-tables` | Ver las tablas y cuántas filas tiene cada una |
| `--tables Ventas,Ventas2` | Migrar solo algunas (por ejemplo un reintento puntual) |
| `--skip TmpVentasPeriodo` | Excluir tablas |
| `--batch 5000` | Lotes más grandes (más rápido, más memoria) |
| `--sql-user` / `--sql-password` | Autenticación SQL en vez de Windows |
| `--log ruta.log` | Nombre del archivo de log |

---

## 6. Si algo falla

* El script sigue con la tabla siguiente y resume los errores al final; nada queda a medias
  porque cada tabla se carga dentro de su propia transacción.
* Para reintentar solo la tabla que falló:
  `--mode load --tables <Tabla> --backup --yes`
* Los logs (`migracion_AAAAMMDD_HHMM.log`) tienen el detalle por tabla.

> **Cuándo se vuelve a correr:** en la Fase 6 (retiro de Access) hay que repetir el ETL
> completo para capturar cualquier movimiento generado en Access después de esta migración.
> Mientras Access siga en uso, la base de SQL no es la fuente de la verdad.
