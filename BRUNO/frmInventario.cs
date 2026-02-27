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

namespace BRUNO
{
    public partial class frmInventario : frmBase
    {

        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        //public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Jaeger Soft\Joyeria.accdb";
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbCommand cmd;
        OleDbDataAdapter da;
        public String usuario = "Admin";
        bool sinllenar = true;


        private List<ProductoInventario> listaCompletaInventario;
        private int tamanoPagina = 30; // ¿Cuántos registros por página?
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
            EstilizarDataGridView(this.dataGridView2);
            EstilizarTextBox(this.textBox1);
            EstilizarTextBox(this.textBox2);

            EstilizarBotonPrimario(this.button6);
            EstilizarBotonPrimario(this.button8);
            EstilizarBotonPrimario(this.button7);
            EstilizarBotonPrimario(this.button13);
            EstilizarBotonPrimario(this.button10);
            EstilizarBotonPrimario(this.button4);
            EstilizarBotonPrimario(this.button11);
            EstilizarBotonPrimario(this.btnPrimero);
            EstilizarBotonPrimario(this.btnAnterior);
            EstilizarBotonPrimario(this.btnSiguiente);
            EstilizarBotonPrimario(this.button14);
            EstilizarBotonPrimario(this.btnUltimo);
            EstilizarBotonPrimario(this.BtnApartados);    // Botón "Agregar"
            EstilizarBotonPeligro(this.button1);
            EstilizarBotonPeligro(this.button12);  // Botón "Eliminar"
            EstilizarBotonAdvertencia(this.button2);
            EstilizarBotonAdvertencia(this.button18);// Botón "Editar Contraseña"
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
            totalPaginas = (int)Math.Ceiling((double)listaCompletaInventario.Count / tamanoPagina);

