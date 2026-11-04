#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Repara la corrupcion de encoding (U+FFFD) introducida por la migracion masiva
OleDb -> SqlClient, usando como referencia los blobs de git HEAD.

Cada caracter no-ASCII del archivo original quedo convertido en U+FFFD.
El script alinea (difflib) el texto corregido de HEAD con el texto actual
(ambos "saneados" para que los no-ASCII sean un mismo marcador) y restaura
solamente las posiciones que hoy son U+FFFD.

Uso:
    python reparar_encoding.py --ref <carpeta_ref> [--apply]
"""
import argparse
import difflib
import os
import sys

MARK = "\u00a7"      # marcador de "caracter no ASCII"
BAD = "\ufffd"       # caracter de reemplazo

SKIP_DIRS = {"bin", "obj", ".git", ".vs", "packages", "node_modules"}


def sanitize(text):
    return "".join(MARK if ord(c) > 127 else c for c in text)


def corrupted_files(root):
    out = []
    for dirpath, dirnames, filenames in os.walk(root):
        dirnames[:] = [d for d in dirnames if d not in SKIP_DIRS]
        for fn in filenames:
            if not fn.endswith(".cs"):
                continue
            p = os.path.join(dirpath, fn)
            try:
                with open(p, "r", encoding="utf-8", errors="strict") as f:
                    if BAD in f.read():
                        out.append(os.path.relpath(p, root))
            except (UnicodeDecodeError, OSError):
                continue
    return out


def repair_text(head, work):
    """Devuelve (texto_reparado, n_reparados, n_pendientes)."""
    hs, ws = sanitize(head), sanitize(work)
    sm = difflib.SequenceMatcher(None, hs, ws, autojunk=False)
    out = list(work)
    fixed = 0
    pending = 0
    for tag, i1, i2, j1, j2 in sm.get_opcodes():
        hblk, wblk = hs[i1:i2], ws[j1:j2]
        if tag == "equal":
            for k in range(len(wblk)):
                if out[j1 + k] == BAD:
                    out[j1 + k] = head[i1 + k]
                    fixed += 1
        elif tag == "replace":
            # El bloque solo debe diferir por palabras ASCII (OleDb -> Sql, etc.)
            h_bad = [k for k, c in enumerate(hblk) if c == MARK]
            w_bad = [k for k, c in enumerate(wblk) if out[j1 + k] == BAD]
            if len(h_bad) == len(w_bad):
                for kh, kw in zip(h_bad, w_bad):
                    out[j1 + kw] = head[i1 + kh]
                    fixed += 1
            else:
                pending += len(w_bad)
        else:
            pending += sum(1 for k in range(j1, j2) if out[k] == BAD)
    return "".join(out), fixed, pending


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--ref", required=True, help="carpeta con el arbol de git HEAD")
    ap.add_argument("--root", default=".")
    ap.add_argument("--apply", action="store_true")
    args = ap.parse_args()

    files = corrupted_files(args.root)
    total_fixed = total_pending = 0
    no_ref = []
    detail = []
    for rel in files:
        ref_path = os.path.join(args.ref, rel)
        if not os.path.exists(ref_path):
            no_ref.append(rel)
            continue
        with open(ref_path, "r", encoding="utf-8-sig", newline="") as f:
            head = f.read()
        work_path = os.path.join(args.root, rel)
        with open(work_path, "r", encoding="utf-8", newline="") as f:
            work = f.read()
        new, fixed, pending = repair_text(head, work)
        total_fixed += fixed
        total_pending += pending
        if fixed or pending:
            detail.append((rel, fixed, pending))
        if args.apply and fixed:
            with open(work_path, "w", encoding="utf-8", newline="") as f:
                f.write(new)

    for rel, fixed, pending in sorted(detail, key=lambda x: -x[2])[:40]:
        print(f"{rel}: reparados={fixed} pendientes={pending}")
    print()
    print(f"Archivos con U+FFFD: {len(files)}")
    print(f"Caracteres reparados: {total_fixed}")
    print(f"Caracteres pendientes: {total_pending}")
    if no_ref:
        print("Sin referencia en HEAD (revisar a mano): " + ", ".join(no_ref))
    return 0 if total_pending == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
