#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Elimina formularios WinForms muertos: borra sus .cs / .Designer.cs / .resx y
quita sus entradas del .csproj, sin tocar el resto del archivo.

Se usa con la lista de formularios que nunca se instancian (`new frmX(`).
Antes de borrar conviene verificar que no haya referencias por reflection;
este proyecto no las tiene.
"""
import os
import re
import sys

CSPROJ = "BRUNO/Jaegersoft.csproj"
FORMULARIOS = [
    "frmAdeudoProveedor",
    "frmCaptura",
    "frmCredito",
    "frmDescuentoCancelacion",
    "frmDescuentoPorCategoria",
    "frmPrecios",
]


def main():
    aplicar = "--apply" in sys.argv

    with open(CSPROJ, encoding="utf-8-sig", newline="") as f:
        csproj = f.read()
    original = csproj

    for nombre in FORMULARIOS:
        for ext in (".cs", ".Designer.cs", ".resx"):
            ruta = os.path.join("BRUNO", nombre + ext)
            if os.path.exists(ruta):
                print(("borrar  " if aplicar else "borraria ") + ruta)
                if aplicar:
                    os.remove(ruta)
            else:
                print("AVISO: no existe " + ruta)

        # <Compile Include="X.cs"> ... </Compile>  y  <Compile Include="X.Designer.cs"> ... </Compile>
        csproj = re.sub(
            r'[ \t]*<Compile Include="' + re.escape(nombre) + r'(?:\.Designer)?\.cs">.*?</Compile>\r?\n',
            "", csproj, flags=re.DOTALL)
        # <EmbeddedResource Include="X.resx"> ... </EmbeddedResource>
        csproj = re.sub(
            r'[ \t]*<EmbeddedResource Include="' + re.escape(nombre) + r'\.resx">.*?</EmbeddedResource>\r?\n',
            "", csproj, flags=re.DOTALL)

    if aplicar and csproj != original:
        with open(CSPROJ, "w", encoding="utf-8", newline="") as f:
            f.write(csproj)
        print("csproj actualizado")
    elif not aplicar:
        print("(simulacion: use --apply)")


if __name__ == "__main__":
    main()
