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
    public partial class frmLineas : Form
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;
        OleDbCommand cmd;

        public frmLineas()
        {
            InitializeComponent();
        }

        private void frmLineas_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Linea;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            //dataGridView1.Columns[0].Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de elimiar la Linea?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                cmd = new OleDbCommand("delete from Linea where Linea='" + dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString() + "';", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha eliminado la Linea con exito", "Lineas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Linea;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarLinea lineas = new frmAgregarLinea();
            lineas.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmAgregarLinea agregar = new frmAgregarLinea();
            agregar.Text = "Editar";
            agregar.line = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.txtLinea.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.txtNumero.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.valor = Convert.ToDouble(dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString());
            agregar.Show();
            this.Close();
        }
    }
}
