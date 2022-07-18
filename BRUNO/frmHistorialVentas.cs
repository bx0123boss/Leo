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
    public partial class frmHistorialVentas : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        public double adeudo = 0;

        public frmHistorialVentas()
        {
            InitializeComponent();
        }

        private void frmHistorialVentas_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Ventas2 where Saldo > 0 and IdCliente=" + lblID.Text + " order by Fecha;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            lblAdeudo.Text = adeudo.ToString("#,#.00", CultureInfo.InvariantCulture);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                frmVentaDetalladaCredito detalles = new frmVentaDetalladaCredito();
                detalles.lblFolio.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.lblFecha.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.button1.Visible = true;
                detalles.id = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.lblMonto.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.usuario = "NO";
                detalles.Show();
                this.Close();
            }
            catch (Exception Ex)
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmAbonoCliente abonar = new frmAbonoCliente();
            abonar.adeudo = Convert.ToDouble(lblAdeudo.Text);
            abonar.lblID.Text = lblID.Text;
            abonar.lblCliente.Text = lblNombre.Text;
            string sal = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
            if (sal == "0" || sal == "")
            {
                abonar.saldo = 0;
            }
            else
            {
                abonar.saldo = Convert.ToDouble(dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString());
                abonar.folio = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
                abonar.Show();
                this.Close();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Ventas2 where IdCliente=" + lblID.Text + " order by Fecha;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[4].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Ventas2 where Saldo > 0 and IdCliente=" + lblID.Text + " order by Fecha;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[4].Visible = false;
            }                        
        }

        private void button4_Click(object sender, EventArgs e)
        {

            frmAbonosFolio histo = new frmAbonosFolio();
            histo.lblFolio.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
            histo.Show();
            this.Close();
        }
    }
}
