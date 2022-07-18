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
    public partial class frmDescuentoPorCategoria : Form
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbCommand cmd;
        OleDbDataAdapter da;

        public frmDescuentoPorCategoria()
        {
            InitializeComponent();
        }

        private void frmDescuentoPorCategoria_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select Id,Nombre,PrecioVenta from Inventario order by Nombre;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
            
            ds = new DataSet();
            da = new OleDbDataAdapter("select * from RespaldoInventario;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void BtnApartados_Click(object sender, EventArgs e)
        {
            if ((textBox2.Text != "") && (comboBox2.Text != ""))
            {
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    double descuento = Convert.ToDouble(dataGridView2[2, i].Value.ToString()) + (Convert.ToDouble(dataGridView2[2, i].Value.ToString()) * (Convert.ToDouble(textBox2.Text) / 100));
                    cmd = new OleDbCommand("UPDATE Inventario set PrecioVenta='" + descuento + "' where Id='" + dataGridView2[0, i].Value.ToString() + "';", conectar);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Se ha actualizado los precios con exito", "Editado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ds = new DataSet();
                da = new OleDbDataAdapter("select Id,Nombre,PrecioVenta from Inventario order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
            }
            else
            {
                MessageBox.Show("Ingrese un valor para hacer el descuento o seleccione una categoria para realizar el descuento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((textBox2.Text != "")&&(comboBox2.Text!=""))
            {
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    double descuento = Convert.ToDouble(dataGridView2[2, i].Value.ToString()) - (Convert.ToDouble(dataGridView2[2, i].Value.ToString()) * (Convert.ToDouble(textBox2.Text) / 100));
                    cmd = new OleDbCommand("UPDATE Inventario set PrecioVenta='" + descuento + "' where Id='" + dataGridView2[0, i].Value.ToString() + "';", conectar);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Se ha actualizado los precios con exito", "Editado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ds = new DataSet();
                da = new OleDbDataAdapter("select Id,Nombre,PrecioVenta from Inventario order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
            }
            else
            {
                MessageBox.Show("Ingrese un valor para hacer el descuento o seleccione una categoria para realizar el descuento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                cmd = new OleDbCommand("UPDATE Inventario set PrecioVenta='" + dataGridView1[1, i].Value.ToString() + "' where Id='" + dataGridView1[0, i].Value.ToString() + "';", conectar);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Se ha restablecido los precios con exito", "Editado", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ds = new DataSet();
            da = new OleDbDataAdapter("select Id,Nombre,PrecioVenta from Inventario order by Nombre;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 4)
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select Id,Nombre,PrecioVenta from Inventario order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
            }
            if (comboBox2.SelectedIndex == 3)
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select Id,Nombre,PrecioVenta from Inventario WHERE SubCategoria='BROQUELES' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select Id,Nombre,PrecioVenta from Inventario where Categoria='" + comboBox2.Text + "';", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cmd = new OleDbCommand("delete * from RespaldoInventario;", conectar);
            cmd.ExecuteNonQuery();
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                cmd = new OleDbCommand("insert into RespaldoInventario values('" + dataGridView2[0, i].Value.ToString() + "','" + dataGridView2[2, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("PRECIOS RESPALDADOS");
        }
    }
}
