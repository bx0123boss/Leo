using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmProveedores : frmBase // Hereda de frmBase
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        OleDbCommand cmd;

        public frmProveedores()
        {
            InitializeComponent();
            AplicarEstilos();
        }

        private void AplicarEstilos()
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPrimario(this.button1);    // Agregar
            EstilizarBotonAdvertencia(this.button2); // Editar
            EstilizarBotonPeligro(this.button3);     // Eliminar
            EstilizarBotonPrimario(this.button4);    // Compras
            EstilizarBotonPrimario(this.button5);    // Agregar Abono
            EstilizarTextBox(this.textBox1);         // Buscador
        }

        private void frmProveedores_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                ds = new DataSet();
                if (conectar.State == ConnectionState.Closed) conectar.Open();

                da = new OleDbDataAdapter("select * from Proveedores;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];

                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns[0].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ds = new DataSet();
                if (textBox1.Text == "")
                {
                    da = new OleDbDataAdapter("select * from Proveedores ORDER BY Nombre;", conectar);
                }
                else
                {
                    da = new OleDbDataAdapter("select * from Proveedores where Nombre LIKE '%" + textBox1.Text + "%' ORDER BY Nombre ;", conectar);
                }

                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];

                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns[0].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarProveedor add = new frmAgregarProveedor();
            add.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
                {
                    frmAgregarProveedor prov = new frmAgregarProveedor();
                    prov.Text = "Editar";
                    prov.lblID.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                    prov.txtNombre.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                    prov.txtRFC.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
                    prov.txtDireccion.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
                    prov.txtTelefono.Text = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
                    prov.txtCorreo.Text = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
                    prov.txtReferencia.Text = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
                    prov.textBox1.Text = dataGridView1[7, dataGridView1.CurrentRow.Index].Value.ToString();
                    prov.button1.Text = "Editar";
                    prov.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Seleccione un proveedor para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
                {
                    frmPolizas add = new frmPolizas();
                    add.idProveedor = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
                    add.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Seleccione un proveedor para ver sus compras.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
                {
                    frmAbonoProveedor abono = new frmAbonoProveedor();
                    abono.txtAdeudo.Text = dataGridView1[8, dataGridView1.CurrentRow.Index].Value.ToString();
                    abono.lblID.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                    abono.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Seleccione un proveedor para agregar un abono.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
                {
                    DialogResult dialogResult = MessageBox.Show("¿Estas seguro de eliminar el proveedor seleccionado?", "Alto!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        cmd = new OleDbCommand("delete from proveedores where Id=" + dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString() + ";", conectar);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("PROVEEDOR ELIMINADO CON EXITO", "ELIMINADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarDatos(); // Recargamos la tabla limpiamente
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione un proveedor para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}