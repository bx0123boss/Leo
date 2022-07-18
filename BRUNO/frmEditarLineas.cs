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
    public partial class frmEditarLineas : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        OleDbDataAdapter da;
        public string id,lain;
        public double PrecioOro, Dolar, FactorKilataje, PrecioCompra,PrecioVenta, Gramos, Maquila, PrecioMaquilaDolaresLinea, PrecioPorGramo, ValorLineaGramo, Costo, Doble, Veinte,Utilidad;

        public frmEditarLineas()
        {
            InitializeComponent();
        }

        private void frmEditarLineas_Load(object sender, EventArgs e)
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
            if (lain != "")
            {
                cmbLineas.Text = lain;
            }

            cmd = new OleDbCommand("select * from Cambio where Id=1;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Dolar = Convert.ToDouble(reader[2].ToString());
                PrecioOro = Convert.ToDouble(reader[1].ToString());
                lblDolar.Text = "" + Dolar;
                lblOroFino.Text = "" + PrecioOro;
            }
            lblPrecioCompra.Text = "$" + PrecioCompra;
            lblGramos.Text = "" + Gramos;
            if (lblKilate.Text == "10")
            {
                FactorKilataje = 0.440;
            }
            else if (lblKilate.Text == "14")
            {
                FactorKilataje = 0.616;
            }
            else if (lblKilate.Text == "18")
            {
                FactorKilataje = 0.8;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double dolares = Costo / Dolar;
            cmd = new OleDbCommand("UPDATE Inventario Set PrecioCompra='" + Math.Truncate(Costo) + "', PrecioVenta='" + Math.Truncate(Veinte) + "', Maquila='" + cmbLineas.Text + "', Utilidad='" + Math.Truncate(Utilidad) + "', PrecioCompraDolares='" + dolares + "', DolarInicial='" + txtDolar.Text + "', OroInicial='"+txtOro.Text+"' where Id='" + id + "';", conectar);
            cmd.ExecuteNonQuery();
            MessageBox.Show("EL PRECIO HA SIDO ACTUALIZADO", "DINASTI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();



        }

        private void frmEditarLineas_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmDinasti dinasti = new frmDinasti();
            dinasti.Show();

        }

        private void cmbLineas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lblDolar.Text == "0")
            {

            }
            else
            {
                PrecioMaquilaDolaresLinea = Convert.ToDouble(cmbLineas.SelectedValue.ToString());
                Costo = (Gramos * PrecioMaquilaDolaresLinea);
                Doble = (Costo * 2);
                Veinte = Math.Truncate(Doble * 1.12);
                lblPrecioCompra.Text = "$" + Math.Truncate(Veinte);
                lblCompra.Text = "$" + Math.Truncate(Costo);
                Utilidad = Doble - Costo;
            }
        }
    }
}
