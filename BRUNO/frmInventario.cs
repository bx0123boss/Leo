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
using System.Globalization;
using Microsoft.Office.Interop.Excel;

namespace JaegerSoft
{
    public partial class frmInventario : frmBase
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        public String usuario = "Admin";

        private List<ProductoInventario> listaCompletaInventario;
        private List<ProductoInventario> listaFiltradaInventario;
        private int tamanoPagina = 30;
        private int paginaActual = 1;
        private int totalPaginas = 0;

        public class ProductoInventario
        {
            public string Id { get; set; }
            public string Nombre { get; set; }
            public decimal PrecioVentaMayoreo { get; set; }
            public decimal PrecioVenta { get; set; }
            public decimal Existencia { get; set; }
            public decimal Limite { get; set; }
            public string Categoria { get; set; }
            public decimal Especial { get; set; }
            public string IVA { get; set; }
            public string Unidad { get; set; }
            public string Uni { get; set; }

            // New fields
            public string ProveedorPrincipal { get; set; }
            public DateTime? FechaUltimaCompra { get; set; }
            public DateTime? FechaUltimaVenta { get; set; }
            public double VentasMes { get; set; }
            public double VentasAnio { get; set; }
            public double StockMaximo { get; set; }
            public int DiasReposicion { get; set; }

            // Calculated fields
            public string EstadoInventario
            {
                get
                {
                    if (Existencia < Limite) return "COMPRAR";
                    if (StockMaximo > 0 && Existencia > (decimal)StockMaximo) return "SOBRESTOCK";
                    return "NORMAL";
                }
            }

            public string NivelRotacion
            {
                get
                {
                    if (Existencia <= 0) return "BAJA";
                    double rotacion = VentasMes / (double)Existencia;
                    if (rotacion >= 1.5) return "ALTA ROTACIÓN";
                    if (rotacion >= 0.5) return "MEDIA";
                    return "BAJA";
                }
            }
        }

        public class ResumenCategoria
        {
            public string Categoria { get; set; }
            public decimal TotalExistencia { get; set; }
            public decimal TotalInversion { get; set; }
            public decimal TotalVenta { get; set; }
            public decimal Utilidad { get; set; }
        }

        public frmInventario()
        {
            InitializeComponent();
            this.MinimumSize = new Size(1346, 805);
        }

        private void BtnApartados_Click(object sender, EventArgs e)
        {
            frmAgregarInventario add = new frmAgregarInventario();
            add.Show();
            this.Close();
        }

