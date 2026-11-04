import re
import os

def clean_file(path):
    with open(path, "r", encoding="utf-8") as f:
        text = f.read()

    # Split into lines
    lines = text.splitlines(keepends=True)
    new_lines = []
    
    param_idx = 0
    in_command = False
    
    for line in lines:
        # Check if line defines a query with '?' or instantiates SqlCommand
        # If line contains 'Parameters.AddWithValue("?"' or 'Parameters.Add("?"'
        if re.search(r'Parameters\.Add(WithValue)?\(\s*"\?"', line):
            line = re.sub(r'Parameters\.Add(WithValue)?\(\s*"\?"', f'Parameters.Add\\1("@p{param_idx}"', line)
            param_idx += 1
        elif 'SqlCommand' in line or 'string query' in line or 'string sql' in line:
            # If line has '?', replace each '?' with @p0, @p1, ...
            if '?' in line:
                # We need to replace only '?' that are SQL parameter placeholders inside strings
                # Count '?'
                def repl_q(m):
                    nonlocal param_idx
                    res = f'@p{param_idx}'
                    param_idx += 1
                    return res
                # Reset param_idx for a new command definition if it has ?
                param_idx = 0
                # Replace '?' inside string literals
                # Simple replacement for '?' when part of SQL
                line = re.sub(r'\?', repl_q, line)
                param_idx = 0 # reset so Parameters.Add will start at @p0
            else:
                param_idx = 0
        new_lines.append(line)
        
    return "".join(new_lines)

# But wait, in some files `string sql = ...` spans multiple lines!
# Let's write dedicated handler for the 6 files.
