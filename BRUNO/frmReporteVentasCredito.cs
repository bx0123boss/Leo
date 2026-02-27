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
    public partial class frmReporteVentasCredito : frmBase
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;
        public String usuario = "";
        public frmReporteVentasCredito()
        {
            InitializeComponent();
        }

        private void frmReporteVentasCredito_Load(object sender, EventArgs e)
        {
            conectar.Open();
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from Ventas2 where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            // Seleccionar el primer registro si hay filas en el DataGridView
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[1]; // Selecciona la celda en la columna 1 (ajusta según tu caso)
                dataGridView1.Rows[0].Selected = true; // Opcional: Resalta toda la fila
            }
            EstilizarTextBox(textBox1);
            EstilizarBotonPrimario(button1);
            EstilizarDataGridView(dataGridView1);
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
                {
                    frmVentaDetalladaCredito detalles = new frmVentaDetalladaCredito();
                    detalles.lblFolio.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
                    detalles.lblFecha.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
                    detalles.button1.Visible = true;
                    detalles.id = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
                    detalles.lblMonto.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                    if (dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString() == "CANCELADO")
                    {
                        detalles.button1.Hide();
                    }
                    //detalles.adeudo = Convert.ToDouble(dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString());
                    detalles.usuario = usuario;
                    detalles.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un registro de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch(Exception Ex)
            {
                
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from Ventas2 where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("Select * from Ventas2 where Folio='" + textBox1.Text + "';", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }
    }
}
