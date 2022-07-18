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
    public partial class frmFisicoDetallado : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public string fisico = "";

        public frmFisicoDetallado()
        {
            InitializeComponent();
        }

        private void frmFisicoDetallado_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from FisicoDetallado where IdFisico='" + fisico + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[6].Visible = false;
        }

        private void frmFisicoDetallado_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmInventariosFisicos inv = new frmInventariosFisicos();
            inv.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                cmd = new OleDbCommand("Update inventario set Existencia='" + dataGridView1[3, i].Value.ToString() + "' where Id='" + dataGridView1[1, i].Value.ToString() + "';", conectar);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("El inventario se restauro con exito", "Exito!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
