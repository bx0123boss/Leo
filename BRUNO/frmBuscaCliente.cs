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
    public partial class frmBuscaCliente : frmBase // Heredamos de frmBase para mantener el diseño
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;

        bool selec = false;
        public string ID { get; set; }
        public string Nombre { get; set; }
        public string Adeudo { get; set; }
        public string Limite { get; set; }
        public string direccion { get; set; }
        public string tel { get; set; }
        public string correo { get; set; }

        public frmBuscaCliente()
        {
            InitializeComponent();
            AplicarEstilos();
        }

        private void AplicarEstilos()
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPrimario(this.button1); // Botón Aceptar
            EstilizarBotonAdvertencia(this.button2); // Botón Nuevo Cliente
            EstilizarTextBox(this.textBox1);
        }

        private void frmBuscaCliente_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Clientes where Estatus='ACTIVO' ORDER BY Nombre;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;

            // Ajustar el foco al buscador al abrir
            textBox1.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Clientes where Estatus='ACTIVO' ORDER BY Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                if (dataGridView1.Columns.Count > 2)
                    dataGridView1.Columns[2].Visible = false;
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Clientes where Estatus='ACTIVO' and Nombre LIKE '%" + textBox1.Text + "%' ORDER BY Nombre ;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selec && dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                ID = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                Nombre = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                Adeudo = dataGridView1[7, dataGridView1.CurrentRow.Index].Value.ToString();
                Limite = dataGridView1[8, dataGridView1.CurrentRow.Index].Value.ToString();
                direccion = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
                tel = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
                correo = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ningún cliente", "¡ALTO!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (frmAgregarClientes cliente = new frmAgregarClientes())
            {
                cliente.buscar = true;
                if (cliente.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = cliente.Nombre;
                    conectar.Close();
                    conectar.Open();
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Clientes where Estatus='ACTIVO' and Nombre LIKE '%" + textBox1.Text + "%' ORDER BY Nombre ;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    dataGridView1.Columns[0].Visible = false;
                }
                cliente.Show();
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                selec = true;
            }
        }
    }
}