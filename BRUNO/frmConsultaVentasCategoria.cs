using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmConsultaVentasCategoria : frmBase
    {
        private SqlConnection conectar = new SqlConnection(Conexion.CadSQL);

        public frmConsultaVentasCategoria()
        {
            InitializeComponent();
        }

        private void frmConsultaVentasCategoria_Load(object sender, EventArgs e)
        {
            // Configurar fechas por defecto (Desde inicio de mes hasta hoy)
            dtpFechaInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            dtpFechaFin.Value = DateTime.Now;

            EstilizarDataGridView(dgvCategorias);
            EstilizarBotonPrimario(btnConsultar);
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            CargarVentasPorCategoria();
        }

        private void CargarVentasPorCategoria()
        {
            try
            {
                if (conectar.State != ConnectionState.Open)
                {
                    conectar.Open();
                }

                // Ajustar horas para abarcar todo el día final (hasta las 23:59:59)
                DateTime fInicio = dtpFechaInicio.Value.Date;
                DateTime fFin = dtpFechaFin.Value.Date.AddDays(1).AddSeconds(-1);

                string fechaInicioSQL = fInicio.ToString("yyyy-MM-dd HH:mm:ss");
                string fechaFinSQL = fFin.ToString("yyyy-MM-dd HH:mm:ss");

                string query = "SELECT IIf(Categoria Is Null OR Categoria = '', 'SIN CATEGORIA', Categoria) as Categoria, " +
                               "SUM(Cantidad) as Piezas, " +
                               "SUM(MontoTotal) as Total " +
                               "FROM VentasContado " +
                               "WHERE Fecha >= '" + fechaInicioSQL + "' and Fecha <= '" + fechaFinSQL + "' " +
                               "AND FolioVenta IN (SELECT Folio FROM Ventas WHERE Estatus = 'COBRADO') " +
                               "GROUP BY IIf(Categoria Is Null OR Categoria = '', 'SIN CATEGORIA', Categoria) " +
                               "UNION ALL " +
                               "SELECT 'TOTAL GENERAL' as Categoria, " +
                               "SUM(Cantidad) as Piezas, " +
                               "SUM(MontoTotal) as Total " +
                               "FROM VentasContado " +
                               "WHERE Fecha >= '" + fechaInicioSQL + "' and Fecha <= '" + fechaFinSQL + "' " +
                               "AND FolioVenta IN (SELECT Folio FROM Ventas WHERE Estatus = 'COBRADO')";

                SqlDataAdapter da = new SqlDataAdapter(query, conectar);
                DataSet ds = new DataSet();
                da.Fill(ds, "Categorias");

                dgvCategorias.DataSource = ds.Tables["Categorias"];

                // Ajustar anchos y alineación
                if (dgvCategorias.Columns["Categoria"] != null) dgvCategorias.Columns["Categoria"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                if (dgvCategorias.Columns["Piezas"] != null) dgvCategorias.Columns["Piezas"].Width = 120;
                if (dgvCategorias.Columns["Total"] != null) dgvCategorias.Columns["Total"].Width = 150;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar las ventas por categoría: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conectar.State == ConnectionState.Open)
                {
                    conectar.Close();
                }
            }
        }

        private void dgvCategorias_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Formato de moneda para el Total
            if (dgvCategorias.Columns[e.ColumnIndex].Name == "Total")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
            // Formato numérico entero para las Piezas
            else if (dgvCategorias.Columns[e.ColumnIndex].Name == "Piezas")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("N0");
                    e.FormattingApplied = true;
                }
            }

            // Resaltar la fila del TOTAL GENERAL
            if (dgvCategorias.Rows[e.RowIndex].Cells["Categoria"].Value?.ToString() == "TOTAL GENERAL")
            {
                dgvCategorias.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(dgvCategorias.Font, FontStyle.Bold);
                dgvCategorias.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
            }
        }
    }
}