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

namespace JaegerSoft
{
    public partial class frmCategorias : frmBase
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public frmCategorias()
        {
            InitializeComponent();
        }
        private void frmCategorias_Load(object sender, EventArgs e)
        {
            CargarCategorias();

            EstilizarBotonPrimario(button1);
            EstilizarBotonAdvertencia(button2);
            EstilizarBotonAdvertencia(button3);
            EstilizarDataGridView(dataGridView1);

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
        }

        private void CargarCategorias()
        {
            ds = new DataSet();
            if (conectar.State == ConnectionState.Closed)
                conectar.Open();

            da = new OleDbDataAdapter("SELECT * FROM Categorias ORDER BY Nombre", conectar);
            da.Fill(ds, "Id");

            dataGridView1.DataSource = ds.Tables["Id"];
            if (dataGridView1.Columns.Count > 0)
                dataGridView1.Columns[0].Visible = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarCategoria CAT = new frmAgregarCategoria();
            CAT.ShowDialog();
            this.Close();

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
                if (conectar.State == ConnectionState.Closed)
                    conectar.Open();

                string nombreCat = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                int idCat = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value);

                // 1) Reasignar inventario a SIN CATEGORIA
                cmd = new OleDbCommand(
                    "UPDATE Inventario SET Categoria = ? WHERE Categoria = ?", conectar);
                cmd.Parameters.AddWithValue("@p1", "SIN CATEGORIA");
                cmd.Parameters.AddWithValue("@p2", nombreCat);
                cmd.ExecuteNonQuery();

                // 2) Borrar la categoría
                cmd = new OleDbCommand(
                    "DELETE FROM Categorias WHERE Id = ?", conectar);
                cmd.Parameters.AddWithValue("@p1", idCat);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Se ha eliminado la categoría con éxito", "ELIMINADO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarCategorias();
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
            if (dataGridView1.CurrentRow == null) return;

            frmAgregarCategoria CAT = new frmAgregarCategoria();
            CAT.Editar = true;
            CAT.Id = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value);
            CAT.txtNombre.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            CAT.Nombre = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            CAT.ShowDialog();
            CargarCategorias();
        }
    }
}
