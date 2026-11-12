#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Convierte las fechas de SQL que se construian con formato dependiente de la
cultura:

    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()

por formato ISO invariante (el unico que SQL Server interpreta igual con
cualquier SET LANGUAGE / DATEFORMAT):

    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

Solo toca las ocurrencias que estan en un contexto SQL (INSERT/UPDATE/SELECT/
VALUES/WHERE/AddWithValue) o en variables `fecha*` que alimentan parametros.
Las que son solo para mostrar (tickets, Excel, MessageBox) se dejan intactas.
"""
import os
import re
import sys

ROOT = "."
SKIP = {"bin", "obj", ".git", ".vs", "packages", "node_modules"}

# Patron (admite el caso partido en dos lineas de concatenacion)
PATRON = re.compile(
    r"\(\s*DateTime\.Now\.ToShortDateString\(\)\s*\+\s*\"\s*\"\s*\+\s*"
    r"(?:\r?\n\s*)?DateTime\.Now\.ToShortTimeString\(\)\s*\)"
    r"|DateTime\.Now\.ToShortDateString\(\)\s*\+\s*\"\s*\"\s*\+\s*"
    r"(?:\r?\n\s*)?DateTime\.Now\.ToShortTimeString\(\)"
)

REEMPLAZO = 'DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")'

SQL_KW = re.compile(
    r"\b(insert|update|select|values|where|parameters\.addwithvalue|"
    r"parameters\.add|commandtext|new sqlcommand)\b|"
    r"\bfecha\w*\s*=",
    re.IGNORECASE,
)


def main():
    aplicar = "--apply" in sys.argv
    total = 0
    archivos = 0
    detalle = []

    for dp, dn, fn in os.walk(ROOT):
        dn[:] = [d for d in dn if d not in SKIP]
        for nombre in fn:
            if not nombre.endswith(".cs"):
                continue
            ruta = os.path.join(dp, nombre)
            with open(ruta, encoding="utf-8", newline="") as f:
                texto = f.read()
            if "ToShortDateString" not in texto:
                continue

            cambios = 0
            salida = []
            ultimo = 0
            for m in PATRON.finditer(texto):
                inicio_linea = texto.rfind("\n", 0, m.start()) + 1
                fin_linea = texto.find("\n", m.end())
                if fin_linea == -1:
                    fin_linea = len(texto)
                # contexto: linea actual + 2 anteriores y 2 siguientes
                ctx_ini = inicio_linea
                for _ in range(3):
                    prev = texto.rfind("\n", 0, ctx_ini - 1)
                    if prev == -1:
                        ctx_ini = 0
                        break
                    ctx_ini = prev + 1
                ctx_fin = fin_linea
                for _ in range(3):
                    nxt = texto.find("\n", ctx_fin + 1)
                    if nxt == -1:
                        ctx_fin = len(texto)
                        break
                    ctx_fin = nxt
                contexto = texto[ctx_ini:ctx_fin]

                if not SQL_KW.search(contexto):
                    continue

                salida.append(texto[ultimo:m.start()])
                salida.append(REEMPLAZO)
                ultimo = m.end()
                cambios += 1
                detalle.append((ruta, texto.count("\n", 0, m.start()) + 1,
                                " ".join(m.group(0).split())[:90]))

            if cambios:
                salida.append(texto[ultimo:])
                nuevo = "".join(salida)
                archivos += 1
                total += cambios
                if aplicar:
                    with open(ruta, "w", encoding="utf-8", newline="") as f:
                        f.write(nuevo)

    for ruta, linea, fragmento in detalle:
        print("%-38s L%-5d %s" % (os.path.basename(ruta), linea, fragmento))
    print()
    print("archivos: %d   ocurrencias SQL corregidas: %d" % (archivos, total))
    if not aplicar:
        print("(simulacion: use --apply para escribir los cambios)")


if __name__ == "__main__":
    main()
