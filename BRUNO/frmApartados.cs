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
    public partial class frmApartados : Form
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;
        public String usuario = "";

        public frmApartados()
        {
            InitializeComponent();
        }

        private void frmApartados_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Apartados where Restante>0;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            double total = 0;
            if (usuario == "Admin")
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    total += Convert.ToDouble(dataGridView1[7, i].Value.ToString());
                }
                lblRestante.Text = "$" + total;
                label4.Visible = true;
                lblRestante.Visible = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Apartados where Restante>0;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[4].Visible = false;
            }
            else
            {

                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Apartados where Folio LIKE '%" + textBox1.Text + "%';", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[4].Visible = false;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Apartados where Restante>0;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[4].Visible = false;
            }
            else
            {

                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Apartados where NombreCliente LIKE '%" + textBox2.Text + "%';", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[4].Visible = false;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from Apartados where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[4].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
            frmAbonoApartado abono = new frmAbonoApartado();
            abono.txtAdeudo.Text = dataGridView1[7, dataGridView1.CurrentRow.Index].Value.ToString();
            abono.idCliente =Convert.ToInt32(dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString());
            abono.lblID.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            abono.abonado = Convert.ToDouble(dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString());
            abono.lblFolio.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
            abono.Show();
            this.Close();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                frmApartadoDetallado detalles = new frmApartadoDetallado();
                detalles.lblFolio.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.lblFecha.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.lblMonto.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.lblAbonado.Text = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.lblRestante.Text = dataGridView1[7, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.usuario = usuario;
                detalles.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmListaPendientes lista = new frmListaPendientes();
            lista.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
