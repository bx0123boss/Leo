using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmVentasCredito : frmBase
    {
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        double total = 0;
        int foli;
        string origen = "";
        string idCliente = "0";
        double descuento;
        public string usuario = "";
        double exis = 0.0;
        string direccion;
        string datos = "", observaciones = "";

        public frmVentasCredito()
        {
            InitializeComponent();
            conectar.Open();
            AplicarEstilos();
        }

        private void AplicarEstilos()
        {
            cmbPago.SelectedIndex = 0;

            EstilizarDataGridView(this.dataGridView1);

            // Botones
            EstilizarBotonPrimario(this.button2);     // Botón Cobrar
            EstilizarBotonPrimario(this.button4);     // Botón Buscar Cliente
            EstilizarBotonAdvertencia(this.button3);  // Botón Buscar Producto
            EstilizarBotonAdvertencia(this.button5);  // Botón X (Cerrar)
            EstilizarBotonPeligro(this.button1);      // Botón Eliminar

            // Entradas de texto y opciones
            EstilizarComboBox(this.cmbPago);
            EstilizarTextBox(this.txtDescuento);
            EstilizarTextBox(this.txtAbono);
            EstilizarTextBox(this.textBox1);
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
                    lblAdeudo.Text = String.Format("{0:0.00}", cliente.Adeudo);
                    lblLimite.Text = cliente.Limite;
                }

                cliente.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (frmBuscarProductos buscar = new frmBuscarProductos())
            {
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen);
                    CalcularTotales();
                }
            }
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                CalcularTotales();
            }
        }

        private bool procesando = false;

        private void button2_Click(object sender, EventArgs e)
        {
            if (procesando) return;
            procesando = true;
            button2.Enabled = false;

            OleDbTransaction trans = null;

            try
            {
                if (lblCliente.Text == "NO SELECCIONADO")
                {
                    MessageBox.Show("Ingrese el cliente para poder hacer la venta", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                double adeudo = Convert.ToDouble(lblAdeudo.Text);
                double limite = Convert.ToDouble(lblLimite.Text);
                double totalVentaCalculo = Convert.ToDouble(lblFinal.Text);

                if (limite < (adeudo + totalVentaCalculo))
                {
                    MessageBox.Show("El limite sera excedido con la compra actual", "ALTO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // VALIDAR EXISTENCIAS
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    double existencia = Convert.ToDouble(dataGridView1[4, i].Value);
                    double cantidad = Convert.ToDouble(dataGridView1[0, i].Value);

                    if (existencia - cantidad < 0)
                    {
                        MessageBox.Show("Stock insuficiente en: " + dataGridView1[1, i].Value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // OBTENER FOLIO
                cmd = new OleDbCommand("SELECT Numero FROM Folios WHERE Folio='FolioCredito'", conectar);
                int foli = Convert.ToInt32(cmd.ExecuteScalar());
                string folioVenta = "C" + String.Format("{0:0000}", foli);

                // VALIDAR QUE NO EXISTA (doble ejecución)
                cmd = new OleDbCommand("SELECT COUNT(*) FROM VentasCredito WHERE FolioVenta = ?", conectar);
                cmd.Parameters.AddWithValue("@folio", folioVenta);
                int existe = Convert.ToInt32(cmd.ExecuteScalar());

                if (existe > 0)
                {
                    MessageBox.Show("Esta venta ya fue procesada.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // INICIAR TRANSACCIÓN
                trans = conectar.BeginTransaction();

                List<Producto> productos = new List<Producto>();

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    string idProducto = dataGridView1[5, i].Value.ToString();
                    double cantidad = Convert.ToDouble(dataGridView1[0, i].Value);
                    double precio = Convert.ToDouble(dataGridView1[2, i].Value);
                    double totalProd = Convert.ToDouble(dataGridView1[3, i].Value);

                    // OBTENER DATOS EXTRA
                    cmd = new OleDbCommand("SELECT Categoria FROM Inventario WHERE Id = ?", conectar, trans);
                    cmd.Parameters.AddWithValue("@id", idProducto);
                    string categoria = Convert.ToString(cmd.ExecuteScalar());

                    // 🔥 BONUS: UPDATE DIRECTO (evita errores de concurrencia)
                    cmd = new OleDbCommand("UPDATE Inventario SET Existencia = Existencia - ?, FechaUltimaVenta = NOW() WHERE Id = ?", conectar, trans);
                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@id", idProducto);
                    cmd.ExecuteNonQuery();

                    cmd = new OleDbCommand(@"
                    INSERT INTO VentasCredito
                    (FolioVenta, IdProducto, Cantidad, Producto, MontoTotal, IdCliente, Fecha, categoria)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?)", conectar, trans);

                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = folioVenta;
                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = dataGridView1[5, i].Value.ToString().Trim();
                    cmd.Parameters.Add("?", OleDbType.Double).Value = Convert.ToDouble(dataGridView1[0, i].Value);
                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = dataGridView1[1, i].Value.ToString();
                    cmd.Parameters.Add("?", OleDbType.Currency).Value = Convert.ToDecimal(dataGridView1[3, i].Value);
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = Convert.ToInt32(idCliente);
                    cmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;
                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = categoria ?? "";
                    cmd.ExecuteNonQuery();
                    
                    
                    //KARDEX
                    cmd = new OleDbCommand(@"
                    INSERT INTO Kardex 
                    (IdProducto, Tipo, Descripcion, Fecha)
                    VALUES (?, ?, ?, ?)", conectar, trans);
                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = idProducto.ToString().Trim();
                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = "SALIDA";
                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = "VENTA FOLIO: " + folioVenta;
                    cmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;
                    cmd.ExecuteNonQuery();

                    productos.Add(new Producto
                    {
                        Nombre = dataGridView1[1, i].Value.ToString(),
                        Cantidad = cantidad,
                        PrecioUnitario = Math.Round(precio / 1.16, 2),
                        Total = totalProd
                    });
                }

                double total = Convert.ToDouble(lblFinal.Text);
                double abono = Convert.ToDouble(txtAbono.Text);
                double nuevoAdeudo = adeudo + total;

                if (abono > 0)
                {
                    cmd = new OleDbCommand(@"
                        INSERT INTO Abonos (Abono,idCliente,Fecha,Nombre,Folio,Estatus)
                        VALUES (?,?,?,?,?,?)", conectar, trans);

                    cmd.Parameters.AddWithValue("?", txtAbono.Text);
                    cmd.Parameters.AddWithValue("?", idCliente);
                    cmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;
                    cmd.Parameters.AddWithValue("?", lblCliente.Text);
                    cmd.Parameters.AddWithValue("?", folioVenta);
                    cmd.Parameters.AddWithValue("?", "PAGADO");

                    cmd.ExecuteNonQuery();
                }
                cmd = new OleDbCommand("insert into Ventas2(Monto,Fecha,Folio,IdCliente,Estatus, Saldo) values('" + lblTotal.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + folioVenta + "','" + idCliente + "','COBRADO'," + total + ");", conectar, trans);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Abono de la venta a credito folio " + folioVenta + "','" + txtAbono.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + cmbPago.Text + "');", conectar,trans);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("UPDATE Clientes SET Adeudo = ? WHERE Id = ?", conectar, trans);
                cmd.Parameters.AddWithValue("@adeudo", nuevoAdeudo);
                cmd.Parameters.AddWithValue("@id", idCliente);
                cmd.ExecuteNonQuery();

                // ACTUALIZAR FOLIO
                cmd = new OleDbCommand("UPDATE Folios SET Numero = Numero + 1 WHERE Folio='FolioCredito'", conectar, trans);
                cmd.ExecuteNonQuery();

                // COMMIT
                trans.Commit();

                MessageBox.Show("Venta realizada con éxito", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                try { trans?.Rollback(); } catch { }
                MessageBox.Show("Error en la venta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                procesando = false;
                button2.Enabled = true;
            }
        }
        private void frmVentasCredito_Load(object sender, EventArgs e)
        {
            cmbPago.SelectedIndex = 0;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox1.Text == "")
                {
                }
                else
                {
                    cmd = new OleDbCommand("select count(*) from Inventario where Id='" + textBox1.Text + "';", conectar);
                    int valor = int.Parse(cmd.ExecuteScalar().ToString());
                    if (valor == 1)
                    {
                        total = Convert.ToDouble(lblTotal.Text);
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
                                    textBox1.Clear();
                                    textBox1.Focus();
                                }
                            }
                        }
                    }
                    else
                    {

                        using (frmBuscarProductos buscar = new frmBuscarProductos())
                        {
                            buscar.textBox1.Text = textBox1.Text;
                            if (buscar.ShowDialog() == DialogResult.OK)
                                dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen, buscar.IVA, buscar.compra);

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
            }
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
            }

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double cantidad = Convert.ToDouble(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
                double precio = Convert.ToDouble(dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString());
                double monto = cantidad * precio;
                dataGridView1.Rows[e.RowIndex].Cells[3].Value = String.Format("{0:0.00}", monto);
                textBox1.Focus();
            }
            catch
            {
                MessageBox.Show("Solo puedes introducir números", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView1.Rows[e.RowIndex].Cells[0].Value = "1";
                double cantidad = 1;
                double precio = Convert.ToDouble(dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString());
                double monto = cantidad * precio;
                dataGridView1.Rows[e.RowIndex].Cells[3].Value = String.Format("{0:0.00}", monto);
                textBox1.Focus();
            }
            CalcularTotales();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CalcularTotales()
        {
            double subtotal = 0;

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[3].Value != null)
                {
                    subtotal += Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value);
                }
            }

            double valorDescuento = 0;
            if (double.TryParse(txtDescuento.Text, out double descInput))
            {
                if (radioButton1.Checked) // Porcentaje
                    valorDescuento = subtotal * (descInput / 100.0);
                else if (radioButton2.Checked) // Directo
                    valorDescuento = descInput;
            }

            double totalConDescuento = subtotal - valorDescuento;

            double valorAbono = 0;
            if (double.TryParse(txtAbono.Text, out double abonoInput))
            {
                valorAbono = abonoInput;
            }

            double saldoRestante = totalConDescuento - valorAbono;

            this.descuento = valorDescuento;
            lblTotal.Text = subtotal.ToString("0.00");
            lblFinal.Text = saldoRestante.ToString("0.00");
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


    }
}
