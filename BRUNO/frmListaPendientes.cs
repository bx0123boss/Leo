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
    public partial class frmListaPendientes : Form
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public frmListaPendientes()
        {
            InitializeComponent();
        }

        private void frmListaPendientes_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from VentasApartados where Estatus='PENDIENTE';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[8].Visible = false;
        }

        private void frmListaPendientes_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmApartados part = new frmApartados();
            part.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
            cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString() + "';", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                double existencia = Convert.ToDouble(Convert.ToString(reader[4].ToString())) - Convert.ToDouble(dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString());
                if (existencia >= 0)
                {
                    cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existencia + "' where Id='" + dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString() + "';", conectar);
                    cmd.ExecuteNonQuery();
                    cmd = new OleDbCommand("UPDATE VentasApartados set Estatus='DESCONTADO DE INVENTARIO' where Id=" + dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString() + ";", conectar);
                    cmd.ExecuteNonQuery();
                    cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString() + "','SALIDA','APARTADO DE ARTICULO FOLIO: " + dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString() + "'," + Convert.ToString(reader[4].ToString()) + ",'" + existencia + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("PRODUCTO DESCONTADO DE INVENTARIO", "DESCONTADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from VentasApartados where Estatus='PENDIENTE';", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    dataGridView1.Columns[0].Visible = false;
                    dataGridView1.Columns[2].Visible = false;
                    dataGridView1.Columns[5].Visible = false;
                    dataGridView1.Columns[6].Visible = false;
                    dataGridView1.Columns[8].Visible = false;
                }
                else
                {
                    MessageBox.Show("El producto no tiene las exitencias necesarias para descontar este producto, verifique sus inventarios", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            
            }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
