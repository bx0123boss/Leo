#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Repara el mojibake (doble codificacion UTF-8 <-> Windows-1252) que la migracion
masiva dejo en algunos archivos: 'Ã‚Â¿Deseas...' -> '¿Deseas...'.

Trabaja solo sobre "rachas" de caracteres altos y aplica cp1252->utf8 hasta que
el texto quede estable, sin caracteres de reemplazo y sin el patron de mojibake.
"""
import os
import re
import sys

FILES = [
    "BRUNO/frmVentasCredito.cs",
    "BRUNO/frmVentaDetallada.cs",
    "BRUNO/frmEditarProducto.cs",
    "BRUNO/frmPrincipal.cs",
]

# Caracteres tipicos del mojibake: suplemento Latin-1 y los 0x80-0x9F de cp1252
HIGH = ("\u0080-\u00ff\u20ac\u201a\u0192\u201e\u2026\u2020\u2021\u02c6\u2030"
        "\u0160\u2039\u0152\u017d\u2018\u2019\u201c\u201d\u2022\u2013\u2014"
        "\u02dc\u2122\u0161\u203a\u0153\u017e\u0178")
RUN = re.compile("[%s]{2,}" % HIGH)
MARK = re.compile("[\u00c3\u00c2]")


def fix_run(run):
    """Intenta deshacer el mojibake de una racha. Devuelve (texto, ok)."""
    cur = run
    for _ in range(3):
        if not MARK.search(cur):
            break
        try:
            cand = cur.encode("cp1252").decode("utf-8")
        except (UnicodeEncodeError, UnicodeDecodeError):
            return run, False
        if cand == cur or "\ufffd" in cand:
            return run, False
        cur = cand
    ok = not MARK.search(cur)
    return cur, ok


def fix_line(line):
    out = []
    changed = False
    for m in RUN.finditer(line):
        fixed, ok = fix_run(m.group(0))
        if ok and fixed != m.group(0):
            changed = True
            out.append((m.start(), m.end(), fixed))
    if not changed:
        return line, False
    res = []
    last = 0
    for start, end, text in out:
        res.append(line[last:start])
        res.append(text)
        last = end
    res.append(line[last:])
    return "".join(res), True


def main():
    apply = "--apply" in sys.argv
    total = 0
    pending = 0
    for path in FILES:
        with open(path, encoding="utf-8", newline="") as f:
            src = f.read()
        lines = src.splitlines(keepends=True)
        out = []
        n = 0
        for line in lines:
            new, changed = fix_line(line)
            if changed:
                n += 1
            if "\u00c3" in new or "\u00c2" in new:
                if "--verbose" in sys.argv:
                    print("  PENDIENTE %s: %s" % (path, new.strip()[:120]))
                pending += 1
            out.append(new)
        print("%-36s lineas corregidas: %d" % (path, n))
        total += n
        if apply and n:
            with open(path, "w", encoding="utf-8", newline="") as f:
                f.write("".join(out))
    print("TOTAL lineas:", total, "| pendientes:", pending)


if __name__ == "__main__":
    main()
