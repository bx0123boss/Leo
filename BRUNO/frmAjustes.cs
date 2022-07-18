using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmAjustes : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        public frmAjustes()
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

        private void button2_Click(object sender, EventArgs e)
        {
            conectar.Open();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + dataGridView1[5, i].Value.ToString() + "' Where Id='" + dataGridView1[3, i].Value.ToString() + "';", conectar);
                cmd.ExecuteNonQuery();
                if(Convert.ToDouble(dataGridView1[0,i].Value.ToString())<0)
                    cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[3, i].Value.ToString() + "','SALIDA','AJUSTE DE ARTICULO FOLIO: " + txtFolio.Text + "'," + dataGridView1[4, i].Value.ToString() + ",'" + dataGridView1[5, i].Value.ToString() + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                else
                    cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[3, i].Value.ToString() + "','ENTRADA','AJUSTE DE ARTICULO FOLIO: " + txtFolio.Text + "'," + dataGridView1[4, i].Value.ToString() + ",'" + dataGridView1[5, i].Value.ToString() + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("insert into ComprasC(FolioCompra,IdProducto,CantidadAnterior,CantidadActual,Producto,Fecha) values('" + txtFolio.Text + "','" + dataGridView1[3, i].Value.ToString() + "','" + dataGridView1[4, i].Value.ToString() + "','" + dataGridView1[5, i].Value.ToString() + "','" + dataGridView1[1, i].Value.ToString() + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            cmd = new OleDbCommand("insert into Compras(Fecha,Folio,Tipo) values('" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + txtFolio.Text + "','AJUSTE');", conectar);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Ajuste realizada con exito", "AJUSTE REALIZADA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Hide();
            //this.Close();
            frmCompras vent = new frmCompras();
            vent.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarInventario add = new frmAgregarInventario();
            add.Show();
        }
    }
}
