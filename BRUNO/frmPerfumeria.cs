using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace BRUNO
{
    public partial class frmPerfumeria : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        OleDbDataAdapter da;
        int piezasPerfume = 0;
        double compraPerfume = 0;
        double ventaPerfume = 0;
        double utilidadPerfume = 0;
        public String usuario = "";

        public frmPerfumeria()
        {
            InitializeComponent();
        }

        private void frmPerfumeria_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Inventario where Categoria='PERFUMERIA' and Existencia > 0 order by Nombre;", conectar);
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
            if (usuario == "Admin")
            {
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    try
                    {
                            piezasPerfume += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                            compraPerfume += Convert.ToDouble(dataGridView2[2, i].Value.ToString()) * Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                            ventaPerfume += Convert.ToDouble(dataGridView2[3, i].Value.ToString()) * Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                            utilidadPerfume += Convert.ToDouble(dataGridView2[8, i].Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("ERROR EN PRODUCTO: " + dataGridView2[1, i].Value.ToString() + "\n" + ex);
                    }
                }

                lblPiezasP.Text = "" + piezasPerfume;
                lblInversionP.Text = "$" + compraPerfume;
                lblVentaP.Text = "$" + ventaPerfume;
                lblUtilidadP.Text = "$" + (ventaPerfume - compraPerfume);
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
                da = new OleDbDataAdapter("select * from Inventario where Categoria='PERFUMERIA' order by Nombre;", conectar);
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
                da = new OleDbDataAdapter("select * from Inventario where Categoria='PERFUMERIA' and Nombre LIKE '%" + textBox1.Text + "%';", conectar);
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='PERFUMERIA' And SubCategoria='" + comboBox2.Text + "' order by Nombre;", conectar);
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
                da = new OleDbDataAdapter("select * from Inventario where Categoria='PERFUMERIA' order by Nombre", conectar);
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
                da = new OleDbDataAdapter("select * from Inventario where Categoria='PERFUMERIA' And SubCategoria='" + comboBox2.Text + "' order by Nombre;", conectar);
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
                da = new OleDbDataAdapter("select * from Inventario where Categoria='PERFUMERIA' order by Nombre", conectar);
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='PERFUMERIA' order by Nombre;", conectar);
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
                da = new OleDbDataAdapter("select * from Inventario where Categoria='PERFUMERIA' and Id LIKE '%" + textBox2.Text + "%';", conectar);
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

        private void button2_Click(object sender, EventArgs e)
        {
            frmEditarProducto editar = new frmEditarProducto();
            editar.usuario = usuario;
            editar.inventario = "PERFUME";
            editar.txtID.Text = dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtProducto.Text = dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtCompra.Text = dataGridView2[2, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtVenta.Text = dataGridView2[3, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtLimite.Text = dataGridView2[5, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.cmbCategoria.Text = dataGridView2[6, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.Show();
            this.Close();
        }

        private void BtnApartados_Click(object sender, EventArgs e)
        {

        }
    }
}
