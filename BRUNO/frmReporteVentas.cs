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
    public partial class frmReporteVentas : Form
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;
        public String usuario = "";
        public frmReporteVentas()
        {
            InitializeComponent();
            this.MinimumSize = new Size(1011, 629);
        }

        private void frmReporteVentas_Load(object sender, EventArgs e)
        {
            conectar.Open();
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from Ventas where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59# ORDER BY Fecha DESC;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            // Seleccionar el primer registro si hay filas en el DataGridView
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[1]; // Selecciona la celda en la columna 1 (ajusta según tu caso)
                dataGridView1.Rows[0].Selected = true; // Opcional: Resalta toda la fila
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from Ventas where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59# ORDER BY Fecha DESC;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                frmVentaDetallada detalles = new frmVentaDetallada();
                detalles.lblFolio.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
                double monto = Convert.ToDouble(dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString());
                detalles.lblDescuento.Text = $"{monto:C}";

                detalles.lblPago.Text = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
                detalles.lblFecha.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
                if (dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString() == "CANCELADO")
                {
                    detalles.button1.Visible = false;
                    detalles.button2.Visible = false;
                }
                else
                {
                    detalles.button1.Visible = true;
                    detalles.button2.Visible = true;
                }
                decimal montoTotal = Convert.ToDecimal(dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString());
                detalles.lblMonto.Text = $"{montoTotal:C}";
                detalles.usuario = usuario;
                detalles.Show();
                this.Close();
            }
            catch (Exception ex)
            {

            }

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("Select * from Ventas where Folio like '%"+textBox1.Text+"%';", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Monto" || dataGridView1.Columns[e.ColumnIndex].Name == "Descuento" )
                /*||

             dataGridView2.Columns[e.ColumnIndex].Name == "PrecioVentaMayoreo" ||
             dataGridView2.Columns[e.ColumnIndex].Name == "Especial")*/
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2"); // Formato moneda con 2 decimales
                    e.FormattingApplied = true;
                }
            }
        }
    }
}
