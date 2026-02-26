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
    public partial class frmPolizas : frmBase
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public String usuario = "";
        public int idProveedor = 0;
        public frmPolizas()
        {
            InitializeComponent();
        }

        private void frmPolizas_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("Select * from Poliza where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);

            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            if (usuario == "Invitado")
            {
                button3.Hide();
            }
            EstilizarDataGridView(dataGridView1);
            EstilizarTextBox(textBox1);
            EstilizarBotonPrimario(button1);
            EstilizarBotonPrimario(button2);
            EstilizarBotonAdvertencia(button3);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarPoliza add = new frmAgregarPoliza();
            add.Show();
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Poliza ORDER BY Folio;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
            }
            else
            {

                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Poliza where Folio LIKE '%" + textBox1.Text + "%' ORDER BY Folio ;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
            }
        }

        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from Poliza where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmDetallesPoliza poli = new frmDetallesPoliza();
            poli.usuario = usuario;
            poli.lblFolio.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.que = 1;
            poli.lblFechaPoli.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.lblFechaRealizada.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.Id = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.lblMonto.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
            double extra =Convert.ToDouble(dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString());
            poli.lblExtra.Text = extra.ToString("#,#.00", CultureInfo.InvariantCulture);
            poli.lblTotal.Text = dataGridView1[9, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.lblIVA.Text = dataGridView1[8, dataGridView1.CurrentRow.Index].Value.ToString();
            poli.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmPolizas2 poli2 = new frmPolizas2();
            poli2.Show();
            this.Close();
        }
    }
}