        private void frmInventario_Load(object sender, EventArgs e)
        {
            // 1. Estilizar Controles y DataGridView2 (Estilo Base)
            EstilizarComboBox(this.comboBox2);
            EstilizarDataGridView(this.dataGridView2);
            this.dataGridView2.DataBindingComplete += DataGridView2_DataBindingComplete;
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;

            EstilizarBotonPaginador(btnPrimero);
            EstilizarBotonPaginador(btnAnterior);
            EstilizarBotonPaginador(btnSiguiente);
            EstilizarBotonPaginador(btnUltimo);

            conectar.Open();
            System.Data.DataTable dt = new System.Data.DataTable();
            cmd = new OleDbCommand("Select Id,Nombre from Categorias;", conectar);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            comboBox2.DisplayMember = "Nombre";
            comboBox2.ValueMember = "Id";
            comboBox2.DataSource = dt;
            DataSet ds = new DataSet();
            da = new OleDbDataAdapter("select * from Inventario order by Nombre;", conectar);
            da.Fill(ds, "Inventario");

            // Permisos de visibilidad en interfaz
            bool tienePermisoAnalitico = Sesion.TienePermiso("VER_ANALITICO_INVENTARIO");
            btnAlmacenes.Visible = Sesion.TienePermiso("MOD_ALMACENES");
            panelAnalitico.Visible = tienePermisoAnalitico;
            panelKpiContainer.Visible = tienePermisoAnalitico;

            // 2. Convertir el DataSet (DataTable) a una List<ProductoInventario>
            listaCompletaInventario = new List<ProductoInventario>();

            if (ds.Tables.Count > 0 && ds.Tables["Inventario"].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables["Inventario"].Rows)
                {
                    ProductoInventario prod = new ProductoInventario();

                    prod.Id = row["Id"] == DBNull.Value ? "" : row["Id"].ToString();
                    prod.Nombre = row["Nombre"] == DBNull.Value ? "" : row["Nombre"].ToString();
                    prod.PrecioVentaMayoreo = row["PrecioVentaMayoreo"] == DBNull.Value ? 0m : Convert.ToDecimal(row["PrecioVentaMayoreo"]);
                    prod.PrecioVenta = row["PrecioVenta"] == DBNull.Value ? 0m : Convert.ToDecimal(row["PrecioVenta"]);
                    prod.Existencia = row["Existencia"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Existencia"]);
                    prod.Limite = row["Limite"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Limite"]);
                    prod.Categoria = row["Categoria"] == DBNull.Value ? "" : row["Categoria"].ToString();
                    prod.Especial = row["Especial"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Especial"]);
                    prod.IVA = row["IVA"] == DBNull.Value ? "" : row["IVA"].ToString();
                    prod.Unidad = row["Unidad"] == DBNull.Value ? "" : row["Unidad"].ToString();
                    prod.Uni = row["Uni"] == DBNull.Value ? "" : row["Uni"].ToString();

                    prod.ProveedorPrincipal = row.Table.Columns.Contains("ProveedorPrincipal") && row["ProveedorPrincipal"] != DBNull.Value ? row["ProveedorPrincipal"].ToString() : "";
                    prod.FechaUltimaCompra = row.Table.Columns.Contains("FechaUltimaCompra") && row["FechaUltimaCompra"] != DBNull.Value ? Convert.ToDateTime(row["FechaUltimaCompra"]) : (DateTime?)null;
                    prod.FechaUltimaVenta = row.Table.Columns.Contains("FechaUltimaVenta") && row["FechaUltimaVenta"] != DBNull.Value ? Convert.ToDateTime(row["FechaUltimaVenta"]) : (DateTime?)null;
                    prod.StockMaximo = row.Table.Columns.Contains("StockMaximo") && row["StockMaximo"] != DBNull.Value ? Convert.ToDouble(row["StockMaximo"]) : 0.0;
                    prod.DiasReposicion = row.Table.Columns.Contains("DiasReposicion") && row["DiasReposicion"] != DBNull.Value ? Convert.ToInt32(row["DiasReposicion"]) : 0;

                    listaCompletaInventario.Add(prod);
                }
            }

            // OPTIMIZACIÓN: Solo ejecutar la query pesada de ventas si el usuario va a ver el dashboard analítico
            if (tienePermisoAnalitico)
            {
                CargarVentasDinamicas(listaCompletaInventario);
            }

            listaFiltradaInventario = new List<ProductoInventario>(listaCompletaInventario);
            totalPaginas = (int)Math.Ceiling((double)listaCompletaInventario.Count / tamanoPagina);

            // OPTIMIZACIÓN: Omitir los pesados bucles de cálculo de KPIs matemáticos (.Sum, .Where, etc.)
            if (tienePermisoAnalitico)
            {
                ActualizarKpiValues();
                this.Resize += FrmInventario_Resize;
                AjustarDisenoAnalitico();
            }

            CargarPagina();
        }

        private void FrmInventario_Resize(object sender, EventArgs e)
        {
            if (Sesion.TienePermiso("VER_ANALITICO_INVENTARIO"))
            {
                AjustarDisenoAnalitico();
            }
        }

        private void AjustarDisenoAnalitico()
        {
            if (panelAnalitico == null || !panelAnalitico.Visible) return;

            int W = panelAnalitico.Width;
            if (W <= 0) return;

            if (lblFinancieroTitulo != null) lblFinancieroTitulo.Location = new System.Drawing.Point(20, 6);

            if (lblFinancieroInversion != null) lblFinancieroInversion.Location = new System.Drawing.Point(20, 25);
            if (lblFinancieroVenta != null) lblFinancieroVenta.Location = new System.Drawing.Point(240, 25);
            if (lblFinancieroUtilidad != null) lblFinancieroUtilidad.Location = new System.Drawing.Point(460, 25);

            if (lblFinancieroMargen != null) lblFinancieroMargen.Location = new System.Drawing.Point(20, 52);
            if (lblFinancieroInmovilizado != null) lblFinancieroInmovilizado.Location = new System.Drawing.Point(240, 52);

            int RS = (W / 2) + 20;

            if (lblRiesgoTitulo != null) lblRiesgoTitulo.Location = new System.Drawing.Point(RS, 6);

            if (lblRiesgoCriticos != null) lblRiesgoCriticos.Location = new System.Drawing.Point(RS, 25);
            if (lblRiesgoSobrestock != null) lblRiesgoSobrestock.Location = new System.Drawing.Point(RS + 240, 25);

            if (lblRiesgoRotacion != null) lblRiesgoRotacion.Location = new System.Drawing.Point(RS, 52);
            if (lblRiesgoReposicion != null) lblRiesgoReposicion.Location = new System.Drawing.Point(RS + 240, 52);
        }

        #region PAGINACION
        private void CargarPagina()
        {
            if (paginaActual < 1) paginaActual = 1;
            if (paginaActual > totalPaginas && totalPaginas > 0) paginaActual = totalPaginas;

            var datosPaginados = listaFiltradaInventario
                .Skip((paginaActual - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            dataGridView2.DataSource = datosPaginados;
            totalPaginas = (int)Math.Ceiling((double)listaFiltradaInventario.Count / tamanoPagina);
            ActualizarControlesPaginacion();
        }

        private void UpdateKpiSafe()
        {
            if (Sesion.TienePermiso("VER_ANALITICO_INVENTARIO"))
            {
                ActualizarKpiValues();
            }
        }

        private void ActualizarControlesPaginacion()
        {
            lblEstado.Text = $"Página {paginaActual} de {totalPaginas}";

            btnPrimero.Enabled = (paginaActual > 1);
            btnAnterior.Enabled = (paginaActual > 1);

            btnSiguiente.Enabled = (paginaActual < totalPaginas);
            btnUltimo.Enabled = (paginaActual < totalPaginas);
        }

        private void btnPrimero_Click(object sender, EventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual = 1;
                CargarDatosPaginados();
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                CargarDatosPaginados();
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (paginaActual < totalPaginas)
            {
                paginaActual++;
                CargarDatosPaginados();
            }
        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            if (paginaActual < totalPaginas)
            {
                paginaActual = totalPaginas;
                CargarDatosPaginados();
            }
        }

        private void CargarDatosPaginados()
        {
            if (listaFiltradaInventario == null || listaFiltradaInventario.Count == 0)
            {
                dataGridView2.DataSource = null;
                totalPaginas = 1;
                ActualizarControlesNavegacion();
                return;
            }

            totalPaginas = (int)Math.Ceiling((double)listaFiltradaInventario.Count / tamanoPagina);

            if (paginaActual > totalPaginas) paginaActual = totalPaginas;
            if (paginaActual < 1) paginaActual = 1;

            var datosPaginados = listaFiltradaInventario
                .Skip((paginaActual - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            dataGridView2.DataSource = datosPaginados;
            ActualizarControlesNavegacion();
        }

        private void ActualizarControlesNavegacion()
        {
            lblEstado.Text = $"Página {paginaActual} de {totalPaginas}";

            btnPrimero.Enabled = (paginaActual > 1);
            btnAnterior.Enabled = (paginaActual > 1);

            btnSiguiente.Enabled = (paginaActual < totalPaginas);
            btnUltimo.Enabled = (paginaActual < totalPaginas);
        }
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView2.CurrentRow != null)
                {
                    ProductoInventario prod = (ProductoInventario)dataGridView2.CurrentRow.DataBoundItem;

                    frmEditarProducto editar = new frmEditarProducto();
                    editar.usuario = usuario;
                    editar.inventario = "INVENT";
                    editar.txtID.Text = prod.Id;
                    editar.txtProducto.Text = prod.Nombre;
                    editar.txtCompra.Text = prod.Especial.ToString();
                    editar.txtVenta.Text = prod.PrecioVenta.ToString();
                    editar.cmbCategoria.Text = prod.Categoria;
                    editar.txtLimite.Text = prod.Limite.ToString();
                    editar.comboBox1.Text = prod.IVA;

                    System.Data.DataTable dt = new System.Data.DataTable();
                    cmd = new OleDbCommand("Select Id,Nombre from Unidades;", conectar);
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);
                    editar.cmbUnidad.DisplayMember = "Nombre";
                    editar.cmbUnidad.ValueMember = "Id";
                    editar.cmbUnidad.DataSource = dt;
                    editar.cmbUnidad.SelectedValue = prod.Unidad;
                    editar.cmbUnidad.Text = prod.Uni;
                    editar.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Silently handle
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null)
            {
                MessageBox.Show("Por favor, seleccione un producto para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ProductoInventario productoSeleccionado = (ProductoInventario)dataGridView2.CurrentRow.DataBoundItem;
            string idParaEliminar = productoSeleccionado.Id;

            try
            {
                DialogResult dialogResult = MessageBox.Show("¿Estas seguro de eliminar el producto?", "Alto!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    string sqlInsert = @"INSERT INTO InventarioSusp 
                (Id, Nombre, PrecioVentaMayoreo, PrecioVenta, Existencia, Limite, Categoria, Especial, IVA, Unidad, Uni) 
                VALUES 
                (@Id, @Nombre, @PrecioVentaMayoreo, @PrecioVenta, @Existencia, @Limite, @Categoria, @Especial, @IVA, @Unidad, @Uni)";

                    using (OleDbCommand cmdInsert = new OleDbCommand(sqlInsert, conectar))
                    {
                        cmdInsert.Parameters.AddWithValue("@Id", productoSeleccionado.Id);
                        cmdInsert.Parameters.AddWithValue("@Nombre", productoSeleccionado.Nombre);
                        cmdInsert.Parameters.AddWithValue("@PrecioVentaMayoreo", productoSeleccionado.PrecioVentaMayoreo);
                        cmdInsert.Parameters.AddWithValue("@PrecioVenta", productoSeleccionado.PrecioVenta);
                        cmdInsert.Parameters.AddWithValue("@Existencia", productoSeleccionado.Existencia);
                        cmdInsert.Parameters.AddWithValue("@Limite", productoSeleccionado.Limite);
                        cmdInsert.Parameters.AddWithValue("@Categoria", productoSeleccionado.Categoria);
                        cmdInsert.Parameters.AddWithValue("@Especial", productoSeleccionado.Especial);
                        cmdInsert.Parameters.AddWithValue("@IVA", productoSeleccionado.IVA);
                        cmdInsert.Parameters.AddWithValue("@Unidad", productoSeleccionado.Unidad);
                        cmdInsert.Parameters.AddWithValue("@Uni", productoSeleccionado.Uni);

                        cmdInsert.ExecuteNonQuery();
                    }

                    string sqlDelete = "DELETE FROM Inventario WHERE Id = @Id";
                    using (OleDbCommand cmdDelete = new OleDbCommand(sqlDelete, conectar))
                    {
                        cmdDelete.Parameters.AddWithValue("@Id", idParaEliminar);
                        cmdDelete.ExecuteNonQuery();
                    }

                    MessageBox.Show("PRODUCTO ELIMINADO CON EXITO", "ELIMINADO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    var itemAEliminar = listaCompletaInventario.FirstOrDefault(p => p.Id == idParaEliminar);
                    if (itemAEliminar != null)
                    {
                        listaCompletaInventario.Remove(itemAEliminar);
                        listaFiltradaInventario.Remove(itemAEliminar);
                    }

                    UpdateKpiSafe();

                    paginaActual = 1;
                    CargarDatosPaginados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarFiltros()
        {
            if (listaCompletaInventario == null) return;

            var filtro = listaCompletaInventario.AsEnumerable();

            if (checkBox1.Checked)
            {
                filtro = filtro.Where(p => p.Categoria != null &&
                                           p.Categoria == comboBox2.Text &&
                                           p.Categoria != "ACCESORIOS");
            }

            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                filtro = filtro.Where(p => p.Nombre != null &&
                                           p.Nombre.IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(textBox2.Text))
            {
                filtro = filtro.Where(p => p.Id != null &&
                                           p.Id.IndexOf(textBox2.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            listaFiltradaInventario = filtro.ToList();
            paginaActual = 1;
            CargarDatosPaginados();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                AplicarFiltros();
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                AplicarFiltros();
                e.Handled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = !checkBox1.Checked;
            AplicarFiltros();
        }

        private void ExportarInventarioExcel(object sender, EventArgs e)
        {
            try
            {
                DialogResult respuesta = MessageBox.Show(
                    "¿Deseas separar el inventario por Categorías en hojas distintas?\n\n" +
                    "SÍ: Crea una hoja de Excel por cada categoría.\n" +
                    "NO: Exporta todo el inventario en una sola lista continua.",
                    "Opciones de Exportación",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.Cancel) return;

                this.Cursor = Cursors.WaitCursor;
                System.Data.DataTable dtInventario = new System.Data.DataTable();
                using (OleDbConnection conn = new OleDbConnection(Conexion.CadCon))
                {
                    conn.Open();
                    string query = "SELECT * FROM Inventario ORDER BY Categoria, Nombre";
                    OleDbDataAdapter da = new OleDbDataAdapter(query, conn);
                    da.Fill(dtInventario);
                }

                if (dtInventario.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = excelApp.Workbooks.Add(XlSheetType.xlWorksheet);

                while (workbook.Worksheets.Count > 1)
                {
                    ((Worksheet)workbook.Worksheets[1]).Delete();
                }

                if (respuesta == DialogResult.Yes)
                {
                    var categorias = dtInventario.AsEnumerable()
                                                 .Select(row => row["Categoria"].ToString())
                                                 .Distinct()
                                                 .ToList();

                    foreach (string categoria in categorias)
                    {
                        DataRow[] filas = dtInventario.Select($"Categoria = '{categoria.Replace("'", "''")}'");
                        if (filas.Length > 0)
                        {
                            Worksheet ws;
                            if (workbook.Worksheets.Count == 1 && ((Worksheet)workbook.Worksheets[1]).Name.Contains("Hoja"))
                                ws = (Worksheet)workbook.Worksheets[1];
                            else
                                ws = (Worksheet)workbook.Worksheets.Add(After: workbook.Worksheets[workbook.Worksheets.Count]);

                            string nombreHoja = string.IsNullOrEmpty(categoria) ? "Sin Categoria" : categoria;
                            nombreHoja = nombreHoja.Replace("/", "-").Replace("*", "").Replace("?", "").Replace("[", "").Replace("]", "").Replace(":", "");
                            if (nombreHoja.Length > 30) nombreHoja = nombreHoja.Substring(0, 30);
                            try { ws.Name = nombreHoja; } catch { }

                            VolcarDatosAExcel(ws, dtInventario.Columns, filas);
                        }
                    }
                }
                else if (respuesta == DialogResult.No)
                {
                    Worksheet ws = (Worksheet)workbook.Worksheets[1];
                    ws.Name = "Inventario Completo";

                    DataRow[] todasLasFilas = dtInventario.Select();
                    VolcarDatosAExcel(ws, dtInventario.Columns, todasLasFilas);
                }
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void VolcarDatosAExcel(Worksheet ws, DataColumnCollection columnas, DataRow[] filas)
        {
            int colIndex = 1;
            foreach (DataColumn col in columnas)
            {
                ws.Cells[1, colIndex] = col.ColumnName;
                colIndex++;
            }

            Range headerRange = ws.Range[ws.Cells[1, 1], ws.Cells[1, columnas.Count]];
            headerRange.Font.Bold = true;
            headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.LightGray);

            if (filas.Length > 0)
            {
                object[,] datosMatriz = new object[filas.Length, columnas.Count];

                for (int r = 0; r < filas.Length; r++)
                {
                    for (int c = 0; c < columnas.Count; c++)
                    {
                        if (filas[r][c] == DBNull.Value)
                            datosMatriz[r, c] = "";
                        else
                            datosMatriz[r, c] = filas[r][c].ToString();
                    }
                }

                Range startCell = ws.Cells[2, 1];
                Range endCell = ws.Cells[1 + filas.Length, columnas.Count];
                Range writeRange = ws.Range[startCell, endCell];

                writeRange.Value2 = datosMatriz;
            }

            ws.Columns.AutoFit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listaCompletaInventario == null || listaCompletaInventario.Count == 0)
            {
                MessageBox.Show("No hay datos de inventario para procesar.", "Aviso",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var resumen = listaCompletaInventario
                .GroupBy(p => p.Categoria)
                .Select(grupo => new ResumenCategoria
                {
                    Categoria = grupo.Key,
                    TotalExistencia = grupo.Sum(p => p.Existencia),
                    TotalVenta = grupo.Sum(p => p.Existencia * p.PrecioVenta),
                    TotalInversion = grupo.Sum(p => p.Existencia * p.Especial),
                    Utilidad = grupo.Sum(p => (p.Existencia * p.PrecioVenta) - (p.Existencia * p.Especial))
                })
                .OrderBy(r => r.Categoria)
                .ToList();
            frmDatos formularioResumen = new frmDatos(resumen);
            formularioResumen.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView2.CurrentRow != null)
                {
                    ProductoInventario prod = (ProductoInventario)dataGridView2.CurrentRow.DataBoundItem;
                    frmKardex kar = new frmKardex();
                    kar.lblProducto.Text = prod.Nombre;
                    kar.idProducto = prod.Id;
                    kar.Show();
                    this.Close();
                }
            }
            catch (Exception)
            {
                // Unhandled
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            frmPolizas comp = new frmPolizas();
            comp.usuario = usuario;
            comp.Show();
            this.Close();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            frmGastos gas = new frmGastos();
            gas.Show();
            this.Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listaFiltradaInventario == null) return;

            foreach (var prod in listaFiltradaInventario)
            {
                cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + prod.Existencia.ToString() + "' where Id='" + prod.Id + "';", conectar);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Se han guardado los cambios correctamente!", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            frmUnidades UN = new frmUnidades();
            UN.Show();
            this.Close();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            frmCategorias es = new frmCategorias();
            es.Show();
            this.Close();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            frmSuspendidos susp = new frmSuspendidos();
            susp.Show();
            this.Close();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (listaFiltradaInventario == null) return;

            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;

            xla.Visible = true;

            ws.Cells[1, 1] = "ID";
            ws.Cells[1, 2] = "Nombre";
            ws.Cells[1, 3] = "Existencia";
            ws.Cells[1, 4] = "Limite";
            ws.Cells[1, 5] = "Fecha: " + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            int cont = 1;

            foreach (var prod in listaFiltradaInventario)
            {
                if (prod.Existencia <= prod.Limite)
                {
                    cont++;
                    ws.Cells[cont, 1] = prod.Id;
                    ws.Cells[cont, 2] = prod.Nombre;
                    ws.Cells[cont, 3] = prod.Existencia.ToString();
                    ws.Cells[cont, 4] = prod.Limite.ToString();
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Desea imprimir un reporte para capturar el inventario fisico?", "Alto!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
                    Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
                    Worksheet ws = (Worksheet)xla.ActiveSheet;

                    ws.Cells[1, 1] = "ID";
                    ws.Cells[1, 2] = "Nombre";
                    ws.Cells[1, 3] = "Existen";
                    ws.Cells[1, 4] = "Fisico";

                    ws.Range["A1", "D1"].Font.Bold = true;

                    int cont = 1;

                    if (listaCompletaInventario != null)
                    {
                        foreach (var producto in listaCompletaInventario)
                        {
                            cont++;
                            ws.Cells[cont, 1] = producto.Id;
                            ws.Cells[cont, 2] = producto.Nombre;
                            ws.Cells[cont, 3] = producto.Existencia.ToString();
                        }
                    }

                    ws.Columns.AutoFit();
                    xla.Visible = true;

                    frmInventariosFisicos fis = new frmInventariosFisicos();
                    fis.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar el reporte de Excel: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                using (this.Cursor = Cursors.Default) { }
            }
            else
            {
                frmInventariosFisicos fis = new frmInventariosFisicos();
                fis.Show();
                this.Close();
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (listaFiltradaInventario == null) return;

            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;

            xla.Visible = true;
            Microsoft.Office.Interop.Excel.Range formatRange;
            formatRange = ws.get_Range("A:A", System.Type.Missing);
            formatRange.NumberFormat = "####";
            formatRange.EntireColumn.ColumnWidth = 17;

            formatRange = ws.get_Range("B:B", System.Type.Missing);
            formatRange.EntireColumn.ColumnWidth = 30;

            formatRange = ws.get_Range("a1", "j1");
            formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
            formatRange.EntireRow.Font.Bold = true;

            ws.Cells[1, 1] = "ID";
            ws.Cells[1, 2] = "Nombre";
            ws.Cells[1, 3] = "Precio de Venta";
            ws.Cells[1, 4] = "Existencia";
            ws.Cells[1, 5] = "Limite";
            ws.Cells[1, 6] = "Categoria";
            ws.Cells[1, 7] = "Precio de compra";
            ws.Cells[1, 8] = "Unidad";
            ws.Cells[1, 9] = "Fecha: " + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            int cont = 1;

            foreach (var prod in listaFiltradaInventario)
            {
                if (prod.Existencia <= prod.Limite)
                {
                    cont++;
                    ws.Cells[cont, 1] = prod.Id;
                    ws.Cells[cont, 2] = prod.Nombre;
                    ws.Cells[cont, 3] = prod.PrecioVenta.ToString();
                    ws.Cells[cont, 4] = prod.Existencia.ToString();
                    ws.Cells[cont, 5] = prod.Limite.ToString();
                    ws.Cells[cont, 6] = prod.Categoria;
                    ws.Cells[cont, 7] = prod.Especial.ToString();
                    ws.Cells[cont, 8] = prod.Uni;
                }
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            frmCaptura cap = new frmCaptura();
            cap.Show();
            this.Close();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            frmCompras2 COM = new frmCompras2();
            COM.Show();
            this.Close();
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name == "PrecioVenta" ||
              dataGridView2.Columns[e.ColumnIndex].Name == "PrecioVentaMayoreo" ||
              dataGridView2.Columns[e.ColumnIndex].Name == "Especial")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
            else if (dataGridView2.Columns[e.ColumnIndex].Name == "EstadoInventario")
            {
                if (e.Value != null)
                {
                    string val = e.Value.ToString();
                    if (val == "COMPRAR")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                        e.CellStyle.SelectionForeColor = Color.FromArgb(231, 76, 60);
                    }
                    else if (val == "SOBRESTOCK")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(230, 126, 34);
                        e.CellStyle.SelectionForeColor = Color.FromArgb(230, 126, 34);
                    }
                    else if (val == "NORMAL")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                        e.CellStyle.SelectionForeColor = Color.FromArgb(46, 204, 113);
                    }
                }
            }
            else if (dataGridView2.Columns[e.ColumnIndex].Name == "NivelRotacion")
            {
                if (e.Value != null)
                {
                    string val = e.Value.ToString();
                    if (val == "ALTA ROTACIÓN")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                        e.CellStyle.SelectionForeColor = Color.FromArgb(46, 204, 113);
                    }
                    else if (val == "MEDIA")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                        e.CellStyle.SelectionForeColor = Color.FromArgb(241, 196, 15);
                    }
                    else if (val == "BAJA")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                        e.CellStyle.SelectionForeColor = Color.FromArgb(231, 76, 60);
                    }
                }
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            frmTarjeta formularioSecundario = new frmTarjeta();
            formularioSecundario.ShowDialog();
        }

        private void frmInventario_Shown(object sender, EventArgs e)
        {
            ActualizarTamanoPagina();
            if (tamanoPagina <= 0)
            {
                tamanoPagina = 20;
            }

            if (listaFiltradaInventario != null && listaFiltradaInventario.Count > 0 && tamanoPagina > 0)
            {
                totalPaginas = (int)Math.Ceiling((double)listaFiltradaInventario.Count / tamanoPagina);
            }
            else
            {
                totalPaginas = 1;
            }

            CargarPagina();
        }

        private void ActualizarTamanoPagina()
        {
            int altoFila = dataGridView2.RowTemplate.Height;
            if (altoFila <= 0) { return; }

            int altoDisponible = dataGridView2.ClientSize.Height;

            if (dataGridView2.ColumnHeadersVisible)
            {
                altoDisponible = altoDisponible - dataGridView2.ColumnHeadersHeight;
            }

            int nuevoTamano = altoDisponible / altoFila;

            if (nuevoTamano > 0)
            {
                tamanoPagina = nuevoTamano;
            }
        }

        private void CargarVentasDinamicas(List<ProductoInventario> productos)
        {
            if (productos == null || productos.Count == 0) return;

            foreach (var p in productos)
            {
                p.VentasMes = 0;
                p.VentasAnio = 0;
            }

            try
            {
                string query = @"
                    SELECT IdProducto, 
                           SUM(IIF(Month(Fecha) = Month(Date()) AND Year(Fecha) = Year(Date()), CantidadDouble, 0)) AS VentasMes, 
                           SUM(IIF(Year(Fecha) = Year(Date()), CantidadDouble, 0)) AS VentasAnio
                    FROM (
                      SELECT IdProducto, Cantidad AS CantidadDouble, Fecha FROM VentasContado WHERE FolioVenta IN (SELECT Folio FROM Ventas WHERE Estatus <> 'CANCELADO')
                      UNION ALL
                      SELECT IdProducto, CInt(Cantidad) AS CantidadDouble, Fecha FROM VentasCredito WHERE FolioVenta IN (SELECT Folio FROM Ventas2 WHERE Estatus <> 'CANCELADO')
                      UNION ALL
                      SELECT IdProducto, Val(Cantidad) AS CantidadDouble, Fecha FROM VentasApartados WHERE Estatus <> 'CANCELADO'
                    ) AS Combined
                    GROUP BY IdProducto
                ";

                bool wasClosed = (conectar.State == ConnectionState.Closed);
                if (wasClosed) conectar.Open();

                using (OleDbCommand cmdSales = new OleDbCommand(query, conectar))
                {
                    using (OleDbDataReader reader = cmdSales.ExecuteReader())
                    {
                        var tempDict = new Dictionary<string, (double mes, double anio)>(StringComparer.OrdinalIgnoreCase);

                        while (reader.Read())
                        {
                            string idProd = reader["IdProducto"] == DBNull.Value ? "" : reader["IdProducto"].ToString().Trim();
                            if (string.IsNullOrEmpty(idProd)) continue;

                            double vMes = reader["VentasMes"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["VentasMes"]);
                            double vAnio = reader["VentasAnio"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["VentasAnio"]);

                            tempDict[idProd] = (vMes, vAnio);
                        }

                        foreach (var p in productos)
                        {
                            if (p.Id != null && tempDict.TryGetValue(p.Id.Trim(), out var sales))
                            {
                                p.VentasMes = sales.mes;
                                p.VentasAnio = sales.anio;
                            }
                        }
                    }
                }

                if (wasClosed) conectar.Close();
            }
            catch (Exception)
            {
                // Silently handle
            }
        }

        private void ActualizarKpiValues()
        {
            if (listaCompletaInventario == null || !Sesion.TienePermiso("VER_ANALITICO_INVENTARIO")) return;

            int totalArticulos = listaCompletaInventario.Count;
            decimal totalStock = listaCompletaInventario.Sum(p => p.Existencia);
            int totalCategorias = listaCompletaInventario.Select(p => p.Categoria).Distinct().Count();

            if (lblKpiArticulos != null) lblKpiArticulos.Text = totalArticulos.ToString();
            if (lblKpiStock != null) lblKpiStock.Text = totalStock.ToString("N0");
            if (lblKpiCategorias != null) lblKpiCategorias.Text = totalCategorias.ToString();

            decimal inversion = listaCompletaInventario.Sum(p => p.Existencia * p.Especial);
            if (lblKpiInversion != null) lblKpiInversion.Text = inversion.ToString("C2");
            if (lblFinancieroInversion != null) lblFinancieroInversion.Text = "Inversión: " + inversion.ToString("C2");

            decimal ventaPotencial = listaCompletaInventario.Sum(p => p.Existencia * p.PrecioVenta);
            if (lblKpiVentaPotencial != null) lblKpiVentaPotencial.Text = ventaPotencial.ToString("C2");
            if (lblFinancieroVenta != null) lblFinancieroVenta.Text = "Venta Potencial: " + ventaPotencial.ToString("C2");

            decimal ganancia = listaCompletaInventario.Sum(p => (p.PrecioVenta - p.Especial) * p.Existencia);
            if (lblKpiUtilidad != null) lblKpiUtilidad.Text = ganancia.ToString("C2");
            if (lblFinancieroUtilidad != null) lblFinancieroUtilidad.Text = "Utilidad Esperada: " + ganancia.ToString("C2");

            double margen = inversion > 0 ? (double)(ganancia / inversion) * 100 : 0.0;
            if (lblKpiMargen != null) lblKpiMargen.Text = margen.ToString("F1") + "%";
            if (lblFinancieroMargen != null) lblFinancieroMargen.Text = "Margen Promedio: " + margen.ToString("F2") + "%";

            int stockCriticosCount = listaCompletaInventario.Count(p => p.Existencia <= p.Limite);
            if (lblKpiStockCritico != null)
            {
                lblKpiStockCritico.Text = stockCriticosCount.ToString();
                if (stockCriticosCount == 0) lblKpiStockCritico.ForeColor = Color.FromArgb(46, 204, 113);
                else if (stockCriticosCount <= 10) lblKpiStockCritico.ForeColor = Color.FromArgb(241, 196, 15);
                else lblKpiStockCritico.ForeColor = Color.FromArgb(231, 76, 60);
            }
            if (lblRiesgoCriticos != null) lblRiesgoCriticos.Text = "Stock Crítico: " + stockCriticosCount + " Prod.";

            decimal capitalInmovilizado = listaCompletaInventario
                .Where(p => p.NivelRotacion == "BAJA")
                .Sum(p => p.Existencia * p.Especial);
            if (lblKpiCapitalInmovilizado != null) lblKpiCapitalInmovilizado.Text = capitalInmovilizado.ToString("C2");
            if (lblFinancieroInmovilizado != null) lblFinancieroInmovilizado.Text = "Capital Inmovilizado: " + capitalInmovilizado.ToString("C2");

            int sobrestockCount = listaCompletaInventario.Count(p => p.StockMaximo > 0 && p.Existencia > (decimal)p.StockMaximo);
            if (lblRiesgoSobrestock != null) lblRiesgoSobrestock.Text = "Sobrestock: " + sobrestockCount + " Prod.";

            int bajaRotacionCount = listaCompletaInventario.Count(p => p.NivelRotacion == "BAJA");
            if (lblRiesgoRotacion != null) lblRiesgoRotacion.Text = "Baja Rotación: " + bajaRotacionCount + " Prod.";

            int reposicionProxCount = listaCompletaInventario.Count(p =>
                p.Existencia > p.Limite &&
                p.VentasMes > 0 &&
                p.DiasReposicion > 0 &&
                ((double)p.Existencia / (p.VentasMes / 30.0)) <= p.DiasReposicion
            );
            if (lblRiesgoReposicion != null) lblRiesgoReposicion.Text = "Reposición Próx.: " + reposicionProxCount + " Prod.";
        }

        private void DataGridView2_DataBindingComplete(object sender, System.Windows.Forms.DataGridViewBindingCompleteEventArgs e)
        {
            if (dataGridView2.Columns.Count > 0)
            {
                dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;

                if (dataGridView2.Columns.Contains("Id"))
                {
                    dataGridView2.Columns["Id"].Visible = true;
                    dataGridView2.Columns["Id"].DisplayIndex = 0;
                }
                if (dataGridView2.Columns.Contains("Nombre"))
                {
                    dataGridView2.Columns["Nombre"].Visible = true;
                    dataGridView2.Columns["Nombre"].DisplayIndex = 1;
                    dataGridView2.Columns["Nombre"].MinimumWidth = 250;
                }

                foreach (System.Windows.Forms.DataGridViewColumn col in dataGridView2.Columns)
                {
                    if (col.Name == "Activo")
                    {
                        col.Visible = false;
                        continue;
                    }

                    col.HeaderCell.Style.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;

                    if (col.Name == "PrecioVenta" || col.Name == "PrecioVentaMayoreo" || col.Name == "Especial" || col.Name == "StockMaximo")
                    {
                        col.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
                        col.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else if (col.Name == "Existencia" || col.Name == "Limite" || col.Name == "DiasReposicion" || col.Name == "VentasMes" || col.Name == "VentasAnio")
                    {
                        col.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
                        col.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else if (col.Name == "Id" || col.Name == "EstadoInventario" || col.Name == "NivelRotacion" || col.Name == "FechaUltimaCompra" || col.Name == "FechaUltimaVenta")
                    {
                        col.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                        col.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else if (col.Name == "Nombre")
                    {
                        col.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
                        col.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
                    }
                    else
                    {
                        col.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
                        col.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                    }

                    switch (col.Name)
                    {
                        case "Id": col.HeaderText = "ID/Código"; break;
                        case "Nombre": col.HeaderText = "Descripción del Producto"; break;
                        case "PrecioVentaMayoreo": col.HeaderText = "Precio Mayoreo"; break;
                        case "PrecioVenta": col.HeaderText = "Precio Público"; break;
                        case "Existencia": col.HeaderText = "Stock Actual"; break;
                        case "Limite": col.HeaderText = "Mínimo/Límite"; break;
                        case "Categoria": col.HeaderText = "Categoría"; break;
                        case "Especial": col.HeaderText = "Precio Compra"; break;
                        case "IVA": col.HeaderText = "Impuesto"; break;
                        case "Unidad": col.HeaderText = "Unidad Medida"; break;
                        case "Uni": col.HeaderText = "Abrev."; break;
                        case "ProveedorPrincipal": col.HeaderText = "Proveedor Principal"; break;
                        case "FechaUltimaCompra": col.HeaderText = "Última Compra"; break;
                        case "FechaUltimaVenta": col.HeaderText = "Última Venta"; break;
                        case "StockMaximo": col.HeaderText = "Stock Máximo"; break;
                        case "DiasReposicion": col.HeaderText = "Días Reposición"; break;
                        case "EstadoInventario": col.HeaderText = "Estado Inventario"; break;
                        case "NivelRotacion": col.HeaderText = "Nivel Rotación"; break;
                    }
                }
            }
        }

        private void EstilizarBotonPaginador(System.Windows.Forms.Button btn)
        {
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            btn.FlatAppearance.BorderSize = 1;
            btn.Cursor = System.Windows.Forms.Cursors.Hand;
            btn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            System.EventHandler enabledHandler = null;
            enabledHandler = (s, e) =>
            {
                if (btn.Enabled)
                {
                    btn.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
                    btn.ForeColor = System.Drawing.Color.White;
                    btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(70, 70, 70);
                    btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(60, 60, 60);
                    btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(30, 30, 30);
                }
                else
                {
                    btn.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
                    btn.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
                    btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(40, 40, 40);
                    btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(25, 25, 25);
                    btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(25, 25, 25);
                }
            };

            btn.EnabledChanged += enabledHandler;
            enabledHandler(btn, EventArgs.Empty);
        }

        private void btnAlmacenes_Click(object sender, EventArgs e)
        {
            frmAlmacenes almacenes = new frmAlmacenes();
            almacenes.ShowDialog();
        }
    }
}