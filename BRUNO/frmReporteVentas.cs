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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BRUNO
{
    public partial class frmReporteVentas : frmBase
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;

        public String usuario = "";

        // --- PARÁMETRO: Define si lee de Ventas o VentasB ---
        public bool ConsultaVentasB { get; set; } = false;

        public frmReporteVentas()
        {
            InitializeComponent();
            this.MinimumSize = new Size(1011, 629);
        }

        private void frmReporteVentas_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarTextBox(textBox1);
            EstilizarBotonPrimario(this.button1);
            conectar.Open();

            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;

            // Cargamos los datos iniciales
            CargarDatos();
        }

        // =========================================================================
        // MÉTODO CENTRALIZADO PARA CARGAR DATOS
        // =========================================================================
        private void CargarDatos(string filtro = "")
        {
            string fechaInicio = dateTimePicker1.Value.ToString("MM/dd/yyyy 00:00:00");
            string fechaFin = dateTimePicker1.Value.ToString("MM/dd/yyyy 23:59:59");
            string query = "";

            if (ConsultaVentasB)
            {
                // Consulta para VentasB: 
                // FolioB se disfraza como "Folio" para que el usuario lo vea.
                // FolioA (El real) lo traemos al final para ocultarlo.
                if (string.IsNullOrEmpty(filtro))
                {
                    query = $"SELECT Id, Monto, Fecha, FolioB AS Folio, Estatus, Descuento, Pago, FolioA FROM VentasB WHERE Fecha >= #{fechaInicio}# AND Fecha <= #{fechaFin}# ORDER BY Fecha DESC;";
                }
                else
                {
                    query = $"SELECT Id, Monto, Fecha, FolioB AS Folio, Estatus, Descuento, Pago, FolioA FROM VentasB WHERE FolioA LIKE '%{filtro}%' OR FolioB LIKE '%{filtro}%' ORDER BY Fecha DESC;";
                }
            }
            else
            {
                // Consulta normal para Ventas
                if (string.IsNullOrEmpty(filtro))
                {
                    query = $"SELECT * FROM Ventas WHERE Fecha >= #{fechaInicio}# AND Fecha <= #{fechaFin}# ORDER BY Fecha DESC;";
                }
                else
                {
                    query = $"SELECT * FROM Ventas WHERE Folio LIKE '%{filtro}%' ORDER BY Fecha DESC;";
                }
            }

            ds = new DataSet();
            da = new OleDbDataAdapter(query, conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];

            // Ocultar siempre el ID
            dataGridView1.Columns[0].Visible = false;

            // --- OCULTAR EL FOLIO REAL (FolioA) CUANDO ES VENTAS B ---
            if (ConsultaVentasB && dataGridView1.Columns.Contains("FolioA"))
            {
                dataGridView1.Columns["FolioA"].Visible = false; // ¡El usuario nunca lo ve!
            }

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[1];
                dataGridView1.Rows[0].Selected = true;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                CargarDatos(textBox1.Text.Trim());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
                {
                    frmVentaDetallada detalles = new frmVentaDetallada();
                    if (ConsultaVentasB)
                    {
                        // Mandamos el Real a la variable oculta
                        detalles.folioRealConsulta = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["FolioA"].Value.ToString();
                        // Y ponemos el Visual en la etiqueta
                        detalles.lblFolio.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
                    }
                    else
                    {
                        // Si es ventas normal, ambos son el mismo
                        detalles.lblFolio.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
                    }

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
                    detalles.monto = montoTotal;
                    detalles.usuario = usuario;
                    detalles.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un registro de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Monto" || dataGridView1.Columns[e.ColumnIndex].Name == "Descuento")
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