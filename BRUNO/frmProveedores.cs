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
    public partial class frmProveedores : frmBase
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public frmProveedores()
        {
            InitializeComponent();
        }

        private void frmProveedores_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);

            EstilizarBotonPrimario(this.button1);
            EstilizarBotonPrimario(this.button4);// Botón "Agregar"
            EstilizarBotonPeligro(this.button3);     // Botón "Eliminar"
            EstilizarBotonAdvertencia(this.button2); // Botón "Editar Contraseña"
            EstilizarBotonPrimario(this.button5);

            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Proveedores;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Proveedores ORDER BY Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
            else
            {

                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Proveedores where Nombre LIKE '%" + textBox1.Text + "%' ORDER BY Nombre ;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
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
            catch (Exception ex)
            {
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                frmPolizas add = new frmPolizas();
                add.idProveedor = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
                add.Show();
                this.Close();
            }
            catch(Exception ex)
            {
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                frmAbonoProveedor abono = new frmAbonoProveedor();
                abono.txtAdeudo.Text = dataGridView1[8, dataGridView1.CurrentRow.Index].Value.ToString();
                abono.lblID.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                abono.Show();
                this.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de eliminar el proveedor?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                cmd = new OleDbCommand("delete from proveedores where Id=" + dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString() + ";", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("PROVEEDOR ELIMINADO CON EXITO", "ELIMINADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Proveedores;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }
    }
}
