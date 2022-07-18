using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmAgregarCompras : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        public int folioExp;
        public frmAgregarCompras()
        {
            InitializeComponent();
        }

        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                using (frmBuscarProductos buscar = new frmBuscarProductos())
                {
                    buscar.textBox1.Text = txtId.Text;
                    buscar.compras = true;
                    if (buscar.ShowDialog() == DialogResult.OK)
                    {
                        double existenciaActual = Convert.ToDouble(txtCantidad.Text) + Convert.ToDouble(buscar.existencia);
                        dataGridView1.Rows.Add(txtCantidad.Text, buscar.producto, buscar.existencia, buscar.ID, buscar.existencia, existenciaActual);                        
                    }
                }
                txtId.Text = "";
                txtCantidad.Clear();
                txtCantidad.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarInventario add = new frmAgregarInventario();
            add.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conectar.Open();
            cmd = new OleDbCommand("select Id from Compras ORDER BY Id DESC;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                folioExp = Convert.ToInt32(reader[0].ToString());
                folioExp++;
                txtFolio.Text = folioExp + "";
            }
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + dataGridView1[5, i].Value.ToString() + "' Where Id='" + dataGridView1[3, i].Value.ToString() + "';", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[3, i].Value.ToString() + "','ENTRADA','COMPRA DE ARTICULO FOLIO: " + txtFolio.Text + "'," + dataGridView1[4, i].Value.ToString() + ",'" + dataGridView1[5, i].Value.ToString() + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("insert into ComprasC(FolioCompra,IdProducto,CantidadAnterior,CantidadActual,Producto,Fecha) values('" + txtFolio.Text + "','" + dataGridView1[3, i].Value.ToString() + "','" + dataGridView1[4, i].Value.ToString() + "','" + dataGridView1[5, i].Value.ToString() + "','" + dataGridView1[1, i].Value.ToString() + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            cmd = new OleDbCommand("insert into Compras(Fecha,Folio,Tipo) values('" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + txtFolio.Text + "','COMPRA');", conectar);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Compra realizada con exito", "COMPRA REALIZADA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();            
            frmCompras vent = new frmCompras();
            vent.Show();
        }

        private void txtCantidad_Leave(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToDouble(txtCantidad.Text) > 0)
                {

                }
                else
                {
                    MessageBox.Show("No puede ingresar valores negativos, favor de verificar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCantidad.Clear();
                    txtCantidad.Focus();
                }
            }
            catch (Exception ex)
            { }
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
