import re

def process_file_exact(filepath, replacements):
    with open(filepath, "r", encoding="utf-8") as f:
        content = f.read()
    
    initial = content
    for old, new in replacements:
        if old not in content:
            print(f"WARNING: Target not found in {filepath}:")
            print("--- OLD TARGET ---")
            print(old[:100] + "...")
        else:
            content = content.replace(old, new)
            
    with open(filepath, "w", encoding="utf-8") as f:
        f.write(content)
    print(f"Done {filepath}, changed: {content != initial}")

# 1. frmAgregarInventario.cs
agregar_inv_rep = [
    (
        'cmdUpdate.Parameters.AddWithValue("?", ParseDouble(txtTotales.Text));',
        'cmdUpdate.Parameters.AddWithValue("@p0", ParseDouble(txtTotales.Text));'
    ),
    (
        'cmdUpdate.Parameters.AddWithValue("?", txtID.Text);',
        'cmdUpdate.Parameters.AddWithValue("@p1", txtID.Text);'
    )
]
process_file_exact(r"BRUNO\frmAgregarInventario.cs", agregar_inv_rep)

# 2. frmEditarProducto.cs
with open(r"BRUNO\frmEditarProducto.cs", "r", encoding="utf-8") as f:
    ep = f.read()

# Replace sql query
ep = ep.replace('WHERE Id = ?;', 'WHERE Id = @Id;')
ep = re.sub(r'Nombre = \?,', 'Nombre = @Nombre,', ep)
ep = re.sub(r'PrecioVenta = \?,', 'PrecioVenta = @PrecioVenta,', ep)
ep = re.sub(r'PrecioVentaMayoreo = \?,', 'PrecioVentaMayoreo = @PrecioVentaMayoreo,', ep)
ep = re.sub(r'Categoria = \?,', 'Categoria = @Categoria,', ep)
ep = re.sub(r'Especial = \?,', 'Especial = @Especial,', ep)
ep = re.sub(r'IVA = \?,', 'IVA = @IVA,', ep)
ep = re.sub(r'Unidad = \?,', 'Unidad = @Unidad,', ep)
ep = re.sub(r'Uni = \?,', 'Uni = @Uni,', ep)
ep = re.sub(r'ProveedorPrincipal = \?,', 'ProveedorPrincipal = @ProveedorPrincipal,', ep)
ep = re.sub(r'StockMaximo = \?,', 'StockMaximo = @StockMaximo,', ep)
ep = re.sub(r'DiasReposicion = \?,', 'DiasReposicion = @DiasReposicion,', ep)
ep = re.sub(r'Limite = \?,', 'Limite = @Limite,', ep)
ep = re.sub(r'ManejaInventario = \?,', 'ManejaInventario = @ManejaInventario,', ep)
ep = re.sub(r'EsInsumo = \?,', 'EsInsumo = @EsInsumo,', ep)
ep = re.sub(r'EsReceta = \? ,', 'EsReceta = @EsReceta ,', ep)
ep = re.sub(r'ImagenUrl = \? ', 'ImagenUrl = @ImagenUrl ', ep)

ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", txtProducto.Text);', 'cmdUpdate.Parameters.AddWithValue("@Nombre", txtProducto.Text);')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", ParseDecimal(txtVenta.Text));', 'cmdUpdate.Parameters.AddWithValue("@PrecioVenta", ParseDecimal(txtVenta.Text));')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", ParseDecimal(txtCompra.Text));', 'cmdUpdate.Parameters.AddWithValue("@PrecioVentaMayoreo", ParseDecimal(txtCompra.Text));')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", comboBox1.Text);', 'cmdUpdate.Parameters.AddWithValue("@Categoria", comboBox1.Text);')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", ParseDouble(txtLimite.Text));', 'cmdUpdate.Parameters.AddWithValue("@Especial", ParseDouble(txtLimite.Text));')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", cmbCategoria.Text);', 'cmdUpdate.Parameters.AddWithValue("@IVA", cmbCategoria.Text);')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", cmbUnidad.SelectedValue != null ? cmbUnidad.SelectedValue.ToString() : "");', 'cmdUpdate.Parameters.AddWithValue("@Unidad", cmbUnidad.SelectedValue != null ? cmbUnidad.SelectedValue.ToString() : "");')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", cmbUnidad.Text);', 'cmdUpdate.Parameters.AddWithValue("@Uni", cmbUnidad.Text);')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", cmbProveedor.Text);', 'cmdUpdate.Parameters.AddWithValue("@ProveedorPrincipal", cmbProveedor.Text);')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", ParseDouble(txtStockMaximo.Text));', 'cmdUpdate.Parameters.AddWithValue("@StockMaximo", ParseDouble(txtStockMaximo.Text));')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", ParseInt(txtDiasReposicion.Text));', 'cmdUpdate.Parameters.AddWithValue("@DiasReposicion", ParseInt(txtDiasReposicion.Text));')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", ParseInt(txtLimiteReal.Text));', 'cmdUpdate.Parameters.AddWithValue("@Limite", ParseInt(txtLimiteReal.Text));')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", chkManejaInventario.Checked);', 'cmdUpdate.Parameters.AddWithValue("@ManejaInventario", chkManejaInventario.Checked);')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", chkEsInsumo.Checked);', 'cmdUpdate.Parameters.AddWithValue("@EsInsumo", chkEsInsumo.Checked);')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", chkEsReceta.Checked);', 'cmdUpdate.Parameters.AddWithValue("@EsReceta", chkEsReceta.Checked);')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", (object)imagenUrl ?? DBNull.Value);', 'cmdUpdate.Parameters.AddWithValue("@ImagenUrl", (object)imagenUrl ?? DBNull.Value);')
ep = ep.replace('cmdUpdate.Parameters.AddWithValue("?", txtID.Text);                          // WHERE Id', 'cmdUpdate.Parameters.AddWithValue("@Id", txtID.Text);')

ep = ep.replace('"values (?, \'PRECIO\', \'MODIFICACION DE PRECIO\', 0, \'0\', ?, ?);"', '"values (@Id, \'PRECIO\', \'MODIFICACION DE PRECIO\', 0, \'0\', @Fecha, @Precio);"')
ep = ep.replace('cmdKardex.Parameters.AddWithValue("?", txtID.Text);', 'cmdKardex.Parameters.AddWithValue("@Id", txtID.Text);')
ep = ep.replace('cmdKardex.Parameters.AddWithValue("?", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());', 'cmdKardex.Parameters.AddWithValue("@Fecha", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());')
ep = ep.replace('cmdKardex.Parameters.AddWithValue("?", txtVenta.Text);', 'cmdKardex.Parameters.AddWithValue("@Precio", txtVenta.Text);')

ep = ep.replace('"SELECT ManejaInventario, EsInsumo, EsReceta, ImagenUrl FROM Inventario WHERE Id = ?"', '"SELECT ManejaInventario, EsInsumo, EsReceta, ImagenUrl FROM Inventario WHERE Id = @Id"')
ep = ep.replace('cmdFlags.Parameters.AddWithValue("?", txtID.Text);', 'cmdFlags.Parameters.AddWithValue("@Id", txtID.Text);')

with open(r"BRUNO\frmEditarProducto.cs", "w", encoding="utf-8") as f:
    f.write(ep)
print("Done frmEditarProducto.cs")
