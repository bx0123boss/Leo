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
    public partial class frmCompraDetallada : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public String usuario = "";

        public frmCompraDetallada()
        {
            InitializeComponent();
        }

        private void frmCompraDetallada_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from ComprasC where FolioCompra='" + lblFolio.Text + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            if (usuario == "Invitado")
            {
                button1.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAjustes upd = new frmAjustes();
            upd.txtFolio.Text = "A-" + lblFolio.Text;
            upd.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
