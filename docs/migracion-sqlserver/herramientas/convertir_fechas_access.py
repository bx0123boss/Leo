#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Convierte los delimitadores de fecha de Access (#...#) a cadenas entre comillas
simples con formato ISO (yyyy-MM-dd HH:mm:ss), que SQL Server interpreta igual
sin importar el idioma del login.

Formas que maneja:

  A) Fecha construida inline (M/D/YYYY, ambiguo para SQL Server):
       Fecha >=#" + picker.Value.Month.ToString() + "/" + ... + " 00:00:00# ...
     -> Fecha >='" + picker.Value.ToString("yyyy-MM-dd") + " 00:00:00' ...

  B) Fecha que ya viene en una variable con formato ISO:
       "WHERE Fecha >= #" + fechaInicioSQL + "# and Fecha <= #" + fechaFinSQL + "# "
     -> "WHERE Fecha >= '" + fechaInicioSQL + "' and Fecha <= '" + fechaFinSQL + "' "

  C) Interpolacion C# (se corrige a mano, aqui solo se reporta).
"""
import re
import sys

FILES = [
    "frmApartados.cs", "frmCompras.cs", "frmCompras2.cs",
    "frmConsultaVentasCategoria.cs", "frmCorte.cs", "frmInventariosFisicos.cs",
    "frmKardex.cs", "frmPolizas.cs", "frmPolizas2.cs",
    "frmReporteVentasCredito.cs", "frmServicios.cs",
    "frmVentasDiariasVendedor.cs", "frmVentasVendedorFechas.cs",
]

# Forma A: ">=#" + X.Value.Month.ToString() + "/" + ... + " 00:00:00#"
RE_INLINE = re.compile(
    r'([<>]=?)#"\s*\+\s*'
    r'(\w+(?:\.Value)?)\.Month\.ToString\(\)\s*\+\s*"/"\s*\+\s*'
    r'\2\.Day\.ToString\(\)\s*\+\s*"/"\s*\+\s*'
    r'\2\.Year\.ToString\(\)\s*\+\s*'
    r'"\s(00:00:00|23:59:59)#'
)

# Forma B: ">= #" + fechaInicioSQL + "#"   /   "<=#" + fechaFinSQL + "#"
RE_VAR = re.compile(r'([<>]=?)\s*#" \+\s*(\w+)\s*\+\s*"#')


def transform(text):
    """Devuelve (texto, n_cambios)."""
    count = 0

    def rep_inline(m):
        nonlocal count
        count += 1
        return "%s'\" + %s.ToString(\"yyyy-MM-dd\") + \" %s'" % (
            m.group(1), m.group(2), m.group(3))

    def rep_var(m):
        nonlocal count
        count += 1
        return "%s '\" + %s + \"'" % (m.group(1), m.group(2))

    lines = text.splitlines(keepends=True)
    out = []
    for line in lines:
        new = RE_VAR.sub(rep_var, line)
        new = RE_INLINE.sub(rep_inline, new)
        # Restos del literal que aun contiene '#' (delimitador de cierre/apertura)
        if "#" in new and ("Fecha" in new or "fecha" in new):
            new = new.replace("#", "'")
        out.append(new)
    return "".join(out), count


def main():
    apply = "--apply" in sys.argv
    grand = 0
    for name in FILES:
        path = "BRUNO/" + name
        with open(path, encoding="utf-8", newline="") as f:
            src = f.read()
        new, n = transform(src)
        print("%-34s %d" % (name, n))
        grand += n
        if apply and new != src:
            with open(path, "w", encoding="utf-8", newline="") as f:
                f.write(new)
    print("TOTAL convertidos:", grand)


if __name__ == "__main__":
    main()
