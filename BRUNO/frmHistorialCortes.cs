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
    public partial class frmHistorialCortes : frmBase
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;

        public frmHistorialCortes()
        {
            InitializeComponent();
        }

        private void frmHistorialCortes_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPrimario(this.button1);
            EstilizarBotonPrimario(this.button2);
            //ds = new DataSet();
            conectar.Open();
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from histocortes where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from histocortes where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
       
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Validar que realmente haya una fila seleccionada
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
            {
                MessageBox.Show("Por favor, seleccione un registro de la lista primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Detiene la ejecución, no hace nada más
            }

            try
            {
                frmDetalleCorte detail = new frmDetalleCorte();

                // Guardamos el índice para no repetir "dataGridView1.CurrentRow.Index" en cada línea
                int index = dataGridView1.CurrentRow.Index;

                detail.ID = Convert.ToInt32(dataGridView1[0, index].Value);
                detail.lblMonto.Text = dataGridView1[1, index].Value.ToString();
                detail.lblFecha.Text = dataGridView1[2, index].Value.ToString();
                decimal inversion = Convert.ToDecimal(dataGridView1[7, index].Value);
                detail.lblInversion.Text = inversion.ToString("C2");

                decimal utilidad = Convert.ToDecimal(dataGridView1[6, index].Value);
                detail.lblUtilidad.Text = utilidad.ToString("C2");

                detail.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
            {
                MessageBox.Show("Por favor, seleccione un registro de la lista primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }
            frmCortePorFechas fch = new frmCortePorFechas();
            fch.Show();
            this.Close();
        }
    }
}
