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
    public partial class frmEditarProducto : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        public string usuario = "";
        public string inventario = "";
        double precio;
        public frmEditarProducto()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cmd = new OleDbCommand("update Inventario set Nombre='" + txtProducto.Text + "', PrecioVenta=" + txtVenta.Text + ",PrecioVentaMayoreo=" + txtCompra.Text + ", Categoria='"+comboBox1.Text+"', Especial=" + txtLimite.Text + ", IVA='" + cmbCategoria.Text + "', Unidad='"+cmbUnidad.SelectedValue.ToString()+"', Uni='"+cmbUnidad.Text+"' where Id='" + txtID.Text + "';", conectar);
            cmd.ExecuteNonQuery();
            if (precio == Convert.ToDouble(txtVenta.Text))
            {

            }
            else
            {
                cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha,Precio) values('" +txtID.Text + "','PRECIO','MODIFICACION DE PRECIO',0,'0','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','"+txtVenta.Text+"');", conectar);
                cmd.ExecuteNonQuery();
            }
            conectar.Close();
            MessageBox.Show("Se ha actualizado el producto con exito!", "Editar Oro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void frmEditarProducto_Load(object sender, EventArgs e)
        {
            string categoria = comboBox1.Text;
            conectar.Open();
            DataTable dt = new DataTable();
            cmd = new OleDbCommand("Select Id,Nombre from Categorias;", conectar);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            comboBox1.DisplayMember = "Nombre";
            comboBox1.ValueMember = "Id";
            comboBox1.DataSource = dt;
            comboBox1.Text = categoria;
            precio = Convert.ToDouble(txtVenta.Text);
        }

        private void frmEditarProducto_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (inventario == "PERFUME")
            {
                frmPerfumeria invent = new frmPerfumeria();
                invent.usuario = usuario;
                invent.Show();
            }
            else if (inventario == "INVENT")
            {
                frmInventario invent = new frmInventario();
                invent.usuario = usuario;
                invent.Show();
            }
            else if (inventario == "RELOJ")
            {
                frmRelojeria invent = new frmRelojeria();
                invent.usuario = usuario;
                invent.Show();
            }

        }

        private void txtCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void cmbSub_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txtLimite_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtVenta_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCompra_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtProducto_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtLimite_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
