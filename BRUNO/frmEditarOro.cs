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
    public partial class frmEditarOro : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbCommand cmd;
        public string usuario = "";
        double PrecioOro, entre24, Kilataje, Resultado, Maquila, Total, Peso, PrecioCompra, Treinta, por2, Diez;
        public string lugar = "";

        public frmEditarOro()
        {
            InitializeComponent();
            conectar.Open();
            cmd = new OleDbCommand("select * from Cambio where Id=1;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                PrecioOro = Convert.ToDouble(reader[1].ToString());

            }
            DataTable dt = new DataTable();
            cmd = new OleDbCommand("Select Id,Nombre from Proveedores;", conectar);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            cmbProveedor.DisplayMember = "Nombre";
            cmbProveedor.ValueMember = "Id";
            cmbProveedor.DataSource = dt;
            cmbProveedor.Text = "";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtMaquila.Text = "N/A";
                txtMaquila.Enabled = false;
            }
            else
            {
                txtMaquila.Text = "";
                txtMaquila.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        { 
            double utilidad = Convert.ToDouble(txtVenta.Text) - Convert.ToDouble(txtCompra.Text);
            cmd = new OleDbCommand("update Inventario set Nombre='" + txtProducto.Text + "', PrecioCompra=" + txtCompra.Text + ", PrecioVenta=" + txtVenta.Text + ", Limite=" + txtLimite.Text + ", Proveedor='"+cmbProveedor.Text+"', SubCategoria='" + cmbSub.Text + "', Utilidad='" + utilidad + "', Peso='" + txtPeso.Text + "', Kilataje='" + txtKilataje.Text + "', Maquila='" + txtMaquila.Text + "', PrecioCompraHistorico='"+textBox1.Text+"' where Id='" + txtID.Text + "';", conectar);
            cmd.ExecuteNonQuery();
            conectar.Close();
            MessageBox.Show("Se ha actualizado el producto con exito!", "Editar Oro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void frmEditarOro_Load(object sender, EventArgs e)
        {
            
        }

        private void txtMaquila_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                entre24 = PrecioOro / 24;
                Kilataje = Convert.ToDouble(txtKilataje.Text);
                Resultado = entre24 * Kilataje;
                Maquila = Convert.ToDouble(txtMaquila.Text);
                Total = Resultado + Maquila;
                Peso = Convert.ToDouble(txtPeso.Text);
                PrecioCompra = Total * Peso;
                txtCompra.Text =Math.Round(PrecioCompra) + "";
                Treinta = PrecioCompra * 1.3;
                por2 = Treinta * 2;
                Diez = por2 * 1.1;
                txtVenta.Text = Math.Round(Diez) + "";
                //MessageBox.Show(entre24 + "\n" + Kilataje + "\n" + Resultado + "\n" + Maquila + "\n" + Total + "\n" + Peso + "\n" + PrecioCompra + "\n" + Treinta + "\n" + por2 + "\n" + Diez);
            }
        }

        private void frmEditarOro_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (lugar == "INVENT")
            {
                frmInventario invent = new frmInventario();
                invent.usuario = usuario;
                invent.Show();
            }
            else if (lugar == "JOYERIA")
            {
                frmJoyeria invent = new frmJoyeria();
                invent.usuario = usuario;
                invent.Show();
            }
        }
    }
}
