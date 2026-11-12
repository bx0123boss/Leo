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
    public partial class frmCategorias : frmBase
    {
        private DataSet ds;
        //SqlConnection conectar = new SqlConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL);
        SqlDataAdapter da;
        SqlCommand cmd;
        public frmCategorias()
        {
            InitializeComponent();
        }
        private void frmCategorias_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new SqlDataAdapter("select * from Categorias Order By Nombre;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            EstilizarBotonPrimario(button1);
            EstilizarBotonAdvertencia(button2);
            EstilizarBotonAdvertencia(button3);
            EstilizarDataGridView(dataGridView1);
            CargarCategorias();
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
        }

        private void CargarCategorias()
        {
            ds = new DataSet();
            if (conectar.State == ConnectionState.Closed)
                conectar.Open();

            da = new SqlDataAdapter("SELECT * FROM Categorias ORDER BY Nombre", conectar);
            da.Fill(ds, "Id");

            dataGridView1.DataSource = ds.Tables["Id"];
            if (dataGridView1.Columns.Count > 0)
                dataGridView1.Columns[0].Visible = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarCategoria CAT = new frmAgregarCategoria();
            CAT.ShowDialog();
            ds = new DataSet();
            da = new SqlDataAdapter("select * from Categorias Order By Nombre;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            DialogResult dialogResult = MessageBox.Show(
                "¿Estás seguro de eliminar la categoría?",
                "¡Alto!", MessageBoxButtons.YesNo);

            if (dialogResult != DialogResult.Yes) return;

            try
            {
                cmd = new SqlCommand("update Inventario set Categoria = 'SIN CATEGORIA' where Categoria='" + dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString() + "';", conectar);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand("delete from Categorias where Id=" + dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString() + ";", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha eliminado la categoria con exito", "ELIMINADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ds = new DataSet();
                da = new SqlDataAdapter("select * from Categorias Order By Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conectar.State == ConnectionState.Open)
                    conectar.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmAgregarCategoria CAT = new frmAgregarCategoria();
            CAT.Editar = true;
            CAT.Id = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value);
            CAT.txtNombre.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            CAT.Nombre = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            CAT.ShowDialog();
            ds = new DataSet();
            da = new SqlDataAdapter("select * from Categorias Order By Nombre;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;

        }
    }
}
