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
    public partial class frmPolizas2 : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;

        public frmPolizas2()
        {
            InitializeComponent();
        }

        private void frmPolizas2_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Poliza2;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Poliza2 ORDER BY Folio;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                //dataGridView1.Columns[0].Visible = false;
            }
            else
            {

                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Poliza2 where Folio LIKE '%" + textBox1.Text + "%' ORDER BY Folio ;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                //dataGridView1.Columns[0].Visible = false;
            }
        }

        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from Poliza2 where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmDetallesPoliza poli = new frmDetallesPoliza();
            poli.label1.Text = "POLIZA CANCELADA EL:";
            poli.button1.Visible = false;
            poli.que = 2;
            poli.Id = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.lblFolio.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.lblFechaPoli.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.lblFechaRealizada.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.lblMonto.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.lblExtra.Text = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.lblTotal.Text = (Convert.ToDouble(poli.lblMonto.Text) + Convert.ToDouble(poli.lblExtra.Text)) + "";
            poli.Show();
            this.Close();
        }
    }
}
