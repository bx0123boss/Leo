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
    public partial class frmJoyeria : Form
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        //public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Jaeger Soft\Joyeria.accdb";
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        OleDbDataAdapter da;
        int piezasPerfume = 0, piezasRelojes = 0, piezasOro10 = 0, piezasOro14 = 0;
        double compraPerfume = 0, compraRelojes = 0, CompraOro10 = 0, CompraOro14 = 0;
        double ventaPerfume = 0, ventaRelojes = 0, ventaOro10 = 0, ventaOro14 = 0;
        double utilidadPerfume = 0, utilidadRelojes = 0, utilidadOro10 = 0, utilidadOro14 = 0;
        double peso10 = 0, peso14 = 0;
        public String usuario = "";

        public frmJoyeria()
        {
            InitializeComponent();
        }

        private void frmJoyeria_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Inventario where Categoria='JOYERIA' and Existencia > 0 order by Nombre;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
            if (usuario == "Admin")
            {
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    try
                    {
                       
                        
                            if (dataGridView2[10, i].Value.ToString() == "10")
                            {
                                piezasOro10 += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                                CompraOro10 += Convert.ToDouble(dataGridView2[13, i].Value.ToString()) * Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                                ventaOro10 += Convert.ToDouble(dataGridView2[3, i].Value.ToString()) * Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                                utilidadOro10 += Convert.ToDouble(dataGridView2[8, i].Value.ToString());
                                peso10 += Convert.ToDouble(dataGridView2[9, i].Value.ToString()) * Convert.ToInt32(dataGridView2[4, i].Value.ToString());

                            }
                            else if (dataGridView2[10, i].Value.ToString() == "14")
                            {
                                piezasOro14 += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                                CompraOro14 += Convert.ToDouble(dataGridView2[13, i].Value.ToString()) * Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                                ventaOro14 += Convert.ToDouble(dataGridView2[3, i].Value.ToString()) * Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                                utilidadOro14 += Convert.ToDouble(dataGridView2[8, i].Value.ToString());
                                peso14 += Convert.ToDouble(dataGridView2[9, i].Value.ToString()) * Convert.ToInt32(dataGridView2[4, i].Value.ToString());

                            }
                        
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("ERROR EN PRODUCTO: " + dataGridView2[1, i].Value.ToString() + "\n" + ex);
                    }
                }

                lblPiezas10.Text = "" + piezasOro10;
                lblInversion10.Text = "$" + CompraOro10;
                lblVenta10.Text = "$" + ventaOro10;
                lblUtilidad10.Text = "$" + (ventaOro10 - CompraOro10);
                lblPeso10.Text = "" + peso10;

                lblPiezas14.Text = "" + piezasOro14;
                lblInversion14.Text = "$" + CompraOro14;
                lblVenta14.Text = "$" + ventaOro14;
                lblUtilidad14.Text = "$" + (ventaOro14 - CompraOro14);
                lblPeso14.Text = "" + peso14;

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
                da = new OleDbDataAdapter("select * from Inventario where Categoria='JOYERIA' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='JOYERIA' and Nombre LIKE '%" + textBox1.Text + "%';", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='JOYERIA' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='JOYERIA' and Id LIKE '%" + textBox2.Text + "%';", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='JOYERIA' AND subCategoria='" + comboBox2.Text + "' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='JOYERIA' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='JOYERIA' AND subCategoria='" + comboBox2.Text + "' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='JOYERIA' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
                frmEditarOro oro = new frmEditarOro();
                oro.usuario = usuario;
                oro.lugar = "JOYERIA";
                oro.txtID.Text = dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.txtProducto.Text = dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.txtCompra.Text = dataGridView2[2, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.txtVenta.Text = dataGridView2[3, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.txtLimite.Text = dataGridView2[5, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.cmbCategoria.Text = dataGridView2[6, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.cmbSub.Text = dataGridView2[7, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.cmbProveedor.Text = dataGridView2[12, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.txtPeso.Text = dataGridView2[9, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.txtKilataje.Text = dataGridView2[10, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.txtMaquila.Text = dataGridView2[11, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.textBox1.Text = dataGridView2[13, dataGridView2.CurrentRow.Index].Value.ToString();
                oro.Show();
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
    }
}
