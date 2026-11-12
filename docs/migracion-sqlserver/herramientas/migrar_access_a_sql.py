#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
=============================================================================
 MIGRADOR DE DATOS  Access (Jaeger.accdb)  ->  SQL Server (PuntoDeVenta)
=============================================================================

Herramienta de ETL para la migracion del POS Jaegersoft (Proyecto Leo).

Lee TODAS las tablas de Access respetando nombres y orden ordinal de columnas,
y las carga en SQL Server preservando los IDs originales (IDENTITY_INSERT),
con verificacion de conteos y de contenido, respaldo opcional y resecuenciado
de los autonumericos.

CARACTERISTICAS
---------------
* No crea ni altera el esquema: asume que las 57 tablas ya existen en SQL.
* Nunca toca tablas que existan en SQL pero no en Access (Consignaciones,
  Cotizaciones, etc. de la version web).
* Modos de ejecucion: verify (solo compara), dry-run (simula), load (migra).
* Respaldo previo de cada tabla dentro de SQL (--backup) antes de vaciarla.
* Conversion robusta de valores heredados: "$1,234.00" -> 1234.00,
  Yes/No (-1/0) -> bit, cadenas vacias en fechas -> NULL, etc.
* Verificacion final: conteos + comparacion de contenido (--deep-check).
* Log a consola y a archivo.

REQUISITOS
----------
    pip install pyodbc

* Driver ODBC "Microsoft Access Driver (*.mdb, *.accdb)" (64 bits).
* Driver ODBC "ODBC Driver 17 for SQL Server" (o 18).

USO TIPICO (produccion)
-----------------------
    # 1) Cerrar el POS y respaldar el .accdb
    # 2) Simular primero:
    python migrar_access_a_sql.py --mode dry-run

    # 3) Comparar sin escribir nada (base ya migrada):
    python migrar_access_a_sql.py --mode verify --deep-check

    # 4) Migracion real, con respaldo de las tablas destino:
    python migrar_access_a_sql.py --mode load --backup --yes

    # 5) Solo algunas tablas:
    python migrar_access_a_sql.py --mode load --tables Ventas,Ventas2,Kardex --yes

SEGURIDAD
---------
El modo `load` exige `--yes` (o confirmacion interactiva) porque VACIA las
tablas destino. Detenga el POS antes de ejecutarlo: si la aplicacion esta
escribiendo mientras se recarga una tabla, se pierden esos movimientos.
"""

from __future__ import annotations

import argparse
import datetime as dt
import decimal
import logging
import os
import re
import sys
import time
from dataclasses import dataclass, field
from typing import Any, Dict, Iterable, List, Optional, Sequence, Tuple

try:
    import pyodbc
except ImportError:  # pragma: no cover
    print("Falta pyodbc. Instalelo con:  pip install pyodbc", file=sys.stderr)
    raise SystemExit(2)


# ---------------------------------------------------------------------------
# Configuracion por defecto (se puede sobreescribir por linea de comandos)
# ---------------------------------------------------------------------------
RUTA_ACCESS = r"C:\Jaeger Soft\Jaeger.accdb"
PASSWORD_ACCESS = "75941232"          # contrasena Jet del archivo de Access
SERVIDOR_SQL = r".\SQLEXPRESS"
BASE_SQL = "PuntoDeVenta"
DRIVER_ACCESS = "Microsoft Access Driver (*.mdb, *.accdb)"
DRIVER_SQL = "ODBC Driver 17 for SQL Server"

# Las 57 tablas que deben existir en Access (control de integridad del origen)
TABLAS_ESPERADAS = [
    "Abonos", "AbonosApartado", "AbonosProveedores", "Almacenes", "Apartados",
    "Cambio", "Categorias", "Clientes", "Compras", "Compras2", "ComprasC",
    "ComprasMovimientos", "Configuracion", "ConsignaCliente",
    "ConsignaMovimientos", "Corte", "Cortes", "CortesB", "Credito",
    "Errores de pegado", "Fech", "Fisico", "FisicoDetallado", "Folios",
    "GastosDetallados", "GastosGeneral", "histocortes", "histocortesB",
    "Inventario", "InventarioAlmacenes", "InventarioSusp", "Kardex", "Linea",
    "MovimientosAlmacenes", "PermisosUsuario", "Poliza", "Poliza2",
    "Porcentaje", "ProduccionLotes", "ProductoReceta", "ProductosPoliza",
    "ProductosPoliza2", "Proveedores", "RespaldoInventario", "Servicios",
    "Tarjeta", "TmpVentasPeriodo", "Unidades", "Usuarios", "Ventas", "Ventas2",
    "VentasApartados", "VentasB", "VentasCajero", "VentasContado",
    "VentasCredito", "VentasPagos",
]

# Tipos destino de SQL Server que requieren conversion de valor
TIPOS_BIT = {"bit"}
TIPOS_NUMERICOS = {"int", "bigint", "smallint", "tinyint", "decimal", "numeric",
                   "money", "smallmoney", "float", "real"}
TIPOS_FECHA = {"date", "datetime", "datetime2", "smalldatetime", "time",
               "datetimeoffset"}
TIPOS_BINARIOS = {"varbinary", "binary", "image", "rowversion", "timestamp"}
TIPOS_TEXTO = {"char", "varchar", "text", "nchar", "nvarchar", "ntext",
               "sysname", "xml"}

log = logging.getLogger("migracion")


# ---------------------------------------------------------------------------
# Utilidades de conversion
# ---------------------------------------------------------------------------
_NUM_RE = re.compile(r"[^0-9eE\+\-\.]")
_VALORES_TRUE = {"1", "true", "verdadero", "si", "sí", "s", "yes", "y", "t"}
_VALORES_FALSE = {"0", "false", "falso", "no", "n", "f"}


def a_bit(valor: Any) -> Optional[int]:
    """Convierte el Si/No de Access (-1/0) al bit de SQL (1/0)."""
    if valor is None:
        return None
    if isinstance(valor, bool):
        return 1 if valor else 0
    if isinstance(valor, (int, float, decimal.Decimal)):
        return 0 if valor == 0 else 1
    texto = str(valor).strip().lower()
    if texto == "":
        return None
    if texto in _VALORES_TRUE:
        return 1
    if texto in _VALORES_FALSE:
        return 0
    return None


def a_numero(valor: Any) -> Optional[decimal.Decimal]:
    """Limpia '$1,234.50', '12%', '(50)' y devuelve Decimal o None."""
    if valor is None:
        return None
    if isinstance(valor, decimal.Decimal):
        return valor
    if isinstance(valor, bool):
        return decimal.Decimal(1 if valor else 0)
    if isinstance(valor, (int, float)):
        return decimal.Decimal(str(valor))
    texto = str(valor).strip()
    if texto == "":
        return None
    negativo = texto.startswith("(") and texto.endswith(")")
    texto = texto.replace(",", "").replace("%", "").replace(" ", "")
    texto = _NUM_RE.sub("", texto)
    if texto in ("", "-", "+", ".", "-.", "+."):
        return None
    try:
        numero = decimal.Decimal(texto)
    except (decimal.InvalidOperation, ValueError):
        return None
    return -numero if negativo else numero


def a_fecha(valor: Any) -> Optional[dt.datetime]:
    """Normaliza fechas de Access (datetime, date o texto heredado)."""
    if valor is None:
        return None
    if isinstance(valor, dt.datetime):
        return valor
    if isinstance(valor, dt.date):
        return dt.datetime(valor.year, valor.month, valor.day)
    texto = str(valor).strip()
    if texto == "":
        return None
    formatos = (
        "%Y-%m-%d %H:%M:%S", "%Y-%m-%dT%H:%M:%S", "%Y-%m-%d",
        "%d/%m/%Y %H:%M:%S", "%d/%m/%Y", "%m/%d/%Y %H:%M:%S", "%m/%d/%Y",
        "%Y/%m/%d %H:%M:%S", "%Y/%m/%d",
    )
    for formato in formatos:
        try:
            return dt.datetime.strptime(texto, formato)
        except ValueError:
            continue
    log.warning("Fecha no reconocida, se envia NULL: %r", valor)
    return None


def a_texto(valor: Any) -> Optional[str]:
    if valor is None:
        return None
    if isinstance(valor, str):
        return valor
    if isinstance(valor, (bytes, bytearray, memoryview)):
        return bytes(valor).decode("utf-8", errors="replace")
    return str(valor)


def a_binario(valor: Any) -> Optional[bytes]:
    if valor is None:
        return None
    if isinstance(valor, memoryview):
        return bytes(valor)
    if isinstance(valor, (bytes, bytearray)):
        return bytes(valor)
    return str(valor).encode("utf-8", errors="replace")


def convertir_fila(fila: Sequence[Any], tipos: Sequence[str]) -> List[Any]:
    """Convierte una fila del origen a los tipos destino de SQL Server."""
    salida = []
    for valor, tipo in zip(fila, tipos):
        tipo = (tipo or "").lower()
        if tipo in TIPOS_BIT:
            salida.append(a_bit(valor))
        elif tipo in TIPOS_NUMERICOS:
            salida.append(a_numero(valor))
        elif tipo in TIPOS_FECHA:
            salida.append(a_fecha(valor))
        elif tipo in TIPOS_BINARIOS:
            salida.append(a_binario(valor))
        elif tipo in TIPOS_TEXTO:
            salida.append(a_texto(valor))
        else:
            salida.append(valor)
    return salida


def clave_normalizada(fila: Sequence[Any], tipos: Sequence[str]) -> Tuple[str, ...]:
    """Representacion estable de una fila para comparar Access vs SQL."""
    partes = []
    for valor, tipo in zip(fila, tipos):
        tipo = (tipo or "").lower()
        if valor is None:
            partes.append("\x00NULL")
        elif tipo in TIPOS_NUMERICOS:
            numero = a_numero(valor)
            if numero is None:
                partes.append("\x00NULL")
            else:
                # normalize() elimina ceros a la derecha: 2.7000 == 2.7
                partes.append(format(numero.normalize(), "f"))
        elif tipo in TIPOS_BIT:
            partes.append(str(a_bit(valor)))
        elif tipo in TIPOS_FECHA:
            fecha = a_fecha(valor)
            partes.append(fecha.isoformat() if fecha else "\x00NULL")
        else:
            partes.append(str(valor))
    return tuple(partes)


def cotizar(nombre: str) -> str:
    """Escapa un identificador para SQL Server."""
    return "[" + nombre.replace("]", "]]") + "]"


# ---------------------------------------------------------------------------
# Metadatos
# ---------------------------------------------------------------------------
@dataclass
class ColumnaSQL:
    nombre: str
    orden: int
    tipo: str
    es_identidad: bool
    es_calculada: bool


@dataclass
class TablaSQL:
    nombre: str
    columnas: List[ColumnaSQL] = field(default_factory=list)

    @property
    def nombres(self) -> List[str]:
        return [c.nombre for c in self.columnas]

    @property
    def tipos(self) -> List[str]:
        return [c.tipo for c in self.columnas]

    @property
    def insertables(self) -> List[ColumnaSQL]:
        return [c for c in self.columnas if not c.es_calculada]

    @property
    def tiene_identidad(self) -> bool:
        return any(c.es_identidad for c in self.columnas)


# ---------------------------------------------------------------------------
# Acceso
# ---------------------------------------------------------------------------
class LectorAccess:
    def __init__(self, ruta: str, password: str):
        cadena = (
            f"DRIVER={{{DRIVER_ACCESS}}};DBQ={ruta};PWD={password};"
        )
        self.conexion = pyodbc.connect(cadena, autocommit=True, readonly=True)
        try:
            self.conexion.timeout = 120
        except pyodbc.Error:
            pass  # el driver de Access no implementa SQLSetConnectAttr
        self.cursor = self.conexion.cursor()

    def tablas(self) -> List[str]:
        return sorted(
            fila.table_name for fila in self.cursor.tables(tableType="TABLE")
        )

    def columnas(self, tabla: str) -> List[Tuple[str, str, int]]:
        """Devuelve [(nombre, tipo, orden)] en orden ordinal."""
        filas = [
            (c.column_name, c.type_name, int(c.ordinal_position or 0))
            for c in self.cursor.columns(table=tabla)
        ]
        return sorted(filas, key=lambda f: f[2])

    def contar(self, tabla: str) -> int:
        self.cursor.execute(f"SELECT COUNT(*) FROM {cotizar(tabla)}")
        return int(self.cursor.fetchval() or 0)

    def leer(self, tabla: str, tamano_lote: int) -> Iterable[List[Tuple[Any, ...]]]:
        self.cursor.execute(f"SELECT * FROM {cotizar(tabla)}")
        while True:
            filas = self.cursor.fetchmany(tamano_lote)
            if not filas:
                break
            yield filas


# ---------------------------------------------------------------------------
# SQL Server
# ---------------------------------------------------------------------------
class EscritorSQL:
    def __init__(self, servidor: str, base: str, driver: str = DRIVER_SQL,
                 usuario: str = "", password: str = ""):
        if usuario:
            cadena = (
                f"DRIVER={{{driver}}};SERVER={servidor};DATABASE={base};"
                f"UID={usuario};PWD={password};TrustServerCertificate=yes;"
            )
        else:
            cadena = (
                f"DRIVER={{{driver}}};SERVER={servidor};DATABASE={base};"
                "Trusted_Connection=yes;TrustServerCertificate=yes;"
            )
        self.conexion = pyodbc.connect(cadena, autocommit=False)
        try:
            self.conexion.timeout = 600
        except pyodbc.Error:
            pass

        self.cursor = self.conexion.cursor()

    def tablas(self) -> Dict[str, TablaSQL]:
        consulta = """
            SELECT t.name, c.name, c.column_id, ty.name, c.is_identity, c.is_computed
            FROM sys.tables t
            JOIN sys.columns c ON c.object_id = t.object_id
            JOIN sys.types ty ON ty.user_type_id = c.user_type_id
            ORDER BY t.name, c.column_id
        """
        resultado: Dict[str, TablaSQL] = {}
        for nombre, columna, orden, tipo, identidad, calculada in self.cursor.execute(consulta):
            tabla = resultado.setdefault(nombre, TablaSQL(nombre))
            tabla.columnas.append(
                ColumnaSQL(columna, int(orden), tipo,
                           bool(identidad), bool(calculada))
            )
        return resultado

    def contar(self, tabla: str) -> int:
        self.cursor.execute(f"SELECT COUNT(*) FROM {cotizar(tabla)}")
        return int(self.cursor.fetchval() or 0)

    def existe_tabla(self, tabla: str) -> bool:
        self.cursor.execute(
            "SELECT COUNT(*) FROM sys.tables WHERE name = ?", tabla)
        return bool(self.cursor.fetchval())

    def respaldar(self, tabla: str, sufijo: str) -> Optional[str]:
        destino = f"{tabla}_bkp_{sufijo}"
        if len(destino) > 120:
            destino = destino[:120]
        if self.existe_tabla(destino):
            log.info("    respaldo ya existente, se reutiliza: %s", destino)
            return destino
        self.cursor.execute(
            f"SELECT * INTO {cotizar(destino)} FROM {cotizar(tabla)}")
        self.conexion.commit()
        return destino

    def vaciar(self, tabla: str) -> None:
        try:
            self.cursor.execute(f"TRUNCATE TABLE {cotizar(tabla)}")
        except pyodbc.Error:
            log.warning("    TRUNCATE no permitido en %s, se usa DELETE", tabla)
            self.cursor.execute(f"DELETE FROM {cotizar(tabla)}")

    def insertar(self, tabla: TablaSQL, filas: List[Sequence[Any]],
                 identidad: bool) -> None:
        columnas = tabla.insertables
        lista = ", ".join(cotizar(c.nombre) for c in columnas)
        marcas = ", ".join("?" for _ in columnas)
        sentencia = (
            f"INSERT INTO {cotizar(tabla.nombre)} ({lista}) VALUES ({marcas})"
        )

        if identidad:
            self.cursor.execute(
                f"SET IDENTITY_INSERT {cotizar(tabla.nombre)} ON")

        try:
            self.cursor.fast_executemany = True
            self.cursor.executemany(sentencia, filas)
        except pyodbc.Error:
            # Algunos drivers/columnas no soportan fast_executemany:
            # reintentamos fila por fila (mas lento pero seguro).
            log.warning("    fast_executemany fallo en %s, se reintenta fila a fila",
                        tabla.nombre)
            self.cursor.fast_executemany = False
            for fila in filas:
                self.cursor.execute(sentencia, fila)
        finally:
            self.cursor.fast_executemany = False
            if identidad:
                self.cursor.execute(
                    f"SET IDENTITY_INSERT {cotizar(tabla.nombre)} OFF")

    def reseed(self, tabla: str) -> None:
        self.cursor.execute(f"DBCC CHECKIDENT ('{tabla}', RESEED) WITH NO_INFOMSGS")

    def leer_todo(self, tabla: TablaSQL) -> List[Tuple[Any, ...]]:
        columnas = ", ".join(cotizar(c.nombre) for c in tabla.columnas)
        self.cursor.execute(f"SELECT {columnas} FROM {cotizar(tabla.nombre)}")
        return self.cursor.fetchall()


# ---------------------------------------------------------------------------
# Comparacion Access vs SQL
# ---------------------------------------------------------------------------
def comparar_tabla(lector: LectorAccess, escritor: EscritorSQL,
                   tabla_sql: TablaSQL, tamano_lote: int,
                   profundo: bool) -> Tuple[bool, List[str]]:
    problemas: List[str] = []
    origen = lector.contar(tabla_sql.nombre)
    destino = escritor.contar(tabla_sql.nombre)
    if origen != destino:
        problemas.append(f"conteo Access={origen} vs SQL={destino}")

    if profundo and origen == destino:
        tipos = tabla_sql.tipos
        multiset_origen: Dict[Tuple[str, ...], int] = {}
        for lote in lector.leer(tabla_sql.nombre, tamano_lote):
            for fila in lote:
                clave = clave_normalizada(fila, tipos)
                multiset_origen[clave] = multiset_origen.get(clave, 0) + 1
        for fila in escritor.leer_todo(tabla_sql):
            clave = clave_normalizada(fila, tipos)
            if multiset_origen.get(clave, 0) <= 0:
                problemas.append(f"fila de SQL no encontrada en Access: {str(clave)[:120]}")
                break
            multiset_origen[clave] -= 1
        restantes = sum(v for v in multiset_origen.values() if v > 0)
        if restantes:
            problemas.append(f"{restantes} fila(s) de Access no llegaron a SQL")

    return (not problemas), problemas


# ---------------------------------------------------------------------------
# Flujo principal
# ---------------------------------------------------------------------------
def configurar_log(ruta_log: Optional[str]) -> None:
    log.setLevel(logging.INFO)
    formato = logging.Formatter("%(asctime)s %(levelname)-7s %(message)s",
                                "%H:%M:%S")
    consola = logging.StreamHandler(sys.stdout)
    consola.setFormatter(formato)
    log.addHandler(consola)
    if ruta_log:
        archivo = logging.FileHandler(ruta_log, encoding="utf-8")
        archivo.setFormatter(formato)
        log.addHandler(archivo)


def parsear_argumentos(argv: Optional[Sequence[str]] = None) -> argparse.Namespace:
    p = argparse.ArgumentParser(
        description="Migra los datos de Jaeger.accdb (Access) a SQL Server.",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="Ejemplo: python migrar_access_a_sql.py --mode verify --deep-check",
    )
    p.add_argument("--mode", choices=["verify", "dry-run", "load"], default="dry-run",
                   help="verify: solo compara | dry-run: simula | load: migra")
    p.add_argument("--access", default=RUTA_ACCESS, help="ruta del archivo .accdb")
    p.add_argument("--access-password", default=os.environ.get("JAEGER_ACCESS_PWD", PASSWORD_ACCESS),
                   help="contrasena del .accdb")
    p.add_argument("--server", default=SERVIDOR_SQL, help="instancia de SQL Server")
    p.add_argument("--database", default=BASE_SQL, help="base de datos destino")
    p.add_argument("--driver", default=DRIVER_SQL, help="driver ODBC de SQL Server")
    p.add_argument("--sql-user", default="", help="usuario SQL (vacio = Windows Auth)")
    p.add_argument("--sql-password", default="", help="contrasena SQL")
    p.add_argument("--tables", default="", help="lista separada por comas (solo esas)")
    p.add_argument("--skip", default="", help="lista separada por comas a excluir")
    p.add_argument("--batch", type=int, default=2000, help="filas por lote (default 2000)")
    p.add_argument("--backup", action="store_true",
                   help="copia cada tabla destino a <tabla>_bkp_<fecha> antes de vaciarla")
    p.add_argument("--deep-check", action="store_true",
                   help="compara el contenido completo, no solo los conteos")
    p.add_argument("--yes", action="store_true", help="no pedir confirmacion en modo load")
    p.add_argument("--log", default="", help="archivo de log (default: automatico)")
    p.add_argument("--list-tables", action="store_true", help="lista las tablas y sale")
    return p.parse_args(argv)


def seleccionar_tablas(disponibles: List[str], incluir: str, excluir: str) -> List[str]:
    if incluir:
        pedidas = [t.strip() for t in incluir.split(",") if t.strip()]
        faltantes = [t for t in pedidas if t not in disponibles]
        if faltantes:
            log.error("No existen en Access: %s", ", ".join(faltantes))
            raise SystemExit(3)
        return pedidas
    excluidas = {t.strip().lower() for t in excluir.split(",") if t.strip()}
    return [t for t in disponibles if t.lower() not in excluidas]


def confirmar(mensaje: str) -> bool:
    try:
        respuesta = input(f"{mensaje} [escribe SI para continuar]: ").strip()
    except EOFError:
        return False
    return respuesta.upper() == "SI"


def main(argv: Optional[Sequence[str]] = None) -> int:
    args = parsear_argumentos(argv)
    sufijo = dt.datetime.now().strftime("%Y%m%d_%H%M")
    ruta_log = args.log or f"migracion_{sufijo}.log"
    configurar_log(ruta_log)

    log.info("=" * 78)
    log.info("MIGRACION ACCESS -> SQL SERVER   modo=%s", args.mode)
    log.info("Origen : %s", args.access)
    log.info("Destino: %s / %s", args.server, args.database)
    log.info("=" * 78)

    if not os.path.exists(args.access):
        log.error("No existe el archivo de Access: %s", args.access)
        return 2

    try:
        lector = LectorAccess(args.access, args.access_password)
    except pyodbc.Error as ex:
        log.error("No se pudo abrir Access (%s). Verifique que el archivo existe, "
                  "que la contrasena es correcta y que el POS esta cerrado.", args.access)
        log.error("Detalle: %s", ex)
        return 2

    try:
        escritor = EscritorSQL(args.server, args.database, args.driver,
                               args.sql_user, args.sql_password)
    except pyodbc.Error as ex:
        log.error("No se pudo conectar a SQL Server (%s / %s).", args.server, args.database)
        log.error("Detalle: %s", ex)
        return 2

    tablas_access = lector.tablas()
    log.info("Tablas encontradas en Access: %d", len(tablas_access))

    if args.list_tables:
        for nombre in tablas_access:
            log.info("  %-24s filas=%d", nombre, lector.contar(nombre))
        return 0

    # Control de integridad del origen
    faltantes_origen = sorted(set(TABLAS_ESPERADAS) - set(tablas_access))
    if faltantes_origen:
        log.warning("Faltan en Access tablas esperadas: %s",
                    ", ".join(faltantes_origen))
    sobrantes = sorted(set(tablas_access) - set(TABLAS_ESPERADAS))
    if sobrantes:
        log.warning("Tablas en Access fuera del plan (se migraran igual): %s",
                    ", ".join(sobrantes))

    tablas_sql = escritor.tablas()
    seleccion = seleccionar_tablas(tablas_access, args.tables, args.skip)
    log.info("Tablas a procesar: %d", len(seleccion))

    if args.mode == "load" and not args.yes:
        log.warning("")
        log.warning("MODO LOAD: se VACIARAN las tablas destino %s.", args.database)
        log.warning("Cierre el POS y respalde el .accdb antes de continuar.")
        log.warning("")
        if not confirmar("¿Continuar con la migracion real?"):
            log.info("Cancelado por el usuario.")
            return 1

    if args.mode == "load" and not args.backup:
        log.warning("Sin --backup: las tablas destino se vaciaran sin copia previa.")

    inicio = time.time()
    errores: List[str] = []
    avisos: List[str] = []
    filas_totales = 0
    tablas_ok = 0

    for indice, nombre in enumerate(seleccion, 1):
        log.info("[%d/%d] %s", indice, len(seleccion), nombre)
        tabla_sql = tablas_sql.get(nombre)
        if tabla_sql is None:
            errores.append(f"{nombre}: no existe en SQL Server")
            log.error("    no existe en SQL Server")
            continue

        # Correspondencia de columnas (por nombre; el orden se informa)
        cols_access = lector.columnas(nombre)
        nombres_access = [c[0] for c in cols_access]
        nombres_sql = tabla_sql.nombres
        faltan_en_sql = [c for c in nombres_access if c not in nombres_sql]
        faltan_en_access = [c for c in nombres_sql if c not in nombres_access]
        if faltan_en_sql:
            errores.append(f"{nombre}: columnas en Access sin equivalente en SQL: {faltan_en_sql}")
            log.error("    columnas sin equivalente en SQL: %s", faltan_en_sql)
            continue
        if faltan_en_access:
            avisos.append(f"{nombre}: columnas SQL que Access no tiene (se dejan tal cual): {faltan_en_access}")
            log.warning("    columnas SQL sin origen en Access: %s", faltan_en_access)
        if [c.lower() for c in nombres_access] != [c.lower() for c in nombres_sql[:len(nombres_access)]]:
            avisos.append(f"{nombre}: el orden ordinal de columnas difiere entre Access y SQL")
            log.warning("    el orden ordinal de columnas difiere entre Access y SQL")

        try:
            if args.mode == "verify":
                ok, problemas = comparar_tabla(lector, escritor, tabla_sql,
                                               args.batch, args.deep_check)
                if ok:
                    tablas_ok += 1
                    log.info("    OK  (%d filas)", escritor.contar(nombre))
                else:
                    errores.append(f"{nombre}: " + "; ".join(problemas))
                    log.error("    DIFERENCIAS: %s", "; ".join(problemas))
                continue

            if args.mode == "dry-run":
                log.info("    Access=%d filas, SQL=%d filas, columnas=%d",
                         lector.contar(nombre), escritor.contar(nombre),
                         len(nombres_access))
                tablas_ok += 1
                continue

            # ------------------------- modo load -------------------------
            respaldo = None
            if args.backup:
                respaldo = escritor.respaldar(nombre, sufijo)
                log.info("    respaldo: %s", respaldo)

            origen_total = lector.contar(nombre)
            escritor.vaciar(nombre)
            escritor.conexion.commit()

            insertadas = 0
            tipos = tabla_sql.tipos
            for lote in lector.leer(nombre, args.batch):
                convertidas = [convertir_fila(fila, tipos) for fila in lote]
                escritor.insertar(tabla_sql, convertidas, tabla_sql.tiene_identidad)
                insertadas += len(convertidas)
            escritor.conexion.commit()

            if tabla_sql.tiene_identidad:
                escritor.reseed(nombre)
                escritor.conexion.commit()

            destino_total = escritor.contar(nombre)
            filas_totales += insertadas
            if insertadas != origen_total or destino_total != origen_total:
                errores.append(
                    f"{nombre}: Access={origen_total}, leidas={insertadas}, en SQL={destino_total}")
                log.error("    DESCUADRE  Access=%d  leidas=%d  SQL=%d",
                          origen_total, insertadas, destino_total)
            else:
                tablas_ok += 1
                log.info("    OK  %d filas", insertadas)

            if args.deep_check:
                ok, problemas = comparar_tabla(lector, escritor, tabla_sql,
                                               args.batch, True)
                if not ok:
                    errores.append(f"{nombre} (contenido): " + "; ".join(problemas))
                    log.error("    contenido distinto: %s", "; ".join(problemas))

        except Exception as ex:  # noqa: BLE001 - reportamos y seguimos con la siguiente
            try:
                escritor.conexion.rollback()
            except Exception:  # pragma: no cover
                pass
            errores.append(f"{nombre}: {ex}")
            log.exception("    ERROR al procesar %s", nombre)

    duracion = time.time() - inicio
    log.info("-" * 78)
    log.info("Tablas OK: %d/%d   filas migradas: %d   tiempo: %.1fs",
             tablas_ok, len(seleccion), filas_totales, duracion)
    if avisos:
        log.info("Avisos (%d):", len(avisos))
        for aviso in avisos:
            log.info("  - %s", aviso)
    if errores:
        log.error("Errores (%d):", len(errores))
        for error in errores:
            log.error("  - %s", error)
        log.error("MIGRACION CON ERRORES. Revise el log: %s", ruta_log)
        return 1

    log.info("Migracion completada sin errores. Log: %s", ruta_log)
    return 0


if __name__ == "__main__":
    sys.exit(main())
