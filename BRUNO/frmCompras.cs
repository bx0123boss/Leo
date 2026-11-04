using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmCompras : Form
    {
        private DataSet ds;
        //SqlConnection conectar = new SqlConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL);
        SqlDataAdapter da;
        public String usuario = "";
        public frmCompras()
        {
            InitializeComponent();
        }

        private void frmCompras_Load(object sender, EventArgs e)
        {
            conectar.Open();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new SqlDataAdapter("Select * from Compras where Fecha >='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' and Fecha <='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 23:59:59';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                frmCompraDetallada detalles = new frmCompraDetallada();
                detalles.lblFolio.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.lblFecha.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.usuario = usuario;
                detalles.Show();
                this.Close();

            }
            catch (Exception)
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmAgregarCompras add = new frmAgregarCompras();
            add.Show();
            this.Close();
        }
    }
}
