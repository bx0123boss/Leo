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
    public partial class frmVentasPorProducto : frmBase
    {
        // Se asume que usas la cadena de conexión centralizada
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        public String usuario = "";

        public frmVentasPorProducto()
        {
            InitializeComponent();
            this.MinimumSize = new Size(1011, 629);
        }

        private void frmVentasPorProducto_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarComboBox(cmbCategoria); // Si tienes este método en tu frmBase
            EstilizarBotonPrimario(this.button1);

            CargarCategorias();
            CargarDatos();
        }

        private void CargarCategorias()
        {
            try
            {
                conectar.Open();
                string queryCat = "SELECT DISTINCT Categoria FROM Inventario WHERE Categoria IS NOT NULL AND Categoria <> ''";
                using (OleDbCommand cmd = new OleDbCommand(queryCat, conectar))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        cmbCategoria.Items.Clear();
                        cmbCategoria.Items.Add("TODAS");

                        while (reader.Read())
                        {
                            cmbCategoria.Items.Add(reader["Categoria"].ToString());
                        }
                    }
                }

                if (cmbCategoria.Items.Count > 0)
                {
                    cmbCategoria.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar categorías: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conectar.Close();
            }
        }

        private void CargarDatos()
        {
            if (cmbCategoria.SelectedIndex == -1) return;

            try
            {
                DateTime fechaInicio = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 0, 0, 0);
                DateTime fechaFin = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 23, 59, 59);

                // Creamos una DataTable vacía donde iremos metiendo los resultados
                DataTable dtResultados = new DataTable("VentasProd");

                // --- 1. CONSULTA DE CRÉDITO (Siempre se muestran) ---
                // Le agregamos la palabra 'CRÉDITO' como columna para saber de dónde viene
                string queryCredito = @"SELECT 'CRÉDITO' AS TipoVenta, VC.FolioVenta, VC.IdProducto, VC.Cantidad, VC.Producto, VC.MontoTotal, VC.Fecha, V.Estatus, VC.Categoria 
                                        FROM VentasCredito VC
                                        INNER JOIN Ventas2 V ON VC.FolioVenta = V.Folio
                                        WHERE VC.Fecha >= @fechaInicio AND VC.Fecha <= @fechaFin ";

                if (cmbCategoria.Text != "TODAS") queryCredito += " AND VC.Categoria = @categoria ";

                using (OleDbCommand cmdCredito = new OleDbCommand(queryCredito, conectar))
                {
                    cmdCredito.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    cmdCredito.Parameters.AddWithValue("@fechaFin", fechaFin);
                    if (cmbCategoria.Text != "TODAS") cmdCredito.Parameters.AddWithValue("@categoria", cmbCategoria.Text);

                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmdCredito))
                    {
                        da.Fill(dtResultados); // Metemos las ventas a crédito a la tabla
                    }
                }

                // --- 2. CONSULTA DE CONTADO (Se agregan SOLO si el CheckBox NO está marcado) ---
                if (!chkSoloCredito.Checked)
                {
                    string queryContado = @"SELECT 'CONTADO' AS TipoVenta, VC.FolioVenta, VC.IdProducto, VC.Cantidad, VC.Producto, VC.MontoTotal, VC.Fecha, V.Estatus, VC.Categoria 
                                            FROM VentasContado VC
                                            INNER JOIN Ventas V ON VC.FolioVenta = V.Folio
                                            WHERE VC.Fecha >= @fechaInicio AND VC.Fecha <= @fechaFin ";

                    if (cmbCategoria.Text != "TODAS") queryContado += " AND VC.Categoria = @categoria ";

                    using (OleDbCommand cmdContado = new OleDbCommand(queryContado, conectar))
                    {
                        cmdContado.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                        cmdContado.Parameters.AddWithValue("@fechaFin", fechaFin);
                        if (cmbCategoria.Text != "TODAS") cmdContado.Parameters.AddWithValue("@categoria", cmbCategoria.Text);

                        using (OleDbDataAdapter daContado = new OleDbDataAdapter(cmdContado))
                        {
                            daContado.Fill(dtResultados); // Metemos las de contado a la MISMA tabla
                        }
                    }
                }

                // Ordenamos toda la tabla combinada por Fecha descendente
                dtResultados.DefaultView.Sort = "Fecha DESC";

                // Refrescamos el grid
                dataGridView1.DataSource = dtResultados.DefaultView;

                this.dataGridView1.ReadOnly = true;
                this.dataGridView1.AllowUserToAddRows = false;
                this.dataGridView1.AllowUserToDeleteRows = false;

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                    dataGridView1.Rows[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void cmbCategoria_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void chkSoloCredito_CheckedChanged(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
                {
                    // Obtenemos los datos de la fila
                    string tipoVenta = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["TipoVenta"].Value.ToString();
                    string folio = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["FolioVenta"].Value.ToString();
                    string fecha = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Fecha"].Value.ToString();
                    string estatus = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Estatus"].Value.ToString();

                    // VERIFICAMOS DE QUÉ TABLA PROVIENE PARA ABRIR EL FORMULARIO CORRECTO
                    if (tipoVenta == "CRÉDITO")
                    {
                        frmVentaDetalladaCredito detallesCredito = new frmVentaDetalladaCredito();
                        detallesCredito.lblFolio.Text = folio;
                        detallesCredito.lblFecha.Text = fecha;

                        if (estatus == "CANCELADO")
                        {
                            detallesCredito.button1.Visible = false;
                            detallesCredito.button2.Visible = false;
                        }
                        else
                        {
                            detallesCredito.button1.Visible = true;
                            detallesCredito.button2.Visible = true;
                        }

                        detallesCredito.usuario = usuario;
                        detallesCredito.Show();
                    }
                    else
                    {
                        frmVentaDetallada detallesContado = new frmVentaDetallada();
                        detallesContado.lblFolio.Text = folio;
                        detallesContado.lblFecha.Text = fecha;

                        if (estatus == "CANCELADO")
                        {
                            detallesContado.button1.Visible = false;
                            detallesContado.button2.Visible = false;
                        }
                        else
                        {
                            detallesContado.button1.Visible = true;
                            detallesContado.button2.Visible = true;
                        }

                        detallesContado.usuario = usuario;
                        detallesContado.Show();
                    }

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un registro de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir los detalles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "MontoTotal")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name == "Estatus")
            {
                if (e.Value != null && e.Value.ToString() == "CANCELADO")
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                }
            }
        }
    }
}