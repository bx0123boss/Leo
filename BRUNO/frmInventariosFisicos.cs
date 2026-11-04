using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmInventariosFisicos : Form
    {
        private DataSet ds;
        //SqlConnection conectar = new SqlConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL);
        SqlDataAdapter da;
        public String usuario = "";
        public frmInventariosFisicos()
        {
            InitializeComponent();
        }

        private void frmInventariosFisicos_Load(object sender, EventArgs e)
        {
            conectar.Open();
            ds = new DataSet();
            da = new SqlDataAdapter("Select * from Fisico where Fecha >='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' and Fecha <='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 23:59:59';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new SqlDataAdapter("Select * from Fisico where Fecha >='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' and Fecha <='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 23:59:59';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
                if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
                {
                    frmFisicoDetallado detalles = new frmFisicoDetallado();
                    detalles.fisico = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                    detalles.lblFecha.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                    detalles.Show();
                    this.Close();
                }
            else
            {
                MessageBox.Show("Por favor, seleccione un registro de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmFisico fis = new frmFisico();
            fis.Show();
            this.Close();
        }
    }
}
