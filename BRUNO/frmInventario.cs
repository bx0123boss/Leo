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
            public decimal Especial {  get; set; }
            public string IVA { get; set; }
            public string Unidad { get; set; }
            public string Uni { get; set; }
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
            this.MinimumSize = new Size(1346,805);
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
            //ds = new DataSet();
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
            da.Fill(ds, "Inventario"); // Usamos "Inventario" como nombre de tabla

            // 2. Convertir el DataSet (DataTable) a una List<ProductoInventario>
            listaCompletaInventario = new List<ProductoInventario>();

            // Verificamos que la tabla se haya cargado
            if (ds.Tables.Count > 0 && ds.Tables["Inventario"].Rows.Count > 0)
            {
                // Iteramos por cada fila (DataRow) en la tabla
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

                    listaCompletaInventario.Add(prod);
                }
            }
            listaFiltradaInventario = new List<ProductoInventario>(listaCompletaInventario); 
            totalPaginas = (int)Math.Ceiling((double)listaCompletaInventario.Count / tamanoPagina);

            // 3. Actualizar valores de KPIs
            ActualizarKpiValues();

            // 4. Cargar la primera página
            CargarPagina();
        }
        #region PAGINACION
        private void CargarPagina()
        {
            // (Validación de paginaActual - sin cambios)
            if (paginaActual < 1) paginaActual = 1;
            if (paginaActual > totalPaginas && totalPaginas > 0) paginaActual = totalPaginas;

            var datosPaginados = listaFiltradaInventario // ¡Usamos la filtrada!
                .Skip((paginaActual - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            // Asignar al DataGridView (sin cambios)
            dataGridView2.DataSource = datosPaginados;
            totalPaginas = (int)Math.Ceiling((double)listaFiltradaInventario.Count / tamanoPagina);
            // Actualizar controles (sin cambios)
            ActualizarControlesPaginacion();
        }
        private void ActualizarControlesPaginacion()
        {
            // Actualizar etiqueta
            lblEstado.Text = $"Página {paginaActual} de {totalPaginas}";

            // Habilitar o deshabilitar botones según la página actual
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
            // CAMBIO 1: Validamos contra la lista FILTRADA
            if (listaFiltradaInventario == null || listaFiltradaInventario.Count == 0)
            {
                dataGridView2.DataSource = null;
                totalPaginas = 1;
                ActualizarControlesNavegacion();
                return;
            }

            // CAMBIO 2: Calculamos las páginas según los resultados FILTRADOS
            totalPaginas = (int)Math.Ceiling((double)listaFiltradaInventario.Count / tamanoPagina);

            // Asegurarse de que la página actual esté dentro de los límites
            if (paginaActual > totalPaginas)
            {
                paginaActual = totalPaginas;
            }
            if (paginaActual < 1)
            {
                paginaActual = 1;
            }

            // CAMBIO 3: Hacemos el Skip y Take sobre la lista FILTRADA
            var datosPaginados = listaFiltradaInventario
                .Skip((paginaActual - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            // Asignar los datos paginados al DataGridView
            dataGridView2.DataSource = datosPaginados;

            // Actualizar los botones y la etiqueta
            ActualizarControlesNavegacion();
        }
        private void ActualizarControlesNavegacion()
        {
            // Actualizar la etiqueta de estado
            lblEstado.Text = $"Página {paginaActual} de {totalPaginas}";

            // Habilitar/Deshabilitar botones
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
               
                    frmEditarProducto editar = new frmEditarProducto();
                    editar.usuario = usuario;
                    editar.inventario = "INVENT";
                    editar.txtID.Text = dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.txtProducto.Text = dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.txtCompra.Text = dataGridView2[2, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.txtVenta.Text = dataGridView2[3, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.cmbCategoria.Text = dataGridView2[8, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.txtLimite.Text = dataGridView2[7, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.comboBox1.Text = dataGridView2[6, dataGridView2.CurrentRow.Index].Value.ToString();
                    System.Data.DataTable dt = new System.Data.DataTable();
                    cmd = new OleDbCommand("Select Id,Nombre from Unidades;", conectar);
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);
                    editar.cmbUnidad.DisplayMember = "Nombre";
                    editar.cmbUnidad.ValueMember = "Id";
                    editar.cmbUnidad.DataSource = dt;
                    editar.cmbUnidad.SelectedValue = dataGridView2[9, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.cmbUnidad.Text = dataGridView2[10, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.Show();
                    this.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Verificar que haya una fila seleccionada
            if (dataGridView2.CurrentRow == null)
            {
                MessageBox.Show("Por favor, seleccione un producto para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Obtener el producto seleccionado de forma segura
            // Usamos DataBoundItem para obtener el objeto 'ProductoInventario' completo
            ProductoInventario productoSeleccionado = (ProductoInventario)dataGridView2.CurrentRow.DataBoundItem;
            string idParaEliminar = productoSeleccionado.Id;

            try
            {
                // 3. Confirmación
                DialogResult dialogResult = MessageBox.Show("¿Estas seguro de eliminar el producto?", "Alto!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    string sqlInsert = @"INSERT INTO InventarioSusp 
                (Id, Nombre, PrecioVentaMayoreo, PrecioVenta, Existencia, Limite, Categoria, Especial, IVA, Unidad, Uni) 
                VALUES 
                (@Id, @Nombre, @PrecioVentaMayoreo, @PrecioVenta, @Existencia, @Limite, @Categoria, @Especial, @IVA, @Unidad, @Uni)";

                    using (OleDbCommand cmdInsert = new OleDbCommand(sqlInsert, conectar))
                    {
                        // Asignamos los valores desde el objeto que ya tenemos
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

                    // 4b. Eliminar de la tabla principal (Inventario)
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

                    ActualizarKpiValues();

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

            // Empezamos asumiendo que mostramos todo
            var filtro = listaCompletaInventario.AsEnumerable();

            // Filtro por Categoría (Combo y Check)
            if (checkBox1.Checked)
            {
                filtro = filtro.Where(p => p.Categoria != null &&
                                           p.Categoria == comboBox2.Text &&
                                           p.Categoria != "ACCESORIOS");
            }

            // Filtro por Nombre (TextBox1)
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                filtro = filtro.Where(p => p.Nombre != null &&
                                           p.Nombre.IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // Filtro por ID (TextBox2)
            if (!string.IsNullOrWhiteSpace(textBox2.Text))
            {
                filtro = filtro.Where(p => p.Id != null &&
                                           p.Id.IndexOf(textBox2.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // Actualizamos la vista y regresamos a la página 1
            listaFiltradaInventario = filtro.ToList();
            paginaActual = 1;
            CargarDatosPaginados();
        }

        // Ahora, enlazamos los controles a este método:
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                AplicarFiltros();
                e.Handled = true; // Quita el sonido molesto de "beep" de Windows
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

                // Limpiar hojas extra
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
                            // Crear/Seleccionar Hoja
                            Worksheet ws;
                            if (workbook.Worksheets.Count == 1 && ((Worksheet)workbook.Worksheets[1]).Name.Contains("Hoja"))
                                ws = (Worksheet)workbook.Worksheets[1];
                            else
                                ws = (Worksheet)workbook.Worksheets.Add(After: workbook.Worksheets[workbook.Worksheets.Count]);

                            // Nombre seguro para la hoja
                            string nombreHoja = string.IsNullOrEmpty(categoria) ? "Sin Categoria" : categoria;
                            nombreHoja = nombreHoja.Replace("/", "-").Replace("*", "").Replace("?", "").Replace("[", "").Replace("]", "").Replace(":", "");
                            if (nombreHoja.Length > 30) nombreHoja = nombreHoja.Substring(0, 30);
                            try { ws.Name = nombreHoja; } catch { }

                            VolcarDatosAExcel(ws, dtInventario.Columns, filas);
                        }
                    }
                }
                // ==========================================================================================
                // OPCIÓN B: TODO EN UNA SOLA HOJA (Respuesta = NO)
                // ==========================================================================================
                else if (respuesta == DialogResult.No)
                {
                    Worksheet ws = (Worksheet)workbook.Worksheets[1];
                    ws.Name = "Inventario Completo";

                    // Convertimos todas las filas del DataTable a un arreglo DataRow[] para usar el mismo método helper
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

        /// <summary>
        /// Método auxiliar para escribir encabezados y datos en una hoja dada (Evita repetir código).
        /// </summary>
        private void VolcarDatosAExcel(Worksheet ws, DataColumnCollection columnas, DataRow[] filas)
        {
            // 1. Encabezados
            int colIndex = 1;
            foreach (DataColumn col in columnas)
            {
                ws.Cells[1, colIndex] = col.ColumnName;
                colIndex++;
            }

            // Formato Encabezados
            Range headerRange = ws.Range[ws.Cells[1, 1], ws.Cells[1, columnas.Count]];
            headerRange.Font.Bold = true;
            headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.LightGray);

            // 2. Datos (Optimizados con Matriz)
            if (filas.Length > 0)
            {
                object[,] datosMatriz = new object[filas.Length, columnas.Count];

                for (int r = 0; r < filas.Length; r++)
                {
                    for (int c = 0; c < columnas.Count; c++)
                    {
                        // Importante: Convertir a String o manejar tipos para evitar errores de COM
                        // Excel a veces se confunde con DBNull
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

            // 3. Ajustar Ancho
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
                frmKardex kar = new frmKardex();
                kar.lblProducto.Text = dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString();
                kar.idProducto = dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString();
                kar.Show();
                this.Close();
            }
            catch (Exception)
            {

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
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + dataGridView2[4, i].Value.ToString() + "' where Id='" + dataGridView2[0, i].Value.ToString() + "';", conectar);
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
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {

                if (Convert.ToDouble(dataGridView2[4, i].Value.ToString()) <= Convert.ToDouble(dataGridView2[5, i].Value.ToString()))
                {
                    cont++;
                    ws.Cells[cont, 1] = dataGridView2[0, i].Value.ToString();
                    ws.Cells[cont, 2] = dataGridView2[1, i].Value.ToString();
                    ws.Cells[cont, 3] = dataGridView2[4, i].Value.ToString();
                    ws.Cells[cont, 4] = dataGridView2[5, i].Value.ToString();

                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Desea imprimir un reporte para capturar el inventario fisico?", "Alto!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                // Cambiamos el cursor para que el usuario sepa que está cargando
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

                    // Poner en negrita los encabezados (opcional, para mejor presentación)
                    ws.Range["A1", "D1"].Font.Bold = true;

                    int cont = 1;

                    // Verificamos que la lista no esté vacía
                    if (listaCompletaInventario != null)
                    {
                        // Recorremos la lista completa en memoria en lugar del DataGridView paginado
                        foreach (var producto in listaCompletaInventario)
                        {
                            cont++;
                            ws.Cells[cont, 1] = producto.Id;
                            ws.Cells[cont, 2] = producto.Nombre;
                            ws.Cells[cont, 3] = producto.Existencia.ToString();
                        }
                    }

                    // Autoajustar el ancho de las columnas
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
                finally
                {
                    this.Cursor = Cursors.Default; // Regresamos el cursor a la normalidad
                }
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
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;
            
            xla.Visible = true;
            Microsoft.Office.Interop.Excel.Range formatRange;
            formatRange = ws.get_Range("A:A",System.Type.Missing);
            formatRange.NumberFormat = "####";
            formatRange.EntireColumn.ColumnWidth = 17;
            
            formatRange = ws.get_Range("B:B",System.Type.Missing);
            formatRange.EntireColumn.ColumnWidth = 30;

            formatRange = ws.get_Range("a1", "j1"); 
            formatRange.Interior.Color = System.Drawing.
            ColorTranslator.ToOle(System.Drawing.Color.Yellow);
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
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {

                if (Convert.ToDouble(dataGridView2[4, i].Value.ToString()) <= Convert.ToDouble(dataGridView2[5, i].Value.ToString()))
                {
                    cont++;
                    ws.Cells[cont, 1] = dataGridView2[0, i].Value.ToString();
                    ws.Cells[cont, 2] = dataGridView2[1, i].Value.ToString();
                    ws.Cells[cont, 3] = dataGridView2[3, i].Value.ToString();
                    ws.Cells[cont, 4] = dataGridView2[4, i].Value.ToString();
                    ws.Cells[cont, 5] = dataGridView2[5, i].Value.ToString();
                    ws.Cells[cont, 6] = dataGridView2[6, i].Value.ToString();
                    ws.Cells[cont, 7] = dataGridView2[7, i].Value.ToString();
                    ws.Cells[cont, 8] = dataGridView2[10, i].Value.ToString();

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
                    e.Value = value.ToString("C2"); // Formato moneda con 2 decimales
                    e.FormattingApplied = true;
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
                tamanoPagina = 20; // Un valor seguro por si acaso
            }

            // CAMBIO 4: Calculamos las páginas base a la lista FILTRADA al mostrar el form
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
            // 1. Obtener el alto de UNA fila. 
            // RowTemplate.Height ya sabe el alto correcto basado en tu fuente de 12px y el padding.
            int altoFila = dataGridView2.RowTemplate.Height;

            // Evitar división por cero si el grid está oculto o la plantilla no tiene alto
            if (altoFila <= 0) { return; }

            // 2. Obtener el alto DISPONIBLE para las filas
            // Usamos ClientSize.Height para obtener el alto interno (sin bordes)
            int altoDisponible = dataGridView2.ClientSize.Height;

            // 3. Restar el alto de la cabecera (Column Headers) si está visible
            if (dataGridView2.ColumnHeadersVisible)
            {
                altoDisponible = altoDisponible - dataGridView2.ColumnHeadersHeight;
            }

            // 4. Calcular el nuevo tamaño de página
            int nuevoTamano = altoDisponible / altoFila;

            // 5. Actualizar la variable global (solo si es un valor válido)
            if (nuevoTamano > 0)
            {
                tamanoPagina = nuevoTamano;
            }
        }

        // =====================================================================
        // ELEMENTOS DE DISEÑO PREMIUM (DASHBOARD REDESIGN)
        // =====================================================================
        private void ActualizarKpiValues()
        {
            if (listaCompletaInventario == null) return;

            int totalArticulos = listaCompletaInventario.Count;
            decimal totalStock = listaCompletaInventario.Sum(p => p.Existencia);
            int totalCategorias = listaCompletaInventario.Select(p => p.Categoria).Distinct().Count();

            if (lblKpiArticulos != null) lblKpiArticulos.Text = totalArticulos.ToString();
            if (lblKpiStock != null) lblKpiStock.Text = totalStock.ToString("N0", CultureInfo.InvariantCulture);
            if (lblKpiCategorias != null) lblKpiCategorias.Text = totalCategorias.ToString();
        }



        private void DataGridView2_DataBindingComplete(object sender, System.Windows.Forms.DataGridViewBindingCompleteEventArgs e)
        {
            if (dataGridView2.Columns.Count > 0)
            {
                dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;

                foreach (System.Windows.Forms.DataGridViewColumn col in dataGridView2.Columns)
                {
                    col.HeaderCell.Style.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                    
                    if (col.Name == "PrecioVenta" || col.Name == "PrecioVentaMayoreo" || col.Name == "Especial")
                    {
                        col.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
                        col.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else if (col.Name == "Existencia" || col.Name == "Limite")
                    {
                        col.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
                        col.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else if (col.Name == "Id")
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
                        case "Especial": col.HeaderText = "Precio Especial"; break;
                        case "IVA": col.HeaderText = "Impuesto"; break;
                        case "Unidad": col.HeaderText = "Unidad Medida"; break;
                        case "Uni": col.HeaderText = "Abrev."; break;
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
            // Ejecutar la primera vez
            enabledHandler(btn, EventArgs.Empty);
        }
    }
}
