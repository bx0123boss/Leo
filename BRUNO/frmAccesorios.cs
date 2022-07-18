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
    public partial class frmAccesorios : Form
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        //public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Jaeger Soft\Joyeria.accdb";
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        OleDbDataAdapter da;
        int piezasPlata = 0, piezasBroquel = 0, piezasVarios = 0;
        double compraPlata = 0, compraBroquel = 0, compraVarios = 0;
        double ventaPlata = 0, ventaBroquel = 0, ventaVarios = 0;
        double utilidadPlata = 0, utilidadBroquel = 0, utilidadVarios = 0;
        public String usuario = "";
        public frmAccesorios()
        {
            InitializeComponent();
        }

        private void frmAccesorios_Load(object sender, EventArgs e)
        {
            
                ds = new DataSet();
                conectar.Open();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='ACCESORIOS' and Existencia > 0 order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                if (usuario == "Admin")
                {
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    try
                    {
                        if (dataGridView2[7, i].Value.ToString() == "PLATA")
                        {
                            piezasPlata += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                            compraPlata += Convert.ToInt32(dataGridView2[4, i].Value.ToString()) * Convert.ToDouble(dataGridView2[2, i].Value.ToString());
                            ventaPlata += Convert.ToInt32(dataGridView2[4, i].Value.ToString()) * Convert.ToDouble(dataGridView2[3, i].Value.ToString());
                            utilidadPlata += Convert.ToDouble(dataGridView2[8, i].Value.ToString());
                        }
                        if (dataGridView2[7, i].Value.ToString() == "BROQUELES")
                        {
                            piezasBroquel += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                            compraBroquel += Convert.ToInt32(dataGridView2[4, i].Value.ToString()) * Convert.ToDouble(dataGridView2[2, i].Value.ToString());
                            ventaBroquel += Convert.ToInt32(dataGridView2[4, i].Value.ToString()) * Convert.ToDouble(dataGridView2[3, i].Value.ToString());
                            utilidadBroquel += Convert.ToDouble(dataGridView2[8, i].Value.ToString());
                        }
                        if (dataGridView2[7, i].Value.ToString() == "VARIOS")
                        {
                            piezasVarios += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
                            compraVarios += Convert.ToInt32(dataGridView2[4, i].Value.ToString()) * Convert.ToDouble(dataGridView2[2, i].Value.ToString());
                            ventaVarios += Convert.ToInt32(dataGridView2[4, i].Value.ToString()) * Convert.ToDouble(dataGridView2[3, i].Value.ToString());
                            utilidadVarios += Convert.ToDouble(dataGridView2[8, i].Value.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("lol");
                    }
                }
                lblPiezasP.Text = piezasPlata + "";
                lblInversionP.Text = compraPlata + "";
                lblVentaP.Text = ventaPlata + "";
                lblUtilidadP.Text = (ventaPlata - compraPlata) + "";

                lblPiezasB.Text = piezasBroquel + "";
                lblInversionB.Text = compraBroquel + "";
                lblVentaB.Text = ventaBroquel + "";
                lblUtilidadB.Text = (ventaBroquel - compraBroquel) + "";

                lblPiezasV.Text = piezasVarios + "";
                lblInversionV.Text = compraVarios + "";
                lblVentaV.Text = ventaVarios + "";
                lblUtilidadV.Text = (ventaVarios - compraVarios) + "";
                panel5.Show();
                button1.Visible = false;
                button2.Visible = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='ACCESORIOS' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Nombre LIKE '%" + textBox1.Text + "%' and Categoria='ACCESORIOS';", conectar);
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
                da = new OleDbDataAdapter("select * from Inventario where Categoria='ACCESORIOS' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Id LIKE '%" + textBox2.Text + "%' and Categoria='ACCESORIOS';", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmEditarAccesorios editar = new frmEditarAccesorios();
            editar.txtID.Text = dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtProducto.Text = dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtCompra.Text = dataGridView2[2, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtVenta.Text = dataGridView2[3, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.txtLimite.Text = dataGridView2[5, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.cmbCategoria.Text = dataGridView2[6, dataGridView2.CurrentRow.Index].Value.ToString();
            editar.cmbSub.Text = dataGridView2[7, dataGridView2.CurrentRow.Index].Value.ToString();
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
                da = new OleDbDataAdapter("select * from Inventario where Categoria='ACCESORIOS' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
            }
        }
    }
}
