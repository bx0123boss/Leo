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
    public partial class frmUnidades : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public frmUnidades()
        {
            InitializeComponent();
        }

        private void frmUnidades_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Unidades;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarUnidad un = new frmAgregarUnidad();
            un.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmAgregarUnidad un = new frmAgregarUnidad();
            un.id = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            un.txtNombre.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            un.txtAbrev.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
            un.Text = "Editar Unidad";
            un.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de elimiar la unidad de medida?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                cmd = new OleDbCommand("UPDATE Inventario set Uni='NA' where Uni='" + dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString() + "';", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("delete from Unidades where Id=" + dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString() + ";", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha eliminado la unidad de medida con exito", "ELIMINADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Unidades;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }
    }
}
