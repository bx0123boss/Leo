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
using System.Globalization;

namespace BRUNO
{
    public partial class frmHistorialCompras : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbDataAdapter da;
        public double adeudo = 0;
        public frmHistorialCompras()
        {
            InitializeComponent();
        }

        private void frmHistorialCompras_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from VentasCredito where IdCliente=" + lblID.Text + ";", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[7].Visible = false;
            ds = new DataSet();
            da = new OleDbDataAdapter("select * from VentasContado where IdCliente=" + lblID.Text + ";", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[2].Visible = false;
            dataGridView2.Columns[7].Visible = false;
            dataGridView2.Columns[8].Visible = false;
            lblAdeudo.Text = adeudo.ToString("#,#.00", CultureInfo.InvariantCulture);
        }

        private void frmHistorialCompras_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmClientes cliente = new frmClientes();
            cliente.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