            // 4. Cargar la primera página
            CargarPagina();
        }
        #region PAGINACION
        private void CargarPagina()
        {
            // (Validación de paginaActual - sin cambios)
            if (paginaActual < 1) paginaActual = 1;
            if (paginaActual > totalPaginas && totalPaginas > 0) paginaActual = totalPaginas;

            // Usar LINQ (Skip y Take) sobre TU lista de inventario
            var datosPaginados = listaCompletaInventario
                .Skip((paginaActual - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            // Asignar al DataGridView (sin cambios)
            dataGridView2.DataSource = datosPaginados;

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
            // Asegúrate de que 'listaCompleta' tenga datos
            if (listaCompletaInventario == null || listaCompletaInventario.Count == 0)
            {
                dataGridView2.DataSource = null;
                ActualizarControlesNavegacion();
                return;
            }

            // Calcular el total de páginas
            // Usamos Math.Ceiling para redondear hacia arriba (ej. 105 items / 10 por pag = 10.5 -> 11 páginas)
            totalPaginas = (int)Math.Ceiling((double)listaCompletaInventario.Count / tamanoPagina);

            // Asegurarse de que la página actual esté dentro de los límites
            if (paginaActual > totalPaginas)
            {
                paginaActual = totalPaginas;
            }
            if (paginaActual < 1)
            {
                paginaActual = 1;
            }

            // Usar LINQ (Skip y Take) para obtener solo los registros de la página actual
            // Skip: Omite los registros de las páginas anteriores
            // Take: Toma el número de registros para la página actual
            var datosPaginados = listaCompletaInventario
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
                    }

                    paginaActual = 1;
                    CargarDatosPaginados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='" + comboBox2.Text + "'order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[7].HeaderText = "Precio Compra";
                dataGridView2.Columns[10].HeaderText = "Unidad";
                dataGridView2.Columns[9].Visible = false;
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                
                //dataGridView2.Columns[0].Visible = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox1.Enabled = false;
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='" + comboBox2.Text + "' and not Categoria='ACCESORIOS' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[7].HeaderText = "Precio Compra";
                dataGridView2.Columns[10].HeaderText = "Unidad";
                dataGridView2.Columns[9].Visible = false;
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                textBox1.Enabled = true;
                comboBox2.SelectedIndex = 0;
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where not Categoria='ACCESORIOS' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[7].HeaderText = "Precio Compra";
                dataGridView2.Columns[10].HeaderText = "Unidad";
                dataGridView2.Columns[9].Visible = false;
                //dataGridView2.Columns[0].Visible = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //if (textBox1.Text == "")
            //{
            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario where not Categoria='ACCESORIOS' and Existencia > 0 order by Nombre;", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView2.DataSource = ds.Tables["Id"];
            //    //dataGridView2.Columns[0].Visible = false;
            //}
            //else
            //{
            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario where not Categoria='ACCESORIOS' and Existencia > 0 and Nombre LIKE '%" + textBox1.Text + "%';", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView2.DataSource = ds.Tables["Id"];
            //    //dataGridView2.Columns[0].Visible = false;
            //}
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //if (textBox2.Text == "")
            //{
            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario where not Categoria='ACCESORIOS' and Existencia > 0  order by Nombre;", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView2.DataSource = ds.Tables["Id"];
            //    //dataGridView2.Columns[0].Visible = false;
            //}
            //else
            //{
            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario where not Categoria='ACCESORIOS'  and Existencia > 0 and Id LIKE '%" + textBox2.Text + "%';", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView2.DataSource = ds.Tables["Id"];
            //    //dataGridView2.Columns[0].Visible = false;
            //}
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
            // 1. Asegurarnos de que la lista principal de productos esté cargada
            if (listaCompletaInventario == null || listaCompletaInventario.Count == 0)
            {
                MessageBox.Show("No hay datos de inventario para procesar.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 2. La magia de LINQ: Agrupar por Categoría y Sumar
            // (Recuerda que listaCompletaInventario es tu List<ProductoInventario>)
            var resumen = listaCompletaInventario
                .GroupBy(p => p.Categoria) // Agrupamos todos los productos por su 'Categoria'
                .Select(grupo => new ResumenCategoria // Para cada grupo, creamos un objeto de resumen
                {
                    Categoria = grupo.Key, // El nombre de la categoría (ej. "Ferreteria")

                    // Sumamos la existencia de todos los productos en este grupo
                    TotalExistencia = grupo.Sum(p => p.Existencia),

                    // (Basado en tu código: Venta = Existencia * PrecioVenta)
                    TotalVenta = grupo.Sum(p => p.Existencia * p.PrecioVenta),

                    // (Basado en tu código: Inversión = Existencia * Col[7])
                    // En tu clase, la col[7] es 'Especial'. Usaremos esa propiedad.
                    TotalInversion = grupo.Sum(p => p.Existencia * p.Especial),

                    // Calculamos la utilidad directamente
                    Utilidad = grupo.Sum(p => (p.Existencia * p.PrecioVenta) - (p.Existencia * p.Especial))
                })
                .OrderBy(r => r.Categoria) // Opcional: ordenar alfabéticamente por categoría
                .ToList(); // Convertimos el resultado a una Lista

            // 3. Crear y mostrar el nuevo formulario, pasándole los datos
            frmDatos formularioResumen = new frmDatos(resumen);
            formularioResumen.Show(); // O .ShowDialog() si quieres que la ventana sea modal

            //button6.Visible = false;

            //else if (usuario == "Admin")
            //{
            //    for (int i = 0; i < dataGridView2.RowCount; i++)
            //    {
            //        try
            //        {
            //            if (dataGridView2[6, i].Value.ToString() == "ARMAZONES")
            //            {
            //                piezasArmazones += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "LENTE DE CONTACTO")
            //            {
            //                piezasLenteContacto += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "ESTUCHES DE LENTE DE CONTACTO")
            //            {
            //                piezasEstucheLenteContacto += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "SOLUCION DE LENTE DE CONTACTO")
            //            {
            //                piezasSolucionLenteContacto += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "MICAS")
            //            {
            //                piezasMicas += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "60 ml")
            //            {
            //                piezas60 += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "90ml")
            //            {
            //                piezas90 += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "120ml")
            //            {
            //                piezas120 += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "500ml")
            //            {
            //                piezas500 += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "LENTE DE LECTURA")
            //            {
            //                piezasLectura += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            //MessageBox.Show("ERROR EN PRODUCTO: " + dataGridView2[1, i].Value.ToString() + "\n" + ex);
            //        }
            //    }

            //    lblPiezasR.Text = "" + piezasArmazones;
            //    lblLenteContacto.Text = "" + piezasLenteContacto;
            //    lblEstuLenteContacto.Text = "" + piezasEstucheLenteContacto;
            //    lblSoluLenteContac.Text = "" + piezasSolucionLenteContacto;
            //    lblMicas.Text = "" + piezasMicas;
            //    lbl60.Text = "" + piezas60;
            //    lbl90.Text = "" + piezas90;
            //    lbl120.Text = "" + piezas120;
            //    lbl500.Text = "" + piezas500;
            //    lblLectura.Text =""+ piezasLectura;
            //    panel5.Visible = true;
            //    button6.Visible = false;
            //}
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox1.Text == "")
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Inventario order by Nombre;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                    dataGridView2.Columns[7].HeaderText = "Precio Compra";
                    dataGridView2.Columns[10].HeaderText = "Unidad";
                    dataGridView2.Columns[9].Visible = false;
                }
                else
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Inventario where Nombre LIKE '%" + textBox1.Text + "%';", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                    dataGridView2.Columns[7].HeaderText = "Precio Compra";
                    dataGridView2.Columns[10].HeaderText = "Unidad";
                    dataGridView2.Columns[9].Visible = false;
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox2.Text == "")
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Inventario order by Nombre;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                    dataGridView2.Columns[7].HeaderText = "Precio Compra";
                    dataGridView2.Columns[10].HeaderText = "Unidad";
                    dataGridView2.Columns[9].Visible = false;
                }
                else
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Inventario where Id LIKE '%" + textBox2.Text + "%';", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                    dataGridView2.Columns[7].HeaderText = "Precio Compra";
                    dataGridView2.Columns[10].HeaderText = "Unidad";
                    dataGridView2.Columns[9].Visible = false;
                }
            }
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
            catch (Exception ex)
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
             DialogResult dialogResult = MessageBox.Show("¿Desea imprimir un reporte para capturar el inventario fisico?", "Alto!", MessageBoxButtons.YesNo);
             if (dialogResult == DialogResult.Yes)
             {
                 Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
                 Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
                 Worksheet ws = (Worksheet)xla.ActiveSheet;

                 xla.Visible = true;

                 ws.Cells[1, 1] = "ID";
                 ws.Cells[1, 2] = "Nombre";
                 ws.Cells[1, 3] = "Existen";
                 ws.Cells[1, 4] = "Fisico";
                 int cont = 1;
                 for (int i = 0; i < dataGridView2.RowCount; i++)
                 {
                         cont++;
                         ws.Cells[cont, 1] = dataGridView2[0, i].Value.ToString();
                         ws.Cells[cont, 2] = dataGridView2[1, i].Value.ToString();
                         ws.Cells[cont, 3] = dataGridView2[4, i].Value.ToString();
                 }
                 frmInventariosFisicos fis = new frmInventariosFisicos();
                 fis.Show();
                 this.Close();
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
            if (listaCompletaInventario.Count > 0 && tamanoPagina > 0)
            {
                totalPaginas = (int)Math.Ceiling((double)listaCompletaInventario.Count / tamanoPagina);
            }
            else
            {
                totalPaginas = 1;
            }

            // 4. Recargar la vista
            // CargarPagina() ya se encarga de validar si 'paginaActual' 
            // se salió de los límites (ej. si achicas la ventana y el total de páginas aumenta)
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
    }
}
