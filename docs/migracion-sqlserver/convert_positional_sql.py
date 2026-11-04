import re
import os

def convert_query_and_params(content):
    # Pattern 1:
    # A function that replaces '?' in strings and matching Parameters.AddWithValue("?", ...)
    pass

# We will handle the 6 files with targeted replacements:

# 1. frmAgregarInventario.cs
def fix_agregar_inventario():
    path = r"BRUNO\frmAgregarInventario.cs"
    with open(path, "r", encoding="utf-8") as f:
        c = f.read()

    # Query 1
    c = c.replace(
        'select count(*) from [" + lblOrigen.Text + "] where Id = ?;',
        'select count(*) from [" + lblOrigen.Text + "] where Id = @p0;'
    )
    c = c.replace('cmdCheck.Parameters.AddWithValue("?", txtID.Text);', 'cmdCheck.Parameters.AddWithValue("@p0", txtID.Text);')

    # Query 2
    c = c.replace(
        'select count(*) from InventarioSusp where Id = ?;',
        'select count(*) from InventarioSusp where Id = @p0;'
    )
    c = c.replace('cmdCheckSusp.Parameters.AddWithValue("?", txtID.Text);', 'cmdCheckSusp.Parameters.AddWithValue("@p0", txtID.Text);')

    # Query 3
    c = c.replace(
        'UPDATE [" + lblOrigen.Text + "] set Existencia = ? Where Id = ?;',
        'UPDATE [" + lblOrigen.Text + "] set Existencia = @p0 Where Id = @p1;'
    )
    c = c.replace(
        'cmdUpdate.Parameters.AddWithValue("?", ParseDouble(txtTotales.Text));\r\n                    cmdUpdate.Parameters.AddWithValue("?", txtID.Text);',
        'cmdUpdate.Parameters.AddWithValue("@p0", ParseDouble(txtTotales.Text));\r\n                    cmdUpdate.Parameters.AddWithValue("@p1", txtID.Text);'
    )
    c = c.replace(
        'cmdUpdate.Parameters.AddWithValue("?", ParseDouble(txtTotales.Text));\n                    cmdUpdate.Parameters.AddWithValue("?", txtID.Text);',
        'cmdUpdate.Parameters.AddWithValue("@p0", ParseDouble(txtTotales.Text));\n                    cmdUpdate.Parameters.AddWithValue("@p1", txtID.Text);'
    )

    # Query 4: Kardex entrada
    c = c.replace(
        'insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values (?, \'ENTRADA\', \'ENTRADA DE ARTICULO\', ?, ?, ?);',
        'insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values (@p0, \'ENTRADA\', \'ENTRADA DE ARTICULO\', @p1, @p2, @p3);'
    )
    old_kardex_p = """cmdKardex.Parameters.AddWithValue("?", txtID.Text);
                        cmdKardex.Parameters.AddWithValue("?", ParseDouble(txtActuales.Text));
                        cmdKardex.Parameters.AddWithValue("?", ParseDouble(txtTotales.Text));
                        cmdKardex.Parameters.AddWithValue("?", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());"""
    new_kardex_p = """cmdKardex.Parameters.AddWithValue("@p0", txtID.Text);
                        cmdKardex.Parameters.AddWithValue("@p1", ParseDouble(txtActuales.Text));
                        cmdKardex.Parameters.AddWithValue("@p2", ParseDouble(txtTotales.Text));
                        cmdKardex.Parameters.AddWithValue("@p3", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());"""
    c = c.replace(old_kardex_p.replace("\n", "\r\n"), new_kardex_p.replace("\n", "\r\n"))
    c = c.replace(old_kardex_p, new_kardex_p)

    # Query 5: Insert inventario
    c = c.replace(
        '"values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);"',
        '"values (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17);"'
    )
    insert_params = [
        ('cmdInsert.Parameters.AddWithValue("?", txtID.Text);', 'cmdInsert.Parameters.AddWithValue("@p0", txtID.Text);'),
        ('cmdInsert.Parameters.AddWithValue("?", txtProducto.Text);', 'cmdInsert.Parameters.AddWithValue("@p1", txtProducto.Text);'),
        ('cmdInsert.Parameters.AddWithValue("?", ParseDecimal(txtCompra.Text));', 'cmdInsert.Parameters.AddWithValue("@p2", ParseDecimal(txtCompra.Text));'),
        ('cmdInsert.Parameters.AddWithValue("?", ParseDecimal(txtVenta.Text));', 'cmdInsert.Parameters.AddWithValue("@p3", ParseDecimal(txtVenta.Text));'),
        ('cmdInsert.Parameters.AddWithValue("?", ParseDouble(txtCantidad.Text));', 'cmdInsert.Parameters.AddWithValue("@p4", ParseDouble(txtCantidad.Text));'),
        ('cmdInsert.Parameters.AddWithValue("?", ParseInt(txtLimite.Text));', 'cmdInsert.Parameters.AddWithValue("@p5", ParseInt(txtLimite.Text));'),
        ('cmdInsert.Parameters.AddWithValue("?", cmbCategoria.Text);', 'cmdInsert.Parameters.AddWithValue("@p6", cmbCategoria.Text);'),
        ('cmdInsert.Parameters.AddWithValue("?", ParseDouble(txtEspecial.Text));', 'cmdInsert.Parameters.AddWithValue("@p7", ParseDouble(txtEspecial.Text));'),
        ('cmdInsert.Parameters.AddWithValue("?", comboBox1.Text);', 'cmdInsert.Parameters.AddWithValue("@p8", comboBox1.Text);'),
        ('cmdInsert.Parameters.AddWithValue("?", cmbUnidad.SelectedValue != null ? cmbUnidad.SelectedValue.ToString() : "");', 'cmdInsert.Parameters.AddWithValue("@p9", cmbUnidad.SelectedValue != null ? cmbUnidad.SelectedValue.ToString() : "");'),
        ('cmdInsert.Parameters.AddWithValue("?", cmbUnidad.Text);', 'cmdInsert.Parameters.AddWithValue("@p10", cmbUnidad.Text);'),
        ('cmdInsert.Parameters.AddWithValue("?", cmbProveedor.Text);', 'cmdInsert.Parameters.AddWithValue("@p11", cmbProveedor.Text);'),
        ('cmdInsert.Parameters.AddWithValue("?", ParseDouble(txtStockMaximo.Text));', 'cmdInsert.Parameters.AddWithValue("@p12", ParseDouble(txtStockMaximo.Text));'),
        ('cmdInsert.Parameters.AddWithValue("?", ParseInt(txtDiasReposicion.Text));', 'cmdInsert.Parameters.AddWithValue("@p13", ParseInt(txtDiasReposicion.Text));'),
        ('cmdInsert.Parameters.AddWithValue("?", chkManejaInventario.Checked);', 'cmdInsert.Parameters.AddWithValue("@p14", chkManejaInventario.Checked);'),
        ('cmdInsert.Parameters.AddWithValue("?", ckbInsumo.Checked);', 'cmdInsert.Parameters.AddWithValue("@p15", ckbInsumo.Checked);'),
        ('cmdInsert.Parameters.AddWithValue("?", ckbReceta.Checked);', 'cmdInsert.Parameters.AddWithValue("@p16", ckbReceta.Checked);'),
        ('cmdInsert.Parameters.AddWithValue("?", (object)imagenUrl ?? DBNull.Value);', 'cmdInsert.Parameters.AddWithValue("@p17", (object)imagenUrl ?? DBNull.Value);'),
    ]
    for old_p, new_p in insert_params:
        c = c.replace(old_p, new_p)

    # Query 6: Kardex creacion
    c = c.replace(
        'insert into Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) values (?, \'ENTRADA\', \'CREACION DE ARTICULO\', 0, ?, ?);',
        'insert into Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) values (@p0, \'ENTRADA\', \'CREACION DE ARTICULO\', 0, @p1, @p2);'
    )
    old_kardex2 = """cmdKardex.Parameters.AddWithValue("?", txtID.Text);
                    cmdKardex.Parameters.AddWithValue("?", ParseDouble(txtCantidad.Text));
                    cmdKardex.Parameters.AddWithValue("?", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());"""
    new_kardex2 = """cmdKardex.Parameters.AddWithValue("@p0", txtID.Text);
                    cmdKardex.Parameters.AddWithValue("@p1", ParseDouble(txtCantidad.Text));
                    cmdKardex.Parameters.AddWithValue("@p2", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());"""
    c = c.replace(old_kardex2.replace("\n", "\r\n"), new_kardex2.replace("\n", "\r\n"))
    c = c.replace(old_kardex2, new_kardex2)

    with open(path, "w", encoding="utf-8") as f:
        f.write(c)
    print("frmAgregarInventario.cs updated.")

fix_agregar_inventario()
