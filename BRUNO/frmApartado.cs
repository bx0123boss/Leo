using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmApartado : frmBase // Hereda de frmBase para los estilos
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        double total = 0;
        string origen = "";
        double exis = 0.0;
        public string direccion = "", tel = "", correo = "";
        public string idCliente = "0";
        double descuento = 0;
        public string usuario = "";
        double existencia = 0;
        int foli;
        string datos = "", observaciones = "";

        public frmApartado()
        {
            InitializeComponent();
            AplicarEstilos();

            // Suscribimos los eventos Leave para recalcular si el usuario sale del TextBox con el tabulador o el ratón
            this.txtDescuento.Leave += new System.EventHandler(this.txtDescuento_Leave);
            this.txtAbono.Leave += new System.EventHandler(this.txtAbono_Leave);
        }

        private void AplicarEstilos()
        {
            EstilizarDataGridView(this.dataGridView1);

            // Botones
            EstilizarBotonPrimario(this.button2);     // Guardar / Cobrar
            EstilizarBotonPrimario(this.button4);     // Buscar Cliente
            EstilizarBotonAdvertencia(this.button3);  // Buscar Producto
            EstilizarBotonPeligro(this.button1);      // Eliminar Producto

            // Controles
            EstilizarComboBox(this.cmbPago);
            EstilizarTextBox(this.txtDescuento);
            EstilizarTextBox(this.txtAbono);
            EstilizarTextBox(this.textBox1);
        }

        private void frmApartado_Load(object sender, EventArgs e)
        {
            conectar.Open();
            cmd = new OleDbCommand("select Numero from Folios where Folio='FolioApartado';", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                foli = Convert.ToInt32(Convert.ToString(reader[0].ToString()));
            }
            if (cmbPago.Items.Count > 0)
            {
                cmbPago.SelectedIndex = 0;
            }
            lblFolio.Text = "A" + String.Format("{0:0000}", foli);

            if (Conexion.lugar == "TURBO LLANTAS" && usuario == "Administrador")
            {
                dataGridView1.Columns[2].ReadOnly = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (frmBuscaCliente cliente = new frmBuscaCliente())
            {
                if (cliente.ShowDialog() == DialogResult.OK)
                {
                    idCliente = cliente.ID;
                    lblCliente.Text = cliente.Nombre;
                    direccion = cliente.direccion;
                    tel = cliente.tel;
                    correo = cliente.correo;
                }
                cliente.Show();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (frmBuscarProductos buscar = new frmBuscarProductos())
            {
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen);
                }
            }
            CalcularTotales();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox1.Text != "")
                {
                    cmd = new OleDbCommand("select count(*) from Inventario where Id='" + textBox1.Text + "';", conectar);
                    int valor = int.Parse(cmd.ExecuteScalar().ToString());
                    if (valor == 1)
                    {
                        cmd = new OleDbCommand("select * from Inventario where Id='" + textBox1.Text + "';", conectar);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            using (frmPrecio buscar = new frmPrecio())
                            {
                                if (buscar.ShowDialog() == DialogResult.OK)
                                {
                                    if (buscar.tipo == "GEN")
                                    {
                                        double preci = Convert.ToDouble(reader[3].ToString());
                                        dataGridView1.Rows.Add("1", Convert.ToString(reader[1].ToString()), String.Format("{0:0.00}", preci), String.Format("{0:0.00}", preci), Convert.ToString(reader[4].ToString()), Convert.ToString(reader[0].ToString()), origen, Convert.ToString(reader[8].ToString()), Convert.ToString(reader[7].ToString()));
                                    }
                                    else
                                    {
                                        double preci = Convert.ToDouble(reader[2].ToString());
                                        dataGridView1.Rows.Add("1", Convert.ToString(reader[1].ToString()), String.Format("{0:0.00}", preci), String.Format("{0:0.00}", preci), Convert.ToString(reader[4].ToString()), Convert.ToString(reader[0].ToString()), origen, Convert.ToString(reader[8].ToString()), Convert.ToString(reader[7].ToString()));
                                    }
                                }
                            }
                        }
                        textBox1.Text = "";
                    }
                    else
                    {
                        using (frmBuscarProductos buscar = new frmBuscarProductos())
                        {
                            buscar.textBox1.Text = textBox1.Text;
                            if (buscar.ShowDialog() == DialogResult.OK)
                            {
                                dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen, buscar.IVA, buscar.compra);
                            }
                        }
                        textBox1.Text = "";
                    }
                    CalcularTotales();
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            CalcularTotales();
            txtDescuento.Focus();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            CalcularTotales();
            txtDescuento.Focus();
        }

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                CalcularTotales();
                MessageBox.Show("DESCUENTO REALIZADO POR LA CANTIDAD DE: $" + descuento.ToString("0.00"), "DESCUENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtAbono.Focus();
                e.Handled = true;
            }
        }

        private void txtDescuento_Leave(object sender, EventArgs e)
        {
            CalcularTotales();
        }

        private void txtAbono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (txtAbono.Text == "")
                {
                    txtAbono.Text = "0";
                }
                CalcularTotales();
                button2.Focus();
                e.Handled = true;
            }
        }

        private void txtAbono_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAbono.Text))
            {
                txtAbono.Text = "0";
            }
            CalcularTotales();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[0].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[2].Value != null)
                {
                    double cantidad = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                    double precio = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                    double monto = cantidad * precio;

                    dataGridView1.Rows[e.RowIndex].Cells[3].Value = monto.ToString("0.00");
                }
            }
            catch
            {
                MessageBox.Show("Solo puedes introducir valores numéricos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dataGridView1.Rows[e.RowIndex].Cells[0].Value = "1";
                double precio = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                dataGridView1.Rows[e.RowIndex].Cells[3].Value = precio.ToString("0.00");
            }
            CalcularTotales();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                CalcularTotales();
            }
            else
            {
                MessageBox.Show("Seleccione un producto para eliminar de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        label10.Text = "Observaciones: " + observaciones;
                        label10.Visible = true;
                    }
                    else
                    {
                        label10.Text = "";
                        label10.Visible = false;
                    }
                }
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (Conexion.lugar == "TURBO LLANTAS")
            {
                if (e.ColumnIndex == 0 || e.ColumnIndex == 2)
                {
                    return;
                }
                var valorCelda5 = dataGridView1.Rows[e.RowIndex].Cells[5].Value;
                string verificador = valorCelda5 != null ? valorCelda5.ToString() : "";
                if (verificador != "0" && verificador != "00")
                {
                    e.Cancel = true;
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

        private void CalcularTotales()
        {
            double subtotal = 0;

            // Sumar los montos de la tabla de forma segura
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[3].Value != null)
                {
                    if (double.TryParse(dataGridView1.Rows[i].Cells[3].Value.ToString(), out double monto))
                    {
                        subtotal += monto;
                    }
                }
            }

            // Calcular el descuento de forma segura
            double valorDescuento = 0;
            if (double.TryParse(txtDescuento.Text, out double descInput))
            {
                if (radioButton1.Checked) // Porcentaje
                    valorDescuento = subtotal * (descInput / 100.0);
                else if (radioButton2.Checked) // Directo
                    valorDescuento = descInput;
            }

            double totalConDescuento = subtotal - valorDescuento;

            // Calcular abono de forma segura
            double valorAbono = 0;
            if (double.TryParse(txtAbono.Text, out double abonoInput))
            {
                valorAbono = abonoInput;
            }

            double saldoRestante = totalConDescuento - valorAbono;

            // Evitar saldos negativos (opcional, por seguridad)
            if (saldoRestante < 0) saldoRestante = 0;

            // Asignar variables globales que se usan en la Impresión
            this.total = subtotal;
            this.descuento = valorDescuento;

            // Actualizar interfaz
            lblTotal.Text = subtotal.ToString("0.00");
            lblFinal.Text = saldoRestante.ToString("0.00");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lblCliente.Text == "NO SELECCIONADO")
            {
                MessageBox.Show("Ingrese el cliente para poder hacer el apartado", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dataGridView1.RowCount == 0)
            {
                MessageBox.Show("Agregue al menos un producto para apartar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Forzar recálculo antes de guardar por seguridad total
            CalcularTotales();

            List<Producto> productos = new List<Producto>();
            existencia = 0;

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string unidad = "0";
                cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[5, i].Value.ToString() + "';", conectar);
                OleDbDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    exis = Convert.ToDouble(Convert.ToString(reader[4].ToString()));
                    unidad = Convert.ToString(reader[9].ToString());
                    if (unidad == "")
                    {
                        unidad = "0";
                    }
                }

                cmd = new OleDbCommand("select * from Unidades where Id=" + unidad + ";", conectar);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    unidad = Convert.ToString(reader[2].ToString());
                }
                else
                {
                    unidad = "";
                }

                existencia = exis - Convert.ToDouble(dataGridView1[0, i].Value.ToString());

                if (existencia < 0)
                {
                    cmd = new OleDbCommand("insert into VentasApartados(FolioVenta,idProducto,Cantidad,Producto,MontoTotal,IdCliente,Fecha,Estatus) values('" + lblFolio.Text + "','" + dataGridView1[5, i].Value.ToString() + "','" + dataGridView1[0, i].Value.ToString() + "','" + dataGridView1[1, i].Value.ToString() + "','" + dataGridView1[3, i].Value.ToString() + "','" + idCliente + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','PENDIENTE');", conectar);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // Actualizamos existencias
                    cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existencia + "' Where Id='" + dataGridView1[5, i].Value.ToString() + "';", conectar);
                    cmd.ExecuteNonQuery();

                    // Insertamos en la venta apartados
                    cmd = new OleDbCommand("insert into VentasApartados(FolioVenta,idProducto,Cantidad,Producto,MontoTotal,IdCliente,Fecha,Estatus) values('" + lblFolio.Text + "','" + dataGridView1[5, i].Value.ToString() + "','" + dataGridView1[0, i].Value.ToString() + "','" + dataGridView1[1, i].Value.ToString() + "','" + dataGridView1[3, i].Value.ToString() + "','" + idCliente + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','DESCONTADO DE INVENTARIO');", conectar);
                    cmd.ExecuteNonQuery();

                    cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[5, i].Value.ToString() + "','SALIDA','APARTADO DE ARTICULO FOLIO: " + lblFolio.Text + "'," + exis + ",'" + existencia + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                    cmd.ExecuteNonQuery();
                }

                productos.Add(new Producto
                {
                    Cantidad = Convert.ToDouble(dataGridView1[0, i].Value.ToString()),
                    Nombre = dataGridView1[1, i].Value.ToString(),
                    PrecioUnitario= Math.Round(Convert.ToDouble(dataGridView1[2, i].Value.ToString()) / 1.16, 2),
                    Total = Convert.ToDouble(dataGridView1[3, i].Value.ToString()),
                });
            }

            // Datos limpios para guardar sin errores
            double granTotal = this.total - this.descuento; // Total ya con descuento
            double abonoGuardar = 0;
            double.TryParse(txtAbono.Text, out abonoGuardar);
            double restanteGuardar = granTotal - abonoGuardar;

            // Inserción maestra a Apartados usando el Total Real
            cmd = new OleDbCommand("insert into Apartados (MontoTotal,Fecha,Folio,IdCliente,NombreCliente,Abono,Restante) values('" + granTotal + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + lblFolio.Text + "','" + idCliente + "','" + lblCliente.Text + "','" + abonoGuardar + "','" + restanteGuardar + "');", conectar);
            cmd.ExecuteNonQuery();

            cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Abono del apartado con folio " + lblFolio.Text + "','" + abonoGuardar + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + cmbPago.Text + "');", conectar);
            cmd.ExecuteNonQuery();

            // -- ÁREA DE IMPRESIÓN --
            Dictionary<string, double> totales = new Dictionary<string, double>();

            if (this.descuento != 0)
            {
                totales.Add("Descuento", this.descuento);
            }
            if (Conexion.ConIva)
            {
                totales.Add("Subtotal", granTotal / 1.16);
                totales.Add("IVA", (granTotal / 1.16) * 0.16);
            }
            totales.Add("Total", granTotal);
            totales.Add("Abono", abonoGuardar);
            totales.Add("Restante", restanteGuardar);

            if (Conexion.impresionMediaCarta)
            {
                try
                {
                    if (Conexion.lugar == "TURBO LLANTAS")
                    {   
                        DialogResult respuesta = MessageBox.Show(
                                "¿Deseas imprimir el Ticket?",
                                "IMPRESIÓN",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);
                        if (respuesta == DialogResult.Yes)
                        {
                            TicketMediaCarta pdfTicket = new TicketMediaCarta(
                                 productos,
                                 lblFolio.Text,
                                 this.descuento,
                                 granTotal,
                                 lblCliente.Text,
                                 idCliente,
                                 cmbPago.Text,
                                 lblDatosCotizacion.Text, // Datos
                                 label10.Text, // Observaciones / Tipo
                                 Conexion.lugar,
                                 Conexion.logoPath,    // Logo
                                 Conexion.datosTicket, // Encabezado
                                 Conexion.pieDeTicket  // Pie de página
                             );

                            pdfTicket.ImprimirDirectamente(Conexion.impresora);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al imprimir PDF Media Carta: " + ex.Message);
                }
            }
            else
            {
                TicketPrinter ticketPrinter = new TicketPrinter(Conexion.datosTicket, Conexion.pieDeTicket, Conexion.logoPath, productos, lblFolio.Text, "", lblCliente.Text, granTotal, false, totales, cmbPago.Text);
                ticketPrinter.ImprimirTicket();
            }

            foli = foli + 1;
            cmd = new OleDbCommand("UPDATE Folios set Numero=" + foli + " where Folio='FolioApartado';", conectar);
            cmd.ExecuteNonQuery();

            MessageBox.Show("Apartado realizado con éxito.", "APARTADO REALIZADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmApartado nuevoApartado = new frmApartado();
            nuevoApartado.Show();
            this.Close();
        }

    }
}