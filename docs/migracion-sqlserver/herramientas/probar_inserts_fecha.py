#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Prueba de humo de los INSERT que escriben fechas.

Toma la forma real de cada INSERT del codigo (tabla + lista de columnas),
la rellena con valores de ejemplo acordes al tipo de cada columna y la ejecuta
contra la base de pruebas dentro de una transaccion que SIEMPRE se revierte.

Objetivo: validar el texto SQL (columnas, tipos, formato de fecha ISO) sin
dejar datos. No sustituye a las pruebas funcionales del POS.

Uso:
    python probar_inserts_fecha.py
    python probar_inserts_fecha.py --database PuntoDeVenta_ETL_Test
"""
import argparse
import os
import re
import sys

import pyodbc

DRIVER = "ODBC Driver 17 for SQL Server"
SERVIDOR = r".\SQLEXPRESS"
BASE = "PuntoDeVenta"

# Formas (tabla, columnas) extraidas del codigo; se completa con las que
# encuentre el propio script al leer los .cs.
FORMAS = [
    ("Kardex", ["IdProducto", "Tipo", "Descripcion", "ExistenciaAntes", "ExistenciaDespues", "Fecha"]),
    ("Kardex", ["IdProducto", "Tipo", "Descripcion", "ExistenciaAntes", "ExistenciaDespues", "Fecha", "Precio"]),
    ("Corte", ["Concepto", "Monto", "FechaHora"]),
    ("Corte", ["Concepto", "Monto", "FechaHora", "Pago"]),
    ("Abonos", ["Abono", "idCliente", "Fecha", "Nombre", "Folio", "Estatus"]),
    ("AbonosApartado", ["Abono", "idCliente", "Fecha", "Folio"]),
    ("AbonosProveedores", ["idProveedor", "Fecha", "Monto", "AdeudoActual"]),
    ("AbonosProveedores", ["idProveedor", "Fecha", "Monto", "Unidad", "ValorUnidad", "ValorPesos", "AdeudoActual"]),
    ("Apartados", ["MontoTotal", "Fecha", "Folio", "IdCliente", "NombreCliente", "Abono", "Restante"]),
    ("Captura", ["IdProducto", "Cantidad", "Fecha"]),
    ("Compras", ["Fecha", "Folio", "Tipo"]),
    ("Compras2", ["Movimiento", "Fecha", "Capturo", "Relacion"]),
    ("ComprasC", ["FolioCompra", "IdProducto", "CantidadAnterior", "CantidadActual", "Producto", "Fecha"]),
    ("ComprasMovimientos", ["Folio", "IdProducto", "Nombre", "ExistenciasAntiguas", "Movimiento", "ExistenciasActuales", "Tipo", "Fecha"]),
    ("ConsignaMovimientos", ["ClienteId", "ProductoId", "TipoMovimiento", "Cantidad", "PrecioVigente", "Fecha"]),
    ("ConsignaMovimientos", ["ClienteId", "ProductoId", "TipoMovimiento", "Cantidad", "PrecioVigente", "Fecha", "ReferenciaVentaId"]),
    ("Fisico", ["Fecha"]),
    ("FisicoDetallado", ["Id", "Nombre", "Existen", "Fisico", "Diferencia", "IdFisico"]),
    ("GastosDetallados", ["idGasto", "Total", "Fecha"]),
    ("histocortes", ["Id", "Monto", "Fecha", "Mas", "Menos", "Tarjeta", "utilidad", "inversion"]),
    ("histocortesB", ["Id", "Monto", "Fecha", "Mas", "Menos", "Tarjeta", "utilidad", "inversion", "CubreFolio"]),
    ("MovimientosAlmacenes", ["IdOrigen", "IdDestino", "IdProducto", "Cantidad", "Fecha", "Usuario"]),
    ("Poliza", ["Folio", "Fecha", "FechaCaptura", "CostoTotal", "CostoExtra", "IdProv", "Proveedor", "IVA", "Total", "IdAlmacen"]),
    ("ProduccionLotes", ["Folio", "IdProductoFinal", "CantidadProducida", "Fecha", "Usuario"]),
    ("Servicios", ["Producto", "Peso", "Descripcion", "Servicio", "Fecha", "FechaEntrega", "Presupuesto", "idCliente", "NombreCliente", "Folio", "CostoReal", "Abono"]),
    ("Ventas", ["Monto", "Fecha", "Folio", "Estatus", "Descuento", "Pago"]),
    ("Ventas2", ["Monto", "Fecha", "Folio", "IdCliente", "Estatus", "Saldo"]),
    ("Ventas2", ["Monto", "Fecha", "Folio", "IdCliente", "Saldo"]),
    ("VentasApartados", ["FolioVenta", "idProducto", "Cantidad", "Producto", "MontoTotal", "IdCliente", "Fecha", "Estatus"]),
    ("VentasB", ["Monto", "Fecha", "FolioA", "Estatus", "Descuento", "Pago", "FolioB"]),
    ("VentasCajero", ["IdUsuario", "Usuario", "FolioVenta", "Total", "Fecha", "Cajero"]),
    ("VentasContado", ["FolioVenta", "IdProducto", "Cantidad", "Producto", "MontoTotal", "idCliente", "Fecha", "Utilidad", "Categoria"]),
    ("VentasCredito", ["FolioVenta", "IdProducto", "Cantidad", "Producto", "MontoTotal", "IdCliente", "Fecha", "Categoria"]),
    ("VentasPagos", ["FolioVenta", "MetodoPago", "Monto"]),
]

FECHA_ISO = "2026-11-05 16:33:04"


def cotizar(nombre):
    return "[" + nombre.replace("]", "]]") + "]"


def valor_ejemplo(columna, tipo, longitud):
    tipo = (tipo or "").lower()
    col = columna.lower()
    if tipo in ("int", "bigint", "smallint", "tinyint"):
        # en tablas cuyo Id lo pone la aplicacion usamos un valor que no choque
        return "999999" if col == "id" else "1"
    if tipo in ("decimal", "numeric", "money", "smallmoney", "float", "real"):
        return "1"
    if tipo == "bit":
        return "0"
    if tipo in ("datetime", "datetime2", "smalldatetime", "date"):
        return "'" + FECHA_ISO + "'"
    if tipo == "time":
        return "'16:33:04'"
    if tipo in ("varbinary", "binary", "image"):
        return "0x00"
    if tipo in ("nvarchar", "varchar", "nchar", "char", "text", "ntext"):
        if col.startswith("fecha"):
            return "'" + FECHA_ISO + "'"
        return "'ZZTEST'" if col == "id" else "'PRUEBA'"
    return "'PRUEBA'"


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--server", default=SERVIDOR)
    ap.add_argument("--database", default=BASE)
    ap.add_argument("--verbose", action="store_true")
    args = ap.parse_args()

    con = pyodbc.connect(
        f"DRIVER={{{DRIVER}}};SERVER={args.server};DATABASE={args.database};"
        "Trusted_Connection=yes;TrustServerCertificate=yes;",
        autocommit=False,
    )
    cur = con.cursor()

    # Esquema: tabla -> columna -> (tipo, nullable, identidad, calculada)
    esquema = {}
    for tabla, col, tipo, nullable, ident, calc in cur.execute(
        """
        SELECT t.name, c.name, ty.name, c.is_nullable, c.is_identity, c.is_computed
        FROM sys.tables t
        JOIN sys.columns c ON c.object_id = t.object_id
        JOIN sys.types ty ON ty.user_type_id = c.user_type_id
        """
    ):
        esquema.setdefault(tabla, {})[col.lower()] = (col, tipo, bool(nullable), bool(ident), bool(calc))

    ok = 0
    fallos = []
    for tabla, columnas in FORMAS:
        meta = esquema.get(tabla)
        if meta is None:
            fallos.append((tabla, "la tabla no existe en la base de pruebas"))
            continue
        faltantes = [c for c in columnas if c.lower() not in meta]
        if faltantes:
            fallos.append((tabla, f"columnas inexistentes: {faltantes}"))
            continue

        valores = []
        columnas_insert = []
        for col in columnas:
            col_real, tipo, nullable, ident, calc = meta[col.lower()]
            col = col_real
            if calc:
                continue
            if ident and col.lower() == "id":
                continue  # el ID lo genera la base
            columnas_insert.append(col)
            valores.append(valor_ejemplo(col, tipo, None))

        sql = "INSERT INTO {} ({}) VALUES ({})".format(
            cotizar(tabla),
            ", ".join(cotizar(c) for c in columnas_insert),
            ", ".join(valores),
        )
        if args.verbose:
            print(sql)

        try:
            cur.execute("BEGIN TRANSACTION")
            cur.execute(sql)
            filas = cur.rowcount
            cur.execute("ROLLBACK TRANSACTION")
            ok += 1
            print(f"OK    {tabla:<22} ({len(columnas_insert)} columnas, rowcount={filas})")
        except Exception as ex:  # noqa: BLE001
            try:
                cur.execute("ROLLBACK TRANSACTION")
            except Exception:
                pass
            fallos.append((tabla, str(ex).split("\n")[0][:220]))
            print(f"FALLA {tabla:<22} {str(ex).split(chr(10))[0][:160]}")

    print("-" * 78)
    print(f"OK: {ok}   fallos: {len(fallos)}")
    for tabla, msg in fallos:
        print(f"  - {tabla}: {msg}")
    return 0 if not fallos else 1


if __name__ == "__main__":
    sys.exit(main())
