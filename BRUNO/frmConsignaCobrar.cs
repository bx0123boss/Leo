using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace BRUNO
{
    public partial class frmConsignaCobrar : frmBase
    {
        private int _idCliente;
        private string _nombreCliente;
        private decimal _granTotal = 0; 
        private string datos = "";
        private string observaciones = "";
        public frmConsignaCobrar(int idCliente, string nombreCliente)
        {
            InitializeComponent();
            _idCliente = idCliente;
            _nombreCliente = nombreCliente;
            // KeyPreview permite que el formulario escuche las teclas (como F5)
            this.KeyPreview = true;
        }

        private void frmConsignaCobrar_Load(object sender, EventArgs e)
        {
            lblCliente.Text = _nombreCliente;
            EstilizarDataGridView(dgvProductos);
            EstilizarBotonPeligro(btnCancelar); // Usa estilo de peligro (Rojo)
            EstilizarBotonPrimario(btnCobrar);  // Usa estilo primario (Verde o Azul)

            CargarMercanciaPendiente();
        }

        private void CargarMercanciaPendiente()
        {
            try
            {
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            c.ProductoId, 
                            i.Nombre AS Producto, 
                            c.PrecioCongelado AS Precio, 
                            c.EnConsigna AS [En Consigna]
                        FROM ConsignaCliente c
                        INNER JOIN Inventario i ON c.ProductoId = i.Id
                        WHERE c.ClienteId = ? AND c.EnConsigna > 0";

                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("?", _idCliente);
                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dt.Columns.Add("A Pagar", typeof(int));
                        foreach (DataRow row in dt.Rows) row["A Pagar"] = 0;

                        dgvProductos.DataSource = dt;

                        dgvProductos.Columns["ProductoId"].Visible = false;
                        dgvProductos.Columns["Precio"].DefaultCellStyle.Format = "C2";

                        // Estilo para resaltar la columna donde el cajero escribe
                        dgvProductos.Columns["A Pagar"].DefaultCellStyle.BackColor = Color.LightYellow;
                        dgvProductos.Columns["A Pagar"].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
                        dgvProductos.Columns["A Pagar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                        foreach (DataGridViewColumn col in dgvProductos.Columns)
                        {
                            col.ReadOnly = (col.Name != "A Pagar");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la mercancía: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                // ---> OBTENER HISTORIAL Y CREAR COMBOBOX <---
                var historialDatos = ObtenerHistorialDatosCliente(_idCliente.ToString());
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
                    lblHistorial.ForeColor = Color.SteelBlue;

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

                    yPos += 45;
                }

                // --- 3. CONSULTA A LA BASE DE DATOS (SQL Server) ---
                string query = "SELECT NombreEtiqueta FROM CotizacionCamposConfig WHERE Activo = 1 ORDER BY Orden";
                try
                {
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Conexion.CadSQL))
                    {
                        conn.Open();
                        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                        {
                            using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
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
                                    lbl.ForeColor = Color.White; // Para que se vea en el fondo negro de tu frmBase

                                    TextBox txt = new TextBox();
                                    txt.Left = 150;
                                    txt.Top = yPos - 3;
                                    txt.Width = 300;
                                    txt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

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

                // ---> EVENTO DEL COMBOBOX PARA LLENAR TEXTBOXES <---
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
                lblObs.ForeColor = Color.White;
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

            if (string.IsNullOrEmpty(idClient) || idClient == "0") return historial;

            try
            {
                using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(Conexion.CadSQL))
                {
                    con.Open();
                    string query = "SELECT Datos FROM Cotizaciones WHERE ClienteId = @ClienteId AND Datos IS NOT NULL AND Datos <> ''";

                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClienteId", idClient);
                        using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
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
        private void dgvProductos_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(ColumnNum_KeyPress);
            if (dgvProductos.CurrentCell.ColumnIndex == dgvProductos.Columns["A Pagar"].Index)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null) tb.KeyPress += new KeyPressEventHandler(ColumnNum_KeyPress);
            }
        }

        private void ColumnNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dgvProductos_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvProductos.Columns["A Pagar"].Index)
            {
                int aPagar = 0;
                int enConsigna = Convert.ToInt32(dgvProductos.Rows[e.RowIndex].Cells["En Consigna"].Value);

                if (dgvProductos.Rows[e.RowIndex].Cells["A Pagar"].Value != DBNull.Value &&
                    dgvProductos.Rows[e.RowIndex].Cells["A Pagar"].Value != null)
                {
                    aPagar = Convert.ToInt32(dgvProductos.Rows[e.RowIndex].Cells["A Pagar"].Value);
                }

                if (aPagar > enConsigna)
                {
                    MessageBox.Show("No puede cobrar más piezas de las que el cliente tiene en consigna.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvProductos.Rows[e.RowIndex].Cells["A Pagar"].Value = enConsigna;
                }

                RecalcularTotal();
            }
        }

        private void RecalcularTotal()
        {
            _granTotal = 0;
            foreach (DataGridViewRow row in dgvProductos.Rows)
            {
                decimal precio = Convert.ToDecimal(row.Cells["Precio"].Value);
                int aPagar = Convert.ToInt32(row.Cells["A Pagar"].Value == DBNull.Value ? 0 : row.Cells["A Pagar"].Value);
                _granTotal += (precio * aPagar);
            }
            lblTotal.Text = _granTotal.ToString("C2");
        }

        // =========================================================================================
        // EVENTOS DE TECLADO (F5 para cobrar)
        // =========================================================================================
        private void frmConsignaCobrar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                btnCobrar.PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }

        // =========================================================================================
        // BOTÓN COBRAR - LA TRANSACCIÓN PRINCIPAL (COPIA EXACTA DE frmVentas)
        // =========================================================================================
        private void btnCobrar_Click(object sender, EventArgs e)
        {
            dgvProductos.EndEdit();
            RecalcularTotal();

            if (_granTotal == 0)
            {
                MessageBox.Show("Debe indicar al menos un producto a pagar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 1. ABRIR EL MODAL DE PAGO (frmPago)
            double efectivo = 0, cambio = 0;
            Dictionary<string, double> pagosFinales = new Dictionary<string, double>();

            using (frmPago ori = new frmPago())
            {
                ori.total = Convert.ToDouble(_granTotal);
                ori.txtTotal.Text = $"{ori.total:C}";
                if (ori.ShowDialog() == DialogResult.OK)
                {
                    efectivo = ori.efectivo;
                    cambio = ori.cambio;
                    pagosFinales = ori.PagosRealizados;
                }
                else
                {
                    return; // Si cancelan el pago, se detiene todo
                }
            }

            using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
            {
                con.Open();
                using (OleDbTransaction transaccion = con.BeginTransaction())
                {
                    try
                    {
                        // 2. OBTENER EL FOLIO ACTUAL
                        int foli = 0;
                        using (var cmd = new OleDbCommand("select Numero from Folios where Folio='FolioContado';", con, transaccion))
                        {
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read()) foli = Convert.ToInt32(reader[0].ToString());
                            }
                        }

                        string folioVenta = (Conexion.lugar == "TURBO LLANTAS" ? "TB" : "VR") + String.Format("{0:0000}", foli);

                        List<Producto> productosTicket = new List<Producto>();
                        double totalVenta = Convert.ToDouble(_granTotal);

                        // 3. PROCESAR CADA PRODUCTO COBRADO
                        foreach (DataGridViewRow row in dgvProductos.Rows)
                        {
                            int aPagar = Convert.ToInt32(row.Cells["A Pagar"].Value == DBNull.Value ? 0 : row.Cells["A Pagar"].Value);

                            if (aPagar > 0)
                            {
                                string idProducto = row.Cells["ProductoId"].Value.ToString();
                                decimal precioCongelado = Convert.ToDecimal(row.Cells["Precio"].Value);
                                string nombreProd = row.Cells["Producto"].Value.ToString();
                                double montoFila = Convert.ToDouble(precioCongelado) * aPagar;

                                // A) Obtener datos extra de Inventario para VentasContado (Sin afectar stock)
                                double exis = 0;
                                double precioCompra = 0;
                                string categoria = "";

                                using (var cmd = new OleDbCommand("select Existencia, Categoria, Especial from Inventario where Id='" + idProducto + "';", con, transaccion))
                                {
                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            exis = Convert.ToDouble(reader["Existencia"].ToString());
                                            categoria = reader["Categoria"].ToString();
                                            precioCompra = reader["Especial"] != DBNull.Value ? Convert.ToDouble(reader["Especial"]) : 0;
                                        }
                                    }
                                }

                                // B) Afectar la Consigna (Descuenta saldo, suma a Vendidos)
                                string queryBalance = "UPDATE ConsignaCliente SET EnConsigna = EnConsigna - ?, Vendidos = Vendidos + ? WHERE ClienteId = ? AND ProductoId = ? AND PrecioCongelado = ?";
                                using (OleDbCommand cmd = new OleDbCommand(queryBalance, con, transaccion))
                                {
                                    cmd.Parameters.AddWithValue("?", aPagar);
                                    cmd.Parameters.AddWithValue("?", aPagar);
                                    cmd.Parameters.AddWithValue("?", _idCliente);
                                    cmd.Parameters.AddWithValue("?", idProducto);
                                    cmd.Parameters.AddWithValue("?", precioCongelado);
                                    cmd.ExecuteNonQuery();
                                }

                                // C) Rastro en Kardex de Consigna
                                string queryKardexConsigna = "INSERT INTO ConsignaMovimientos (ClienteId, ProductoId, TipoMovimiento, Cantidad, PrecioVigente, Fecha, ReferenciaVentaId) VALUES (?, ?, 'VENTA', ?, ?, NOW(), ?)";
                                using (OleDbCommand cmd = new OleDbCommand(queryKardexConsigna, con, transaccion))
                                {
                                    cmd.Parameters.AddWithValue("?", _idCliente);
                                    cmd.Parameters.AddWithValue("?", idProducto);
                                    cmd.Parameters.AddWithValue("?", aPagar);
                                    cmd.Parameters.AddWithValue("?", precioCongelado);
                                    cmd.Parameters.AddWithValue("?", folioVenta); // Se guarda el folio correcto como lo pediste
                                    cmd.ExecuteNonQuery();
                                }

                                // D) Kardex General de la tienda (Sin restar stock, existencia antes y después es igual)
                                string queryKardexGen = "insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + idProducto + "','VENTA','VENTA DE CONSIGNA FOLIO: " + folioVenta + "'," + exis + ",'" + exis + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');";
                                using (var cmd = new OleDbCommand(queryKardexGen, con, transaccion))
                                {
                                    cmd.ExecuteNonQuery();
                                }

                                // E) Guardar en VentasContado (Detalle de Venta)
                                double utilidad = (Convert.ToDouble(precioCongelado) * aPagar) - (precioCompra * aPagar);
                                string queryVentasContado = "insert into VentasContado(FolioVenta,IdProducto,Cantidad,Producto,MontoTotal,idCliente,Fecha,Utilidad, Categoria) values('" + folioVenta + "','" + idProducto + "','" + aPagar + "','" + nombreProd + "','" + montoFila + "','" + _idCliente + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + utilidad + "','" + categoria + "');";
                                using (var cmd = new OleDbCommand(queryVentasContado, con, transaccion))
                                {
                                    cmd.ExecuteNonQuery();
                                }

                                // F) Agregar a la lista para imprimir el ticket
                                productosTicket.Add(new Producto
                                {
                                    Nombre = nombreProd,
                                    Cantidad = aPagar,
                                    PrecioUnitario = Convert.ToDouble(precioCongelado),
                                    Total = montoFila
                                });
                            }
                        }

                        // 4. CABECERA GENERAL DE VENTA
                        string tipoPagoVenta = pagosFinales.Count > 1 ? "MIXTO" : pagosFinales.First().Key;
                        using (var cmd = new OleDbCommand("insert into Ventas(Monto,Fecha,Folio,Estatus, Descuento, Pago) values('" + totalVenta + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + folioVenta + "','COBRADO','0','" + tipoPagoVenta + "');", con, transaccion))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // 5. REGISTRAR LOS PAGOS EN EL CORTE
                        foreach (var pago in pagosFinales)
                        {
                            if (pago.Value > 0)
                            {
                                using (var cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Cobro de consigna folio " + folioVenta + "','" + pago.Value + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + pago.Key + "');", con, transaccion))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // 6. REGISTRAR CAJERO
                        using (var cmd = new OleDbCommand("insert into VentasCajero(IdUsuario,Usuario,FolioVenta,Total,Fecha,Cajero) values('1','Administrador','" + folioVenta + "','" + totalVenta + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','Cajero');", con, transaccion))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // 7. INCREMENTAR FOLIO Y ACTUALIZAR
                        foli++;
                        using (var cmd = new OleDbCommand("UPDATE Folios set Numero = " + foli + " where Folio = 'FolioContado'; ", con, transaccion))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        transaccion.Commit();

                        // 8. IMPRESIÓN DEL TICKET
                        Dictionary<string, double> totalesTicket = new Dictionary<string, double>();
                        if (Conexion.ConIva)
                        {
                            totalesTicket.Add("Subtotal", totalVenta / 1.16);
                            totalesTicket.Add("IVA", (totalVenta / 1.16) * 0.16);
                        }
                        totalesTicket.Add("Total", totalVenta);
                        totalesTicket.Add("Recibido", efectivo);
                        totalesTicket.Add("Cambio", cambio);

                        if (Conexion.impresionMediaCarta)
                        {
                            DialogResult respuesta = MessageBox.Show("¿Deseas imprimir?", "IMPRESIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (respuesta == DialogResult.Yes)
                            {
                                TicketMediaCarta pdfTicket = new TicketMediaCarta(productosTicket, folioVenta, 0, totalVenta, _nombreCliente, _idCliente.ToString(), tipoPagoVenta, datos, observaciones, Conexion.lugar, Conexion.logoPath, Conexion.datosTicket, Conexion.pieDeTicket);
                                pdfTicket.ImprimirDirectamente(Conexion.impresora);
                            }
                        }
                        else
                        {
                            TicketPrinter ticketPrinter = new TicketPrinter(Conexion.datosTicket, Conexion.pieDeTicket, Conexion.logoPath, productosTicket, folioVenta, "", "", totalVenta, false, totalesTicket, tipoPagoVenta);
                            ticketPrinter.ImprimirTicket();
                        }

                        MessageBox.Show("Venta de consigna realizada con éxito", "VENTA REALIZADA", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        MessageBox.Show("Error al procesar el cobro. Se ha cancelado la operación para evitar descuadres. Detalle: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}