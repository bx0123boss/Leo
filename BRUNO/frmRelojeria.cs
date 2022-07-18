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
    public partial class frmRelojeria : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        OleDbDataAdapter da;
        int piezasPerfume = 0, piezasRelojes = 0, piezasOro10 = 0, piezasOro14 = 0;
        double compraPerfume = 0, compraRelojes = 0, CompraOro10 = 0, CompraOro14 = 0;
        double ventaPerfume = 0, ventaRelojes = 0, ventaOro10 = 0, ventaOro14 = 0;
        double utilidadPerfume = 0, utilidadRelojes = 0, utilidadOro10 = 0, utilidadOro14 = 0;
        double peso10 = 0, peso14 = 0;
        public String usuario = "";
        public frmRelojeria()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmEditarProducto editar = new frmEditarProducto();
            editar.usuario = usuario;
            editar.inventario = "RELOJ";
            editar.txtID.Text = dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtProducto.Text = dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtCompra.Text = dataGridView2[2, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtVenta.Text = dataGridView2[3, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtLimite.Text = dataGridView2[5, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.cmbCategoria.Text = dataGridView2[6, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de eliminar el producto?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                cmd = new OleDbCommand("delete from Inventario where Id='" + dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString() + "';", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("PRODUCTO ELIMINADO CON EXITO", "ELIMINADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where not Categoria='ACCESORIOS' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
            }
        }

        private void frmRelojeria_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Inventario where Categoria='RELOJERIA' and Existencia > 0 order by Nombre;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
            dataGridView2.DataSource = ds.Tables["Id"];
            dataGridView2.Columns[9].Visible = false;
            dataGridView2.Columns[10].Visible = false;
            dataGridView2.Columns[11].Visible = false;
            dataGridView2.Columns[12].Visible = false;
            dataGridView2.Columns[14].Visible = false;
            dataGridView2.Columns[13].Visible = false;
            dataGridView2.Columns[15].Visible = false;
            dataGridView2.Columns[16].Visible = false;
            if (usuario == "Admin")
            {
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    try
                    {
                       
                            piezasRelojes += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                            compraRelojes += Convert.ToInt32(dataGridView2[4, i].Value.ToString()) * Convert.ToDouble(dataGridView2[2, i].Value.ToString());
                            ventaRelojes += Convert.ToInt32(dataGridView2[4, i].Value.ToString()) * Convert.ToDouble(dataGridView2[3, i].Value.ToString());
                            utilidadRelojes += Convert.ToDouble(dataGridView2[8, i].Value.ToString());
                        
                       
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("ERROR EN PRODUCTO: " + dataGridView2[1, i].Value.ToString() + "\n" + ex);
                    }
                }

               

                lblPiezasR.Text = "" + piezasRelojes;
                lblInversion.Text = "$" + compraRelojes;
                lblVenta.Text = "$" + ventaRelojes;
                lblUtilidad.Text = "$" + (ventaRelojes - compraRelojes);


              
            }
            else
            {
                panel5.Hide();
                panel6.Hide();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='RELOJERIA' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[9].Visible = false;
                dataGridView2.Columns[10].Visible = false;
                dataGridView2.Columns[11].Visible = false;
                dataGridView2.Columns[12].Visible = false;
                dataGridView2.Columns[14].Visible = false;
                dataGridView2.Columns[13].Visible = false;
                dataGridView2.Columns[15].Visible = false;
                dataGridView2.Columns[16].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='RELOJERIA' and Nombre LIKE '%" + textBox1.Text + "%';", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[9].Visible = false;
                dataGridView2.Columns[10].Visible = false;
                dataGridView2.Columns[11].Visible = false;
                dataGridView2.Columns[12].Visible = false;
                dataGridView2.Columns[14].Visible = false;
                dataGridView2.Columns[13].Visible = false;
                dataGridView2.Columns[15].Visible = false;
                dataGridView2.Columns[16].Visible = false;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='RELOJERIA' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[9].Visible = false;
                dataGridView2.Columns[10].Visible = false;
                dataGridView2.Columns[11].Visible = false;
                dataGridView2.Columns[12].Visible = false;
                dataGridView2.Columns[14].Visible = false;
                dataGridView2.Columns[15].Visible = false;
                dataGridView2.Columns[16].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='RELOJERIA' and Id LIKE '%" + textBox2.Text + "%';", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[9].Visible = false;
                dataGridView2.Columns[10].Visible = false;
                dataGridView2.Columns[11].Visible = false;
                dataGridView2.Columns[12].Visible = false;
                dataGridView2.Columns[14].Visible = false;
                dataGridView2.Columns[15].Visible = false;
                dataGridView2.Columns[16].Visible = false;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='RELOJERIA' and SubCategoria='" + comboBox2.Text + "' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[9].Visible = false;
                dataGridView2.Columns[10].Visible = false;
                dataGridView2.Columns[11].Visible = false;
                dataGridView2.Columns[12].Visible = false;
                dataGridView2.Columns[14].Visible = false;
                dataGridView2.Columns[15].Visible = false;
                dataGridView2.Columns[16].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='RELOJERIA' order by Nombre", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[9].Visible = false;
                dataGridView2.Columns[10].Visible = false;
                dataGridView2.Columns[11].Visible = false;
                dataGridView2.Columns[12].Visible = false;
                dataGridView2.Columns[14].Visible = false;
                dataGridView2.Columns[15].Visible = false;
                dataGridView2.Columns[16].Visible = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='RELOJERIA' and SubCategoria='" + comboBox2.Text + "' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[9].Visible = false;
                dataGridView2.Columns[10].Visible = false;
                dataGridView2.Columns[11].Visible = false;
                dataGridView2.Columns[12].Visible = false;
                dataGridView2.Columns[14].Visible = false;
                dataGridView2.Columns[15].Visible = false;
                dataGridView2.Columns[16].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='RELOJERIA' order by Nombre", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[9].Visible = false;
                dataGridView2.Columns[10].Visible = false;
                dataGridView2.Columns[11].Visible = false;
                dataGridView2.Columns[12].Visible = false;
                dataGridView2.Columns[14].Visible = false;
                dataGridView2.Columns[15].Visible = false;
                dataGridView2.Columns[16].Visible = false;
            }
        }
    }
}
