using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace BRUNO
{
    public partial class frmReporteVentas : Form
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;
        public String usuario = "";
        public frmReporteVentas()
        {
            InitializeComponent();
        }

        private void frmReporteVentas_Load(object sender, EventArgs e)
        {
            //ds = new DataSet();
            conectar.Open();
            //da = new OleDbDataAdapter("select * from Ventas;", conectar);
            //da.Fill(ds, "Id");
            //dataGridView1.DataSource = ds.Tables["Id"];
            //dataGridView1.Columns[0].Visible = false;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from Ventas where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                frmVentaDetallada detalles = new frmVentaDetallada();
                detalles.lblFolio.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.lblFecha.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
                if (dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString() == "CANCELADO")
                {
                    detalles.button1.Visible = false;
                    detalles.button2.Visible = false;
                }
                else
                {
                    detalles.button1.Visible = true;
                    detalles.button2.Visible = true;
                }
                detalles.lblMonto.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.usuario = usuario;
                detalles.Show();
                this.Close();
            }
            catch (Exception ex)
            {

            }

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("Select * from Ventas where Folio='"+textBox1.Text+"';", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }
    }
}
