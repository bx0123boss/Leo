using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmNuevaConsignacion : frmBase
    {
        OleDbConnection conectarAccess = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        double total = 0;
        string origen = "";
        string idCliente = "0";
        string observaciones = "", datos = "";
        public string usuario = "", idUsuario = "";

        public frmNuevaConsignacion()
        {
            InitializeComponent();
            this.MinimumSize = new Size(1066, 418);
        }

        private void frmNuevaConsignacion_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPrimario(this.btnGenerarConsigna);
            EstilizarBotonAdvertencia(this.button3); // Botón Buscar Producto
            EstilizarBotonAdvertencia(this.btnBuscarCliente); // Botón Buscar Producto
            EstilizarBotonPeligro(this.button1); // Botón Eliminar
            EstilizarTextBox(this.textBox1);

            lblFolio.Visible = false;
            label2.Visible = false;

            // Configurar columnas editables
            dataGridView1.Columns[0].ReadOnly = false; // Cantidad
            if (Conexion.lugar == "DEPORTES LEO")
            {
                dataGridView1.Columns[2].ReadOnly = false; // Precio
            }
            if (Conexion.lugar == "TURBO LLANTAS")
            {
                dataGridView1.Columns[2].ReadOnly = false;
                dataGridView1.Columns[1].ReadOnly = false; // Descripción
            }
        }

        public void ReiniciarForm()
        {
            total = 0;
            observaciones = "";
            datos = "";
            origen = "";
            idCliente = "0";
            lblCliente.Text = "PUBLICO EN GENERAL";
            lblDatosCotizacion.Text = "Sin datos extra";
            dataGridView1.Rows.Clear();
            lblTotal.Text = $"{RecalcularTotal:C}";
            textBox1.Text = "";
            textBox1.Focus();
        }

        private double RecalcularTotal
        {
            get
            {
                total = 0;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    total += Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value); // Columna Monto
                }
                return total;
            }
        }

        // =========================================================================
        // EVENTOS DE AGREGAR PRODUCTOS (Báscula, Lector de código y Buscador)
        // =========================================================================

        private double ObtenerPesoLocal()
        {
            if (!frmPrincipal.IsAgenteBasculaActivo)
            {
                return 1;
            }
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://localhost:8080/peso/");
                request.Timeout = 300;
                request.Method = "GET";

                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                using (System.IO.StreamReader stream = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string respuesta = stream.ReadToEnd();
                    if (respuesta != "ERROR")
                    {
                        return double.Parse(respuesta, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }
            catch (Exception) { }
            return 0;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            EliminarProductos();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Índice 10 es el botón 'btnEliminar' con la X
            if (e.ColumnIndex == 10 && e.RowIndex >= 0)
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                lblTotal.Text = $"{RecalcularTotal:C}";
                textBox1.Focus();
            }
        }

        // =========================================================================
        // CLIENTE, EXTRAS Y GUARDADO DE CONSIGNA
        // =========================================================================

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            using (frmBuscaCliente cliente = new frmBuscaCliente())
            {
                if (cliente.ShowDialog() == DialogResult.OK)
                {
                    idCliente = cliente.ID;
                    lblCliente.Text = cliente.Nombre;
                }
            }
        }

        private void btnGenerarConsigna_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount == 0)
            {
                MessageBox.Show("No hay productos para consignar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (idCliente == "0" || string.IsNullOrEmpty(idCliente))
            {
                MessageBox.Show("Debe seleccionar un cliente obligatoriamente para una consignación.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult respuesta = MessageBox.Show("¿Desea generar la salida a consignación? Se descontará el inventario físico.", "Confirmar Consigna", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta != DialogResult.Yes) return;

            GuardarConsignacion();
        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text)) return;
                string input = textBox1.Text.Trim();

                try
                {
                    conectarAccess.Open();
                    cmd = new OleDbCommand("select count(*) from Inventario where Id='" + input + "';", conectarAccess);
                    int valor = int.Parse(cmd.ExecuteScalar().ToString());

                    if (valor == 1)
                    {
                        cmd = new OleDbCommand("select * from Inventario where Id='" + input + "';", conectarAccess);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            using (frmPrecio buscar = new frmPrecio())
                            {
                                if (buscar.ShowDialog() == DialogResult.OK)
                                {
                                    double preci = (buscar.tipo == "GEN") ? Convert.ToDouble(reader[3]) : Convert.ToDouble(reader[2]);
                                    double cantidad = ObtenerPesoLocal();
                                    if (cantidad <= 0) cantidad = 1;
                                    double importe = preci * cantidad;

                                    dataGridView1.Rows.Add(
                                        cantidad.ToString(),
                                        reader[1].ToString(),
                                        String.Format("{0:0.00}", preci),
                                        String.Format("{0:0.00}", importe),
                                        reader[4].ToString(), reader[0].ToString(), origen, reader[8].ToString(), reader[7].ToString(), "", "X");
                                }
                            }
                        }
                        reader.Close();
                        lblTotal.Text = $"{RecalcularTotal:C}";
                        textBox1.Text = "";
                    }
                    else
                    {
                        using (frmBuscarProductos buscar = new frmBuscarProductos())
                        {
                            buscar.textBox1.Text = input;
                            if (buscar.ShowDialog() == DialogResult.OK)
                            {
                                double cantidad = ObtenerPesoLocal();
                                if (cantidad <= 0) cantidad = 1;

                                double precioUnitario = Convert.ToDouble(buscar.precio);
                                double importeCalculado = precioUnitario * cantidad;

                                dataGridView1.Rows.Add(
                                    cantidad.ToString(),
                                    buscar.producto,
                                    String.Format("{0:0.00}", precioUnitario),
                                    String.Format("{0:0.00}", importeCalculado),
                                    buscar.existencia, buscar.ID, origen, buscar.IVA, buscar.compra, "", "X"
                                );
                                lblTotal.Text = $"{RecalcularTotal:C}";
                            }
                        }
                        textBox1.Text = "";
                    }
                }
                finally
                {
                    if (conectarAccess.State == ConnectionState.Open) conectarAccess.Close();
                }
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (frmBuscarProductos buscar = new frmBuscarProductos())
            {
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    double cantidad = ObtenerPesoLocal();
                    if (cantidad <= 0) cantidad = 1;

                    double precio = Convert.ToDouble(buscar.precio);
                    double importe = precio * cantidad;

                    dataGridView1.Rows.Add(
                        cantidad.ToString(),
                        buscar.producto,
                        String.Format("{0:0.00}", precio),
                        String.Format("{0:0.00}", importe),
                        buscar.existencia, buscar.ID, origen, buscar.IVA, buscar.compra, "", "X");
                }
            }
            lblTotal.Text = $"{RecalcularTotal:C}";
            textBox1.Focus();
        }

        // =========================================================================
        // EVENTOS DE EDICIÓN Y ELIMINACIÓN EN EL GRID
        // =========================================================================

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 2)
            {
                try
                {
                    var row = dataGridView1.Rows[e.RowIndex];
                    if (row.Cells[0].Value != null && row.Cells[2].Value != null)
                    {
                        double cantidad = Convert.ToDouble(row.Cells[0].Value);
                        double precio = Convert.ToDouble(row.Cells[2].Value);
                        double monto = cantidad * precio;
                        row.Cells[3].Value = monto.ToString("0.00");
                    }
                }
                catch
                {
                    MessageBox.Show("Solo puedes introducir números válidos.", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dataGridView1.Rows[e.RowIndex].Cells[0].Value = "1";
                    double precio = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                    dataGridView1.Rows[e.RowIndex].Cells[3].Value = precio.ToString("0.00");
                }
                lblTotal.Text = $"{RecalcularTotal:C}";
                textBox1.Focus();
            }
        }

        private void EliminarProductos()
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione al menos una fila para eliminar.", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (row.DataBoundItem != null)
                    {
                        ((DataRowView)row.DataBoundItem).Row.Delete();
                    }
                    dataGridView1.Rows.Remove(row);
                }
                lblTotal.Text = $"{RecalcularTotal:C}";
                textBox1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar filas: {ex.Message}");
            }
        }
        private void lblDatosCotizacion_Click(object sender, EventArgs e)
        {
            using (frmBase frmCaptura = new frmBase())
            {
                frmCaptura.Text = "Captura de Datos Extra";
                frmCaptura.StartPosition = FormStartPosition.CenterParent;
                frmCaptura.MaximizeBox = true;
                frmCaptura.MinimizeBox = false;
                frmCaptura.ClientSize = new Size(500, 550); // Tamaño inicial cómodo
                frmCaptura.MinimumSize = new Size(450, 400);

                // --- 1. CREACIÓN DE PANELES PARA ARREGLAR EL SCROLL ---
                Panel pnlBotones = new Panel();
                pnlBotones.Height = 70;
                pnlBotones.Dock = DockStyle.Bottom;

                Panel pnlCampos = new Panel();
                pnlCampos.Dock = DockStyle.Fill;
                pnlCampos.AutoScroll = true;

                frmCaptura.Controls.Add(pnlCampos);
                frmCaptura.Controls.Add(pnlBotones);

                // --- 2. LECTURA DE DATOS PREVIOS ---
                int yPos = 20;
                Dictionary<string, string> valoresExistentes = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(datos))
                {
                    string[] pares = datos.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string par in pares)
                    {
                        string[] partes = par.Split(new[] { ':' }, 2);
                        if (partes.Length == 2)
                        {
                            valoresExistentes[partes[0].Trim()] = partes[1].Trim();
                        }
                    }
                }

                List<TextBox> listaTextBoxes = new List<TextBox>();
                List<string> listaEtiquetas = new List<string>();

                // ---> NUEVO: OBTENER HISTORIAL Y CREAR COMBOBOX <---
                var historialDatos = ObtenerHistorialDatosCliente(idCliente);
                ComboBox cmbHistorial = null;

                if (historialDatos.Count > 0)
                {
                    Label lblHistorial = new Label();
                    lblHistorial.Text = "Historial:";
                    lblHistorial.Left = 20;
                    lblHistorial.Top = yPos;
                    lblHistorial.Width = 120;
                    lblHistorial.TextAlign = ContentAlignment.MiddleRight;
                    lblHistorial.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    lblHistorial.ForeColor = Color.SteelBlue; // Para que resalte visualmente

                    cmbHistorial = new ComboBox();
                    cmbHistorial.Left = 150;
                    cmbHistorial.Top = yPos - 3;
                    cmbHistorial.Width = 300;
                    cmbHistorial.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbHistorial.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                    cmbHistorial.Items.Add("-- Seleccione para rellenar --");
                    foreach (var reg in historialDatos)
                    {
                        string resumen = string.Join(" | ", reg.Values.Where(v => !string.IsNullOrEmpty(v)).Take(3));
                        cmbHistorial.Items.Add(resumen);
                    }
                    cmbHistorial.SelectedIndex = 0;

                    pnlCampos.Controls.Add(lblHistorial);
                    pnlCampos.Controls.Add(cmbHistorial);

                    yPos += 45; // Bajamos el eje Y para los siguientes campos
                }

                // --- 3. CONSULTA A LA BASE DE DATOS ---
                string query = "SELECT NombreEtiqueta FROM CotizacionCamposConfig WHERE Activo = 1 ORDER BY Orden";
                try
                {
                    using (SqlConnection conn = new SqlConnection(Conexion.CadSQL))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string etiqueta = reader["NombreEtiqueta"].ToString();
                                    listaEtiquetas.Add(etiqueta);

                                    Label lbl = new Label();
                                    lbl.Text = etiqueta + ":";
                                    lbl.Left = 20;
                                    lbl.Top = yPos;
                                    lbl.Width = 120;
                                    lbl.TextAlign = ContentAlignment.MiddleRight;
                                    lbl.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

                                    TextBox txt = new TextBox();
                                    txt.Left = 150;
                                    txt.Top = yPos - 3;
                                    txt.Width = 300;
                                    txt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                                    // Si existe la funcion EstilizarTextBox, descomentarla:
                                    EstilizarTextBox(txt);

                                    if (valoresExistentes.ContainsKey(etiqueta))
                                    {
                                        txt.Text = valoresExistentes[etiqueta];
                                    }

                                    pnlCampos.Controls.Add(lbl);
                                    pnlCampos.Controls.Add(txt);
                                    listaTextBoxes.Add(txt);

                                    yPos += 45;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar configuración de campos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ---> NUEVO: EVENTO DEL COMBOBOX PARA LLENAR TEXTBOXES <---
                if (cmbHistorial != null)
                {
                    cmbHistorial.SelectedIndexChanged += (sCombo, evCombo) =>
                    {
                        if (cmbHistorial.SelectedIndex > 0)
                        {
                            var registroElegido = historialDatos[cmbHistorial.SelectedIndex - 1];
                            for (int i = 0; i < listaEtiquetas.Count; i++)
                            {
                                string etiq = listaEtiquetas[i];
                                if (registroElegido.ContainsKey(etiq))
                                {
                                    listaTextBoxes[i].Text = registroElegido[etiq];
                                }
                            }
                        }
                    };
                }

                // --- 4. CAMPO DE OBSERVACIONES ---
                yPos += 10;
                Label lblObs = new Label() { Text = "Observaciones (Ej. Detalles extra):", Left = 20, Top = yPos, Width = 380 };
                lblObs.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                yPos += 25;

                TextBox txtObs = new TextBox()
                {
                    Left = 20,
                    Top = yPos,
                    Width = 430,
                    Height = 90,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Text = observaciones,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

                EstilizarTextBox(txtObs);
                pnlCampos.Controls.Add(lblObs);
                pnlCampos.Controls.Add(txtObs);

                // --- 5. BOTONES EN EL PANEL FIJO ---
                Button btnAceptar = new Button() { Text = "Aceptar", Width = 90, Height = 40, DialogResult = DialogResult.OK };
                Button btnCancelar = new Button() { Text = "Cancelar", Width = 90, Height = 40, DialogResult = DialogResult.Cancel };

                btnAceptar.Top = 15;
                btnCancelar.Top = 15;
                btnAceptar.Left = pnlBotones.ClientSize.Width - 210;
                btnCancelar.Left = pnlBotones.ClientSize.Width - 110;
                btnAceptar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                EstilizarBotonPrimario(btnAceptar);
                EstilizarBotonPeligro(btnCancelar);

                pnlBotones.Controls.Add(btnAceptar);
                pnlBotones.Controls.Add(btnCancelar);

                // --- 6. PROCESAR GUARDADO ---
                frmCaptura.AcceptButton = btnAceptar;
                frmCaptura.CancelButton = btnCancelar;

                if (frmCaptura.ShowDialog() == DialogResult.OK)
                {
                    List<string> datosGuardar = new List<string>();

                    for (int i = 0; i < listaTextBoxes.Count; i++)
                    {
                        string valorCapturado = listaTextBoxes[i].Text.Trim();
                        if (!string.IsNullOrEmpty(valorCapturado))
                        {
                            datosGuardar.Add($"{listaEtiquetas[i]}: {valorCapturado}");
                        }
                    }

                    datos = string.Join("; ", datosGuardar);
                    observaciones = txtObs.Text.Trim();

                    if (!string.IsNullOrEmpty(datos))
                    {
                        string textoVisual = datos.Replace(";", "   |   ");
                        lblDatosCotizacion.Text = textoVisual;
                        lblDatosCotizacion.Visible = true;
                    }
                    else
                    {
                        lblDatosCotizacion.Text = "[ Clic aquí para capturar datos extra ]";
                        lblDatosCotizacion.Visible = true;
                    }

                    if (!string.IsNullOrEmpty(observaciones))
                    {
                        label9.Text = "Observaciones: " + observaciones;
                        label9.Visible = true;
                    }
                    else
                    {
                        label9.Text = "";
                        label9.Visible = false;
                    }
                }
            }
        }

        private List<Dictionary<string, string>> ObtenerHistorialDatosCliente(string idClient)
        {
            var historial = new List<Dictionary<string, string>>();

            // Si no hay cliente seleccionado, no buscamos historial
            if (string.IsNullOrEmpty(idClient) || idClient == "0") return historial;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
                {
                    con.Open();
                    // Buscamos el historial de los datos en las cotizaciones pasadas
                    string query = "SELECT Datos FROM Cotizaciones WHERE ClienteId = @ClienteId AND Datos IS NOT NULL AND Datos <> ''";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClienteId", idClient);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string datosRaw = reader["Datos"].ToString();
                                if (!string.IsNullOrWhiteSpace(datosRaw))
                                {
                                    var pares = datosRaw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    var registro = new Dictionary<string, string>();

                                    foreach (var par in pares)
                                    {
                                        var partes = par.Split(new[] { ':' }, 2);
                                        if (partes.Length == 2)
                                        {
                                            string etiqueta = partes[0].Trim();
                                            string valor = partes[1].Trim();

                                            if (!string.IsNullOrWhiteSpace(valor))
                                            {
                                                registro[etiqueta] = valor;
                                            }
                                        }
                                    }

                                    // Evitamos agregar registros exactos duplicados
                                    if (registro.Count > 0)
                                    {
                                        bool yaExiste = historial.Any(h => h.Count == registro.Count && !h.Except(registro).Any());
                                        if (!yaExiste) historial.Add(registro);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { /* Ignoramos si ocurre un error de conexión */ }

            return historial;
        }

        private void GuardarConsignacion()
        {
            try
            {
                // 1. GENERAR FOLIO
                int foli = 1;
                int idConsignacionInsertada = 0;
                using (OleDbCommand cmdFolio = new OleDbCommand("SELECT Numero FROM Folios WHERE Folio='consigna';", conectarAccess))
                {
                    object res = cmdFolio.ExecuteScalar();
                    if (res != null && res != DBNull.Value)
                    {
                        foli = Convert.ToInt32(res);
                    }
                }
                // 2. GUARDAR EN SQL SERVER
                using (SqlConnection conSql = new SqlConnection(Conexion.CadSQL))
                {
                    conSql.Open();
                    SqlTransaction transaccionSql = conSql.BeginTransaction();

                    try
                    {
                        string queryPadre = @"INSERT INTO Consignaciones (Folio, ClienteId, ClienteNombre, Fecha, Total, Estado, Observaciones, Datos) 
                                              OUTPUT INSERTED.Id 
                                              VALUES (@Folio, @CId, @CNom, GETDATE(), @Total, 'Pendiente', @Obs, @Dat)";
                        using (SqlCommand cmdSql = new SqlCommand(queryPadre, conSql, transaccionSql))
                        {
                            cmdSql.Parameters.AddWithValue("@Folio", foli);
                            cmdSql.Parameters.AddWithValue("@CId", idCliente);
                            cmdSql.Parameters.AddWithValue("@CNom", lblCliente.Text);
                            cmdSql.Parameters.AddWithValue("@Total", total);
                            cmdSql.Parameters.AddWithValue("@Obs", observaciones);
                            cmdSql.Parameters.AddWithValue("@Dat", datos);

                            idConsignacionInsertada = (int)cmdSql.ExecuteScalar();
                        }

                        string queryDetalle = @"INSERT INTO DetalleConsignacion (ConsignacionId, ProductoCodigo, Descripcion, PrecioUnitario, CantidadLlevada, ImporteTotalLlevado) 
                                                VALUES (@CId, @ProdCod, @Desc, @Pre, @Cant, @Imp)";

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            using (SqlCommand cmdDet = new SqlCommand(queryDetalle, conSql, transaccionSql))
                            {
                                cmdDet.Parameters.AddWithValue("@CId", foli);
                                cmdDet.Parameters.AddWithValue("@Cant", Convert.ToDouble(row.Cells[0].Value));
                                cmdDet.Parameters.AddWithValue("@Desc", row.Cells[1].Value.ToString());
                                cmdDet.Parameters.AddWithValue("@Pre", Convert.ToDecimal(row.Cells[2].Value));
                                cmdDet.Parameters.AddWithValue("@Imp", Convert.ToDecimal(row.Cells[3].Value));
                                cmdDet.Parameters.AddWithValue("@ProdCod", row.Cells[5].Value.ToString());
                                cmdDet.ExecuteNonQuery();
                            }
                        }
                        transaccionSql.Commit();
                    }
                    catch (Exception exSql)
                    {
                        transaccionSql.Rollback();
                        throw new Exception("Error al guardar en SQL: " + exSql.Message);
                    }
                }

                // 3. DESCONTAR INVENTARIO EN ACCESS
                OleDbTransaction transaccionAccess = conectarAccess.BeginTransaction();
                try
                {
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        string idProd = dataGridView1.Rows[i].Cells[5].Value.ToString();
                        double cantidad = Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value);
                        double existenciaActual = Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value);
                        double nuevaExistencia = existenciaActual - cantidad;

                        using (OleDbCommand cmdAcc = new OleDbCommand("UPDATE Inventario SET Existencia='" + nuevaExistencia + "' WHERE Id='" + idProd + "';", conectarAccess, transaccionAccess))
                        {
                            cmdAcc.ExecuteNonQuery();
                        }

                        string descKardex = "SALIDA A CONSIGNACION FOLIO: " + foli;
                        string queryKardex = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) VALUES('" + idProd + "', 'SALIDA', '" + descKardex + "', " + existenciaActual + ", '" + nuevaExistencia + "', '" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "');";
                        using (OleDbCommand cmdKar = new OleDbCommand(queryKardex, conectarAccess, transaccionAccess))
                        {
                            cmdKar.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Consignación generada exitosamente con Folio: " + foli, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    foli++;
                    using (OleDbCommand cmdUpdFolio = new OleDbCommand("UPDATE Folios SET Numero=" + foli + " WHERE Folio='consigna';", conectarAccess, transaccionAccess))
                    {
                        cmdUpdFolio.ExecuteNonQuery();
                    }

                    transaccionAccess.Commit();
                }
                catch (Exception exAcc)
                {
                    transaccionAccess.Rollback();
                    throw new Exception("Error al descontar inventario en Access: " + exAcc.Message);
                }
                finally
                {
                    conectarAccess.Close();
                }

                

                // TODO: Imprimir Ticket 

                ReiniciarForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cierre del Formulario y Hotkeys
        private void frmNuevaConsignacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                btnGenerarConsigna_Click(sender, e);
            }
            if (e.KeyCode == Keys.Delete)
            {
                EliminarProductos();
            }
        }
    }
}
    