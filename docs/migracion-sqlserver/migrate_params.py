import re
import sys
import os

target_files = [
    r"BRUNO\frmAgregarInventario.cs",
    r"BRUNO\frmConsigna.cs",
    r"BRUNO\frmConsignaCobrar.cs",
    r"BRUNO\frmConsignaDevolver.cs",
    r"BRUNO\frmConsignaEntregar.cs",
    r"BRUNO\frmEditarProducto.cs"
]

def process_file(filepath):
    with open(filepath, "r", encoding="utf-8") as f:
        content = f.read()

    # Step 1: Let's find every `new SqlCommand(...)` or query string assignment followed by cmd.Parameters.Add...
    # In C#, each query is either inside `new SqlCommand("...", ...)` or assigned to a string / cmd.CommandText.
    # Rather than a full C# AST, let's look at the blocks.
    
    # Notice: In all these files, queries have "?" in strings, and parameters are added with `Parameters.AddWithValue("?", ...)` or `Parameters.Add("?", ...)`
    
    # Let's inspect each occurrence of AddWithValue("?") and its corresponding query.
    print(f"Checking {filepath}...")
    q_matches = re.findall(r'Parameters\.Add(WithValue)?\(\s*"(\?)"', content)
    print(f"  Found {len(q_matches)} positional parameter additions.")

for tf in target_files:
    if os.path.exists(tf):
        process_file(tf)
