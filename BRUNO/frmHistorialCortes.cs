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
    public partial class frmHistorialCortes : Form
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;

        public frmHistorialCortes()
        {
            InitializeComponent();
        }

        private void frmHistorialCortes_Load(object sender, EventArgs e)
        {
            //ds = new DataSet();
            conectar.Open();
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from histocortes where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from histocortes where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                frmDetalleCorte detail = new frmDetalleCorte();
                detail.ID = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
                detail.lblMonto.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                detail.lblFecha.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
                detail.mas = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
                detail.menos = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
                detail.tarjeta = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
                detail.Show();
                this.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmCortePorFechas fch = new frmCortePorFechas();
            fch.Show();
            this.Close();
        }
    }
}
