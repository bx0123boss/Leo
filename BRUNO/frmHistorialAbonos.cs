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
    public partial class frmHistorialAbonos : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbDataAdapter da;

        public frmHistorialAbonos()
        {
            InitializeComponent();
        }

        private void frmHistorialAbonos_Load(object sender, EventArgs e)
        {
            conectar.Open();
            ds = new DataSet();
            da = new OleDbDataAdapter("select * from Abonos where IdCliente='" + lblID.Text + "' Order By Fecha;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[2].Visible = false;

            ds = new DataSet();
            da = new OleDbDataAdapter("select * from AbonosApartado where IdCliente='" + lblID.Text + "' Order By Fecha;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[2].Visible = false;
        }

        private void frmHistorialAbonos_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmClientesCredito cliente = new frmClientesCredito();
            cliente.Show();
        }
    }
}
