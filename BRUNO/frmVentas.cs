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
    public partial class frmVentas : frmBase
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        double total = 0;
        double iva;
        string origen = "";
        double exis = 0.0;
        string idCliente = "0";
        double descuento;
        string datos = "", observaciones="", direccion;
        int foli;
        public string usuario = "", idUsuario = "";
        public frmVentas()
        {
            InitializeComponent();
            this.MinimumSize = new Size(1066, 418);
           
        }
        private void frmVentas_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPrimario(this.button2);
            EstilizarBotonPrimario(this.button4);
            EstilizarBotonAdvertencia(this.button3);
            EstilizarBotonAdvertencia(this.button5);
            EstilizarBotonPeligro(this.button1);
            EstilizarTextBox(this.txtDescuento);
            EstilizarTextBox(this.textBox1);
            conectar.Open();
            dataGridView1.Columns[0].ReadOnly = false;
            if (Conexion.lugar == "DEPORTES LEO")
            {
                dataGridView1.Columns[2].ReadOnly = false;
            }
            if (Conexion.lugar == "TURBO LLANTAS")
            {
                dataGridView1.Columns[2].ReadOnly = false;
                dataGridView1.Columns[1].ReadOnly = false;
                dataGridView1.Columns[0].ReadOnly = false;
            }
            else if (Conexion.lugar == "SANJUAN" && usuario == "Admin")
            {
                dataGridView1.Columns[2].ReadOnly = false;
            }
        }
        public void ReiniciarForm()
        {
            conectar.Close();
            total = 0;
            observaciones = "";
            datos = "";
            iva = 0;
            origen = "";
            exis = 0.0;
            idCliente = "0";
            descuento = 0;
            foli = 0;
            direccion = "";
            txtDescuento.Text = "";
            lblFolio.Text = "0";
            lblFolio.Visible = false;
            label2.Visible = false;
            dataGridView1.Rows.Clear();
            lblTotal.Text = $"{RecalcularTotal:C}";
            lblCliente.Text = "PUBLICO EN GENERAL";
            txtFolioCotizacion.Enabled = true;
            txtFolioCotizacion.Text = "";
            label9.Text = "";
            button5.Text = "Buscar";
            dataGridView1.Rows.Clear();
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            txtDescuento.Text = "";
            lblDatosCotizacion.Text = "Sin datos extra";
            conectar.Open();
        }
        private double RecalcularTotal
        {
            get
            {
                total = 0;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    total += Convert.ToDouble(dataGridView1.Rows[i].Cells["Monto"].Value);
                }
                return total - descuento;
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


        }
        private void ProcesarPreventaQR(string cadenaQR)
        {
            try
            {
                string datos = cadenaQR.Substring(4);
                string[] items = datos.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in items)
                {
                    string[] partes = item.Split(':');
                    if (partes.Length == 2)
                    {
                        string idProducto = partes[0];
                        double cantidad = double.Parse(partes[1], System.Globalization.CultureInfo.InvariantCulture);

                        // 3. Buscar datos completos del producto en la BD
                        cmd = new OleDbCommand("select * from Inventario where Id='" + idProducto + "';", conectar);
                        OleDbDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            // En preventa tomamos el precio del índice 2 (Público) automáticamente
                            double precio = Convert.ToDouble(reader[3].ToString());
                            double importe = precio * cantidad;

                            // 4. Agregar al Grid con todos tus campos
                            dataGridView1.Rows.Add(
                                cantidad.ToString(),
                                reader[1].ToString(),             // Nombre/Descripción
                                String.Format("{0:0.00}", precio), // Precio Unitario
                                String.Format("{0:0.00}", importe),// Importe Total
                                reader[4].ToString(),             // Existencia
                                reader[0].ToString(),             // ID
                                origen,                           // Variable de tu formulario
                                reader[8].ToString(),             // IVA
                                reader[7].ToString(),             // Precio Compra
                                "",                               // Comentario vacío
                                "X"                               // Botón borrar
                            );
                        }
                        reader.Close();
                    }
                }
                lblTotal.Text = $"{RecalcularTotal:C}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar QR de preventa: " + ex.Message);
            }
        }
        private double ObtenerPesoLocal()
           {
            if (!frmPrincipal.IsAgenteBasculaActivo)
            {
                return 1;
            }
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://localhost:8080/peso/");
                request.Timeout = 300; // Espera máximo 300 milisegundos
                request.Method = "GET";

                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                using (System.IO.StreamReader stream = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string respuesta = stream.ReadToEnd();
                    if (respuesta != "ERROR")
                    {
                        double peso = double.Parse(respuesta, System.Globalization.CultureInfo.InvariantCulture);
                        return peso;
                    }
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text)) return;
                string input = textBox1.Text.Trim();

                if (input.StartsWith("PRE:"))
                {
                    ProcesarPreventaQR(input);
                    textBox1.Clear();
                    e.Handled = true;
                    return;
                }

                cmd = new OleDbCommand("select count(*) from Inventario where Id='" + input + "';", conectar);
                int valor = int.Parse(cmd.ExecuteScalar().ToString());

                if (valor == 1)
                {
                    cmd = new OleDbCommand("select * from Inventario where Id='" + input + "';", conectar);
                    OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        using (frmPrecio buscar = new frmPrecio())
                        {
                            if (buscar.ShowDialog() == DialogResult.OK)
                            {
                                double preci = 0;
                                // Si tipo es GEN usa índice 3, si no usa índice 2
                                preci = (buscar.tipo == "GEN") ? Convert.ToDouble(reader[3]) : Convert.ToDouble(reader[2]);
                                double cantidad = ObtenerPesoLocal();
                                if (cantidad <= 0) cantidad = 1; double importe = preci * cantidad; // Recalcula el importe

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

                            if (cantidad <= 0)
                            {
                                cantidad = 1;
                            }

                            double precioUnitario = Convert.ToDouble(buscar.precio);
                            double importeCalculado = precioUnitario * cantidad;

                            // 4. Agregamos al DataGridView con la cantidad real
                            dataGridView1.Rows.Add(
                                cantidad.ToString(),                    // Cantidad (Báscula o 1)
                                buscar.producto,                        // Descripción
                                String.Format("{0:0.00}", precioUnitario), // Precio
                                String.Format("{0:0.00}", importeCalculado), // Importe totalizado
                                buscar.existencia,
                                buscar.ID,
                                origen,
                                buscar.IVA,
                                buscar.compra,
                                "",
                                "X"
                            );

                            lblTotal.Text = $"{RecalcularTotal:C}";
                        }
                    }
                    textBox1.Text = "";
                }

                e.Handled = true;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            EliminarProductos();
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
                textBox1.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar filas: {ex.Message}");
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
                lblTotal.Text = $"{RecalcularTotal:C}";
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
                lblTotal.Text = $"{RecalcularTotal:C}";
                textBox1.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Venta();
        }




        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            txtDescuento.Focus();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            txtDescuento.Focus();
        }

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
                MessageBox.Show("Solo se permiten numeros", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (radioButton1.Checked)
                {

                    descuento = ((Convert.ToDouble(txtDescuento.Text) / 100)) * total;
                    MessageBox.Show("DESCUENTO REALIZADO POR LA CANTIDAD DE: $" + descuento, "DESCUENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblTotal.Text = $"{RecalcularTotal:C}";
                }
                else if (radioButton2.Checked)
                {

                    descuento = Convert.ToDouble(txtDescuento.Text);
                    total = total - descuento;
                    MessageBox.Show("DESCUENTO REALIZADO POR LA CANTIDAD DE: $" + descuento, "DESCUENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblTotal.Text = $"{RecalcularTotal:C}";
                }
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
                }

                cliente.Show();
            }
        }

        public void Venta()
        {
            if (total == 0)
            {
                MessageBox.Show("No se puede realizar una venta sin productos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double efectivo = 0, cambio = 0;
            Dictionary<string, double> pagosFinales = new Dictionary<string, double>();

            using (frmPago ori = new frmPago())
            {
                ori.total = (total - descuento);
                ori.txtTotal.Text = $"{ori.total:C}";
                if (ori.ShowDialog() == DialogResult.OK)
                {
                    efectivo = ori.efectivo;
                    cambio = ori.cambio;
                    pagosFinales = ori.PagosRealizados;
                }
                else
                {
                    return;
                }
            }

            using (var con = new OleDbConnection(Conexion.CadCon))
            {
                con.Open();
                using (var transaccion = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new OleDbCommand("select Numero from Folios where Folio='FolioContado';", con, transaccion))
                        {
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    foli = Convert.ToInt32(Convert.ToString(reader[0].ToString()));
                                }
                            }
                        }

                        if (Conexion.lugar == "TURBO LLANTAS")
                            lblFolio.Text = "TB" + String.Format("{0:0000}", foli);
                        else
                            lblFolio.Text = "VR" + String.Format("{0:0000}", foli);

                        lblFolio.Visible = true;
                        label2.Visible = true;

                        double IVA = 0;
                        double existencia = 0;
                        double totalUtilidad = 0;
                        List<Producto> productos = new List<Producto>();
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            double venta = Convert.ToDouble(dataGridView1[2, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                            double compra = Convert.ToDouble(dataGridView1[8, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                            totalUtilidad += (venta - compra);
                        }

                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            string idProd = dataGridView1[5, i].Value.ToString();
                            double cantidad = Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                            double precioVenta = Math.Round(Convert.ToDouble(dataGridView1[2, i].Value.ToString()) / 1.16,2);
                            double montoFila = Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                            string nombreProd = dataGridView1[1, i].Value.ToString();
                            double precioCompra = Convert.ToDouble(dataGridView1[8, i].Value.ToString());

                            productos.Add(new Producto
                            {
                                Nombre = nombreProd,
                                Cantidad = cantidad,
                                PrecioUnitario = precioVenta,
                                Total = montoFila,
                            });

                            string categoria = "";

                            using (var cmd = new OleDbCommand("select Existencia, Categoria from Inventario where Id='" + idProd + "';", con, transaccion))
                            {
                                using (var reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        exis = Convert.ToDouble(reader["Existencia"].ToString());
                                        categoria = reader["Categoria"].ToString();
                                    }
                                }
                            }

                            existencia = exis - cantidad;

                            using (var cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existencia + "' Where Id='" + idProd + "';", con, transaccion))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            using (var cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + idProd + "','SALIDA','VENTA DE ARTICULO FOLIO: " + lblFolio.Text + "'," + exis + ",'" + existencia + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", con, transaccion))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            double utilidad = (precioVenta * cantidad) - (precioCompra * cantidad);
                            double descuentoProporcional = totalUtilidad > 0 ? (utilidad / totalUtilidad) * descuento : 0;
                            double nuevaUtilidad = utilidad - descuentoProporcional;

                            string queryVentasContado = "insert into VentasContado(FolioVenta,IdProducto,Cantidad,Producto,MontoTotal,idCliente,Fecha,Utilidad, Categoria) values('" + lblFolio.Text + "','" + idProd + "','" + cantidad + "','" + nombreProd + "','" + montoFila + "','" + (string.IsNullOrEmpty(idCliente) ? "0" : idCliente) + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + nuevaUtilidad + "','" + categoria + "');";
                            using (var cmd = new OleDbCommand(queryVentasContado, con, transaccion))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            string precioStr = "" + Math.Round(montoFila, 2);
                            if (dataGridView1[7, i].Value.ToString() == "IVA(16)")
                            {
                                IVA += Convert.ToDouble(precioStr) - (Convert.ToDouble(precioStr) / 1.16);
                            }
                        }
                        total = 0;
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                        }

                        Dictionary<string, double> totales = new Dictionary<string, double>();
                        if (descuento != 0)
                        {
                            total = total - descuento;
                            totales.Add("Descuento", descuento);
                        }
                        if (Conexion.ConIva)
                        {
                            totales.Add("Subtotal", total / 1.16);
                            totales.Add("IVA", (total / 1.16) * 0.16);
                        }
                        totales.Add("Total", total);
                        totales.Add("Recibido", efectivo);
                        totales.Add("Cambio", cambio);

                        string tipoPagoVenta = pagosFinales.Count > 1 ? "MIXTO" : pagosFinales.First().Key;
                        if (Conexion.impresionMediaCarta)
                        {
                            DialogResult respuesta = MessageBox.Show("¿Deseas imprimir?", "IMPRESIÓN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (respuesta == DialogResult.Yes)
                            {
                                TicketMediaCarta pdfTicket = new TicketMediaCarta(productos, lblFolio.Text, descuento, total, lblCliente.Text, idCliente, tipoPagoVenta, datos, observaciones, Conexion.lugar, Conexion.logoPath, Conexion.datosTicket, Conexion.pieDeTicket);
                                pdfTicket.ImprimirDirectamente(Conexion.impresora);
                            }
                        }
                        else
                        {
                            TicketPrinter ticketPrinter = new TicketPrinter(Conexion.datosTicket, Conexion.pieDeTicket, Conexion.logoPath, productos, lblFolio.Text, "", "", total, false, totales, tipoPagoVenta);
                            ticketPrinter.ImprimirTicket();
                        }
                        using (var cmd = new OleDbCommand("insert into Ventas(Monto,Fecha,Folio,Estatus, Descuento, Pago) values('" + total + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + lblFolio.Text + "','COBRADO','" + descuento + "','" + tipoPagoVenta + "');", con, transaccion))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        foreach (var pago in pagosFinales)
                        {
                            if (pago.Value > 0)
                            {
                                using (var cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Venta a contado folio " + lblFolio.Text + "','" + pago.Value + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + pago.Key + "');", con, transaccion))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        using (var cmd = new OleDbCommand("insert into VentasCajero(IdUsuario,Usuario,FolioVenta,Total,Fecha,Cajero) values('" + idUsuario + "','" + lblUsuario.Text + "','" + lblFolio.Text + "','" + total + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + lblCajero.Text + "');", con, transaccion))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        foli = foli + 1;
                        using (var cmd = new OleDbCommand("UPDATE Folios set Numero = " + foli + " where Folio = 'FolioContado'; ", con, transaccion))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        // ----------------------------------------------------------------------------------
                        // NUEVO: GUARDAR HISTORIAL DE DATOS EXTRA EN SQL SERVER (Para el autocompletado)
                        // ----------------------------------------------------------------------------------
                        if (!string.IsNullOrEmpty(datos) && !string.IsNullOrEmpty(idCliente) && idCliente != "0")
                        {
                            try
                            {
                                using (SqlConnection conSql = new SqlConnection(Conexion.CadSQL))
                                {
                                    conSql.Open();

                                    if (!string.IsNullOrEmpty(txtFolioCotizacion.Text))
                                    {
                                        // Si la venta viene de una cotización, simplemente actualizamos sus datos para mantener el historial fresco
                                        string queryUpdate = "UPDATE Cotizaciones SET Datos = @Datos, Observaciones = @Obs WHERE Id = @FolioCot";
                                        using (SqlCommand cmdSql = new SqlCommand(queryUpdate, conSql))
                                        {
                                            cmdSql.Parameters.AddWithValue("@Datos", datos);
                                            cmdSql.Parameters.AddWithValue("@Obs", observaciones);
                                            cmdSql.Parameters.AddWithValue("@FolioCot", txtFolioCotizacion.Text.Trim());
                                            cmdSql.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        // Si es venta directa, creamos una cotización "fantasma" con Total = -1 para que sirva de historial
                                        string queryInsert = @"INSERT INTO Cotizaciones 
                                     (Fecha, ClienteId, ClienteNombre, Total, Observaciones, Datos) 
                                     VALUES (GETDATE(), @CId, @CNom, -1, 'HISTORIAL OCULTO - VENTA DIRECTA', @Datos)";
                                        using (SqlCommand cmdSql = new SqlCommand(queryInsert, conSql))
                                        {
                                            cmdSql.Parameters.AddWithValue("@CId", idCliente);
                                            cmdSql.Parameters.AddWithValue("@CNom", lblCliente.Text);
                                            cmdSql.Parameters.AddWithValue("@Datos", datos);
                                            cmdSql.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            catch (Exception exHistorial)
                            {
                                // Solo mostramos en consola para no interrumpir el flujo de la venta si falla el SQL
                                Console.WriteLine("Error al guardar historial extra: " + exHistorial.Message);
                            }
                        }
                        // ----------------------------------------------------------------------------------

                        ReiniciarForm();
                        MessageBox.Show(this, "Venta realizada con éxito", "VENTA REALIZADA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        MessageBox.Show("Error al procesar la venta. Se ha cancelado todo el movimiento para evitar descuadres. Detalle: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void frmVentas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                Venta();
            }
            if (e.KeyCode == Keys.Delete)
            {
                EliminarProductos();
            }
        }
        private void lblUsuario_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            using (frmClaveVendedor ori = new frmClaveVendedor())
            {
                if (ori.ShowDialog() == DialogResult.OK)
                {
                    lblUsuario.Text = ori.Usuario;
                    idUsuario = ori.Id.ToString();
                }
            }
        }

        private void txtDescuento_Leave(object sender, EventArgs e)
        {
            // Verifica si el campo está vacío o no es un número
            if (string.IsNullOrWhiteSpace(txtDescuento.Text) || !decimal.TryParse(txtDescuento.Text, out _))
            {
                // Muestra un mensaje de error o maneja el caso como desees
                MessageBox.Show("Por favor, ingrese un valor numérico válido.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else
            {
                if (radioButton1.Checked)
                {
                    total = 0;
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                    }
                    descuento = ((Convert.ToDouble(txtDescuento.Text) / 100)) * total;
                    MessageBox.Show("DESCUENTO REALIZADO POR LA CANTIDAD DE: $" + descuento, "DESCUENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblTotal.Text = $"{RecalcularTotal:C}";
                }
                else if (radioButton2.Checked)
                {
                    total = 0;
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                    }
                    descuento = Convert.ToDouble(txtDescuento.Text);
                    MessageBox.Show("DESCUENTO REALIZADO POR LA CANTIDAD DE: $" + descuento, "DESCUENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblTotal.Text = $"{RecalcularTotal:C}";
                }
            }
        }

        private void txtFolioCotizacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(txtFolioCotizacion.Text))
                {
                    CargarCotizacionWeb(txtFolioCotizacion.Text.Trim());
                    txtFolioCotizacion.Enabled = false;
                    button5.Text = "Borrar";
                }
            }
        }
        private void CargarCotizacionWeb(string folio)
        {
            
            string query = @"
                   SELECT 
                        C.ClienteId,
	                    C.ClienteNombre,
	                    C.Id,
	                    DC.ProductoCodigo,
	                    DC.Descripcion,
	                    DC.Cantidad,
	                    DC.PrecioUnitario,
	                    DC.Importe,
                        C.Datos,
                        C.Observaciones
                    FROM Cotizaciones C
                    INNER JOIN DetalleCotizacion DC ON C.Id = DC.CotizacionId
                    WHERE C.Id = @Folio";

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.CadSQL))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Folio", folio);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                MessageBox.Show("Cotización no encontrada o inactiva.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            while (reader.Read())
                            {
                                idCliente = reader["ClienteId"].ToString();
                                lblCliente.Text = reader["ClienteNombre"].ToString();
                                string codigoProd = reader["ProductoCodigo"].ToString();
                                string descripcion = reader["Descripcion"].ToString();
                                double cantidad = Convert.ToDouble(reader["Cantidad"]);
                                double precioUni = Convert.ToDouble(reader["PrecioUnitario"]);
                                double tolProducto = Convert.ToDouble(reader["Importe"]);
                                observaciones = reader["Observaciones"].ToString();
                                datos = reader["Datos"].ToString();
                                AgregarProductoAVenta(codigoProd, cantidad, descripcion,precioUni, tolProducto);
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar cotización: " + ex.Message);
            }
            if (!string.IsNullOrEmpty(datos))
            {
                string textoVisual = datos.Replace(";", "   |   ");

                lblDatosCotizacion.Text = textoVisual;
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

        private void AgregarProductoAVenta(string codigoProd, double cantidad, string descripcion, double precioUni, double tolProducto)
        {
            cmd = new OleDbCommand("select * from Inventario where Id='" + codigoProd + "';", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                dataGridView1.Rows.Add(cantidad, descripcion, String.Format("{0:0.00}", precioUni), String.Format("{0:0.00}", tolProducto), Convert.ToString(reader[4].ToString()), codigoProd, origen, Convert.ToString(reader[8].ToString()), Convert.ToString(reader[7].ToString()), "", "X");
            }
            lblTotal.Text = $"{RecalcularTotal:C}";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (txtFolioCotizacion.Enabled == false)
            {
                ReiniciarForm(); // Limpia grid, cliente, totales, etc.
                txtFolioCotizacion.Enabled = true; // Reactivamos el campo
                txtFolioCotizacion.Text = "";
                button5.Text = "Buscar"; // Regresamos el texto a su estado original
                datos = "";
                observaciones = "";
                txtFolioCotizacion.Focus();
                return;
            }

            using (frmCotizacion historial = new frmCotizacion())
            {
                if (historial.ShowDialog() == DialogResult.OK)
                {
                    txtFolioCotizacion.Text = historial.Folio;
                    CargarCotizacionWeb(historial.Folio);
                    txtFolioCotizacion.Enabled = false;
                    button5.Text = "Borrar";
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
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == dataGridView1.Columns["btnEliminar"].Index && e.RowIndex >= 0)
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                lblTotal.Text = $"{RecalcularTotal:C}";
            }
        }
        protected override void FrmBase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
            }
            else
            {
                base.FrmBase_KeyDown(sender, e);
            }
        }
    }
}
