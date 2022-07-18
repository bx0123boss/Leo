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
    public partial class frmSuspendidos : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        OleDbDataAdapter da;
        public String usuario = "Admin";

        public frmSuspendidos()
        {
            InitializeComponent();
        }

        private void frmSuspendidos_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("select * from InventarioSusp order by Nombre;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
            dataGridView2.Columns[7].HeaderText = "Precio Compra";
            dataGridView2.Columns[10].HeaderText = "Unidad";
            dataGridView2.Columns[9].Visible = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {

                DialogResult dialogResult = MessageBox.Show("¿Estas seguro de reactivar el producto?", "Alto!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    conectar.Open();
                    cmd = new OleDbCommand("insert into Inventario(Id,Nombre,PrecioVentaMayoreo,PrecioVenta,Existencia,Limite,Categoria,Especial,IVA,Unidad,Uni) values('" + dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[2, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[3, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[4, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[5, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[6, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[7, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[8, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[9, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[10, dataGridView2.CurrentRow.Index].Value.ToString() + "');", conectar);
                    cmd.ExecuteNonQuery();
                    cmd = new OleDbCommand("delete from InventarioSusp where Id='" + dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString() + "';", conectar);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("PRODUCTO REACTIVADO CON EXITO", "ELIMINADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from InventarioSusp order by Nombre;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox1.Text == "")
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from InventarioSusp order by Nombre;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                }
                else
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from InventarioSusp where Nombre LIKE '%" + textBox1.Text + "%';", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox2.Text == "")
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from InventarioSusp order by Nombre;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                }
                else
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from InventarioSusp where Id LIKE '%" + textBox2.Text + "%';", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                }
            }
        }
    }
}
