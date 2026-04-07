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
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;

        // --- NUEVO PARÁMETRO: Define si lee de histocortes o histocortesB ---
        public bool ConsultaB { get; set; } = false;

        public frmHistorialCortes()
        {
            InitializeComponent();
        }

        private void frmHistorialCortes_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPrimario(this.button1);
            EstilizarBotonPrimario(this.button2);

            conectar.Open();

            CargarDatos();

            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
        }

        // =========================================================================
        // MÉTODO CENTRALIZADO PARA CARGAR DATOS
        // =========================================================================
        private void CargarDatos()
        {
            try
            {
                // Elegimos la tabla según el parámetro
                string tablaHistorial = ConsultaB ? "histocortesB" : "histocortes";

                string fechaFiltro = dateTimePicker1.Value.ToString("MM/dd/yyyy");

                ds = new DataSet();
                string sql = $"Select * from {tablaHistorial} where Fecha >= #{fechaFiltro} 00:00:00# and Fecha <= #{fechaFiltro} 23:59:59# ORDER BY Id DESC;";

                da = new OleDbDataAdapter(sql, conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];

                // Ocultamos columnas innecesarias
                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns[0].Visible = false; // ID
                    dataGridView1.Columns[3].Visible = false; // Mas
                    dataGridView1.Columns[4].Visible = false; // Menos
                    dataGridView1.Columns[5].Visible = false; // Tarjeta
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar historial: " + ex.Message);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Validar selección
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
            {
                MessageBox.Show("Por favor, seleccione un registro de la lista primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                frmDetalleCorte detail = new frmDetalleCorte();

                // --- NUEVO: Pasar el parámetro al formulario de detalle ---
                // Nota: Debes agregar la propiedad 'EsCorteB' en tu frmDetalleCorte también
                detail.EsCorteB = this.ConsultaB;

                int index = dataGridView1.CurrentRow.Index;

                // Si guardaste el folio con ceros (D6), aquí se mantiene el valor numérico para la consulta
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

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Formatear montos a moneda si es necesario
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Monto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }
    }
}