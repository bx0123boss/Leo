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
    public partial class frmDinasti : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        OleDbDataAdapter da;
        double PrecioOro, Dolar;
        public frmDinasti()
        {
            InitializeComponent();
        }

        private void frmDinasti_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Inventario where Proveedor='DINASTI' and Existencia > 0;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
            dataGridView2.Columns[5].Visible = false;
            dataGridView2.Columns[6].Visible = false;
            dataGridView2.Columns[7].Visible = false;
            dataGridView2.Columns[8].Visible = false;
            dataGridView2.Columns[12].Visible = false;

            

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmEditarLineas lineas = new frmEditarLineas();
            lineas.PrecioCompra = Convert.ToDouble(dataGridView2[2, dataGridView2.CurrentRow.Index].Value.ToString());
            lineas.Gramos = Convert.ToDouble(dataGridView2[9, dataGridView2.CurrentRow.Index].Value.ToString());
            lineas.lblProducto.Text=dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString();
            lineas.lblCompra.Text = dataGridView2[3, dataGridView2.CurrentRow.Index].Value.ToString();
            lineas.id = dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString();
            lineas.lblKilate.Text = dataGridView2[10, dataGridView2.CurrentRow.Index].Value.ToString();
            lineas.txtOro.Text = dataGridView2[16, dataGridView2.CurrentRow.Index].Value.ToString();
            lineas.lain = dataGridView2[11, dataGridView2.CurrentRow.Index].Value.ToString();
            lineas.txtDolar.Text = dataGridView2[15, dataGridView2.CurrentRow.Index].Value.ToString();
            lineas.Show();
            this.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmLineas lineas = new frmLineas();
            lineas.Show();
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Proveedor='DINASTI' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Proveedor='DINASTI' and Nombre LIKE '%" + textBox1.Text + "%';", conectar);
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
                da = new OleDbDataAdapter("select * from Inventario where  Proveedor='DINASTI' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Proveedor='DINASTI' and Id LIKE '%" + textBox2.Text + "%';", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                //dataGridView2.Columns[0].Visible = false;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                double DolarInicial, OroInicial, PrecioCompraDolares;
                DolarInicial = 21.85;
                OroInicial = 830;
                PrecioCompraDolares = Convert.ToDouble(dataGridView2[2, i].Value.ToString()) / DolarInicial;
                cmd = new OleDbCommand("UPDATE Inventario Set DolarInicial='" + DolarInicial + "', OroInicial='" + OroInicial + "', PrecioCompraDolares='" + PrecioCompraDolares + "' where Id='" + dataGridView2[0, i].Value.ToString() + "';", conectar);
                cmd.ExecuteNonQuery();
            
            }
            MessageBox.Show("");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarDinasti DINASTI = new frmAgregarDinasti();
            DINASTI.Show();
            this.Close();
        }
    }
}
