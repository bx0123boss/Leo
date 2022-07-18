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
    public partial class frmBuscarProveedor : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        public string ID { get; set; }
        public string Nombre { get; set; }
        public frmBuscarProveedor()
        {
            InitializeComponent();
        }

        private void frmBuscarProveedor_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Proveedores ORDER BY Nombre;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ID = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            Nombre = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (frmAgregarProveedor cliente = new frmAgregarProveedor())
            {
                cliente.buscar = true;
                if (cliente.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = cliente.Nombre;
                    conectar.Close();
                    conectar.Open();
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Proveedores where Nombre LIKE '%" + textBox1.Text + "%' ORDER BY Nombre ;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    dataGridView1.Columns[0].Visible = false;
                }
                cliente.Show();
            }
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
                dataGridView1.Columns[2].Visible = false;
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
    }
}
