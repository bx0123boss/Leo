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
    public partial class frmAgregarDinasti : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        OleDbDataAdapter da;
        public string id;
        public double PrecioOro, Dolar, FactorKilataje, PrecioCompra, PrecioVenta, Gramos, Maquila, PrecioMaquilaDolaresLinea, PrecioPorGramo, ValorLineaGramo, Costo, Doble, Veinte, Utilidad;
        public double costoDolares;
        public frmAgregarDinasti()
        {
            InitializeComponent();
        }

        private void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbSub.DataSource = null;
            cmbSub.Items.Clear();

           if (cmbCategoria.SelectedIndex == 0)
            {

                cmbSub.Items.Add("ARGOLLAS MATRIMONIO");
                cmbSub.Items.Add("ANILLOS DAMA");
                cmbSub.Items.Add("ANILLOS CABALLERO");
                cmbSub.Items.Add("CADENAS");
                cmbSub.Items.Add("ESCLAVAS CABALLERO");
                cmbSub.Items.Add("ESCLAVAS DAMA");
                cmbSub.Items.Add("PULSERAS CABALLERO");
                cmbSub.Items.Add("PULSERAS DAMA");
                cmbSub.Items.Add("ARETES");
                cmbSub.Items.Add("ARRACADAS");
                cmbSub.Items.Add("AROS");
                cmbSub.Items.Add("BROQUELES");
                cmbSub.Items.Add("MEDALLAS");
                cmbSub.Items.Add("DIJES");
                cmbSub.Items.Add("GARGANTILLAS");
                txtPeso.Text = "";
                txtKilataje.Text = "";
                txtPrecioCompraHistorico.Text = "";
                txtPeso.Enabled = true;
                txtKilataje.Enabled = true;
                txtPrecioCompraHistorico.Enabled = true;
            }
        }

        private void frmAgregarDinasti_Load(object sender, EventArgs e)
        {
            conectar.Open();

            DataTable dt = new DataTable();
            cmd = new OleDbCommand("Select * from Linea;", conectar);
            da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            cmbLineas.DisplayMember = "Linea";
            cmbLineas.ValueMember = "Cantidad";
            cmbLineas.DataSource = dt;
            cmbLineas.Text = "";

            cmd = new OleDbCommand("select * from Cambio where Id=1;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Dolar = Convert.ToDouble(reader[2].ToString());
                PrecioOro = Convert.ToDouble(reader[1].ToString());
                lblDolar.Text = "" + Dolar;
                lblOroFino.Text = "" + PrecioOro;
            }
        }

        private void cmbLineas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtPeso.Text == "")
            {
            }
            else
            {
                PrecioMaquilaDolaresLinea = Convert.ToDouble(cmbLineas.SelectedValue.ToString());
                Costo = (Convert.ToDouble(txtPeso.Text) * PrecioMaquilaDolaresLinea);
                costoDolares = Costo / Dolar;
                Doble = Costo * 2;
                Doble = Doble * 1.12;
                Utilidad = Doble - Costo;
                lblCompra.Text = "" + Costo;
                lblVenta.Text = "" + Doble;
                txtPrecioCompraHistorico.Text = "" + Costo;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cmd = new OleDbCommand("insert into inventario values('" + txtID.Text + "','" + txtProducto.Text + "','" + Math.Truncate(Costo) + "','" + Math.Truncate(Doble) + "','" + txtCantidad.Text + "','" + txtLimite.Text + "','" + cmbCategoria.Text + "','" + cmbSub.Text + "','" + Math.Truncate(Utilidad) + "','" + txtPeso.Text + "','" + txtKilataje.Text + "','" + cmbLineas.Text + "','DINASTI','" + Math.Truncate(Costo) + "','" + costoDolares + "','" + Dolar + "','" + PrecioOro + "');", conectar);
            cmd.ExecuteNonQuery();
            MessageBox.Show("EL PRODUCTO HA SIDO AGREGADO CORRECTAMENTE", "DINASTI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtProducto.Focus();
            }
        }
    }
}
