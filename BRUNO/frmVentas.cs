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
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        double total = 0;
        double iva;
        string origen = "";
        double exis = 0.0;
        string idCliente = "0";
        double descuento;
        string datos = "", observaciones="";
        string direccion;
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
            EstilizarComboBox(this.cmbPago);
            EstilizarTextBox(this.txtDescuento);
            EstilizarTextBox(this.textBox1);
            EstilizarCheckBox(this.checkBox1);
            conectar.Open();
            dataGridView1.Columns[0].ReadOnly = false;
            cmbPago.SelectedIndex = 0;
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
            checkBox1.Checked = false;
            txtFolioCotizacion.Enabled = true;
            txtFolioCotizacion.Text = "";
            label9.Text = "";
            button5.Text = "Buscar";
            dataGridView1.Rows.Clear();
            string[] opcionesPago = {
                    "01=EFECTIVO",
                    "02=CHEQUE NOMINATIVO",
                    "03=TRANFERENCIA ELECTRONICA DE FONDOS",
                    "05=MONEDERO ELECTRONICO",
                    "06=DINERO ELECTRONICO",
                    "08=VALES DE DESPENSA",
                    "12=DACION EN PAGO",
                    "13=PAGO POR SUBROGACION",
                    "14=PAGO POR CONSIGNACION",
                    "15=CONDONACION",
                    "17=COMPENSACION",
                    "23=NOVACION",
                    "24=CONFUSION",
                    "25=REMISION DE DEUDA",
                    "26=PRESCRIPCION O CADUCIDAD",
                    "27=A SATISFACCION DEL ACREEDOR",
                    "29=TARJETA DE SERVICIOS",
                    "30=APLICACION DE ANTICIPOS",
                    "31=INTERMEDIARIO PAGOS",
                    "99=POR DEFINIR"
                };
            cmbPago.Items.Clear();
            cmbPago.Items.AddRange(opcionesPago);
            if (cmbPago.Items.Count > 0)
            {
                cmbPago.SelectedIndex = 0;
            }
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            txtDescuento.Text = "";
            AjustarAnchoDropDown(this.cmbPago);
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
                    dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen, buscar.IVA, buscar.compra,"","X");

                }
            }

            lblTotal.Text = $"{RecalcularTotal:C}";


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
                        cmd = new OleDbCommand("select * from Inventario where Id='" + textBox1.Text + "';", conectar);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            // AQUI FALTABA ABRIR EL DIALOGO Y SELECCIONAR EL PRECIO
                            using (frmPrecio buscar = new frmPrecio())
                            {
                                if (buscar.ShowDialog() == DialogResult.OK)
                                {
                                    double preci = 0;
                                    if (buscar.tipo == "GEN")
                                    {
                                        preci = Convert.ToDouble(reader[3].ToString());
                                    }
                                    else
                                    {
                                        preci = Convert.ToDouble(reader[2].ToString());
                                    }

                                    dataGridView1.Rows.Add("1", Convert.ToString(reader[1].ToString()), String.Format("{0:0.00}", preci), String.Format("{0:0.00}", preci), Convert.ToString(reader[4].ToString()), Convert.ToString(reader[0].ToString()), origen, Convert.ToString(reader[8].ToString()), Convert.ToString(reader[7].ToString()), "", "X");
                                }
                            }
                        }
                        lblTotal.Text = $"{RecalcularTotal:C}";
                        textBox1.Text = "";
                    }
                    else
                    {
                        using (frmBuscarProductos buscar = new frmBuscarProductos())
                        {
                            buscar.textBox1.Text = textBox1.Text;
                            if (buscar.ShowDialog() == DialogResult.OK)
                            {
                                // Como ya arreglamos frmBuscarProductos en el paso anterior, 
                                // buscar.precio ya trae el precio correcto (GEN o MAYOREO)
                                dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen, buscar.IVA, buscar.compra, "", "X");
                            }
                        }
                        lblTotal.Text = $"{RecalcularTotal:C}";
                        textBox1.Text = "";
                    }
                }
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.Checked = true;
                cmbPago.Items.Clear();
                cmbPago.Items.Add("04=TARJETA DE CREDITO");
                cmbPago.Items.Add("28=TARJETA DE DEBITO");
                cmbPago.SelectedIndex = 0;

            }
            else
            {
                // Lista de opciones para el ComboBox
                string[] opcionesPago = {
                    "01=EFECTIVO",
                    "02=CHEQUE NOMINATIVO",
                    "03=TRANFERENCIA ELECTRONICA DE FONDOS",
                    "05=MONEDERO ELECTRONICO",
                    "06=DINERO ELECTRONICO",
                    "08=VALES DE DESPENSA",
                    "12=DACION EN PAGO",
                    "13=PAGO POR SUBROGACION",
                    "14=PAGO POR CONSIGNACION",
                    "15=CONDONACION",
                    "17=COMPENSACION",
                    "23=NOVACION",
                    "24=CONFUSION",
                    "25=REMISION DE DEUDA",
                    "26=PRESCRIPCION O CADUCIDAD",
                    "27=A SATISFACCION DEL ACREEDOR",
                    "29=TARJETA DE SERVICIOS",
                    "30=APLICACION DE ANTICIPOS",
                    "31=INTERMEDIARIO PAGOS",
                    "99=POR DEFINIR"
                };
                cmbPago.Items.Clear();
                cmbPago.Items.AddRange(opcionesPago);
                if (cmbPago.Items.Count > 0)
                {
                    cmbPago.SelectedIndex = 0;
                }
                AjustarAnchoDropDown(cmbPago);
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

            // 1. Declaramos el diccionario AFUERA del using para que exista después de que se cierre frmPago
            Dictionary<string, double> pagosFinales = new Dictionary<string, double>();

            using (frmPago ori = new frmPago())
            {
                ori.total = (total - descuento);
                ori.txtTotal.Text = $"{ori.total:C}";
                if (ori.ShowDialog() == DialogResult.OK)
                {
                    efectivo = ori.efectivo;
                    cambio = ori.cambio;

                    // 2. Extraemos la lista de pagos antes de que 'ori' se destruya
                    pagosFinales = ori.PagosRealizados;
                }
                else
                {
                    return; // Si cancela el pago, cancelamos la venta
                }
            }

            cmd = new OleDbCommand("select Numero from Folios where Folio='FolioContado';", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                foli = Convert.ToInt32(Convert.ToString(reader[0].ToString()));
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
                double utilidad = venta - compra;

                totalUtilidad += utilidad; // Suma a la utilidad total
            }
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                productos.Add(new Producto
                {
                    Nombre = dataGridView1[1, i].Value.ToString(),
                    Cantidad = Convert.ToDouble(dataGridView1[0, i].Value.ToString()),
                    PrecioUnitario = Convert.ToDouble(dataGridView1[2, i].Value.ToString()),
                    Total = Convert.ToDouble(dataGridView1[3, i].Value.ToString()),
                });
                string unidad = "0";
                string categoria = "";
                //Obtenemos existencias del articulo
                cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[5, i].Value.ToString() + "';", conectar);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    exis = Convert.ToDouble(Convert.ToString(reader[4].ToString()));
                    unidad = Convert.ToString(reader[9].ToString());
                    categoria = Convert.ToString(reader["Categoria"].ToString());
                    unidad = "pz";
                    if (unidad == "")
                    {
                        unidad = "0";
                    }

                }
                existencia = exis - Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                //Actualizamos existencias
                cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existencia + "' Where Id='" + dataGridView1[5, i].Value.ToString() + "';", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[5, i].Value.ToString() + "','SALIDA','VENTA DE ARTICULO FOLIO: " + lblFolio.Text + "'," + exis + ",'" + existencia + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                cmd.ExecuteNonQuery();
                //Insertamos en la venta a credito
                double venta = Convert.ToDouble(dataGridView1[2, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                double compra = Convert.ToDouble(dataGridView1[8, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                double utilidad = venta - compra;
                // Calcula el descuento proporcional para este producto
                double descuentoProporcional = (utilidad / totalUtilidad) * descuento;
                double nuevaUtilidad = utilidad - descuentoProporcional;

                cmd = new OleDbCommand("insert into VentasContado(FolioVenta,IdProducto,Cantidad,Producto,MontoTotal,idCliente,Fecha,Utilidad, Categoria) values('" + lblFolio.Text + "','" + dataGridView1[5, i].Value.ToString() + "','" + dataGridView1[0, i].Value.ToString() + "','" + dataGridView1[1, i].Value.ToString() + "','" + dataGridView1[3, i].Value.ToString() + "','" + (string.IsNullOrEmpty(idCliente) ? "0" : idCliente) + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + nuevaUtilidad + "','" + categoria + "');", conectar);
                cmd.ExecuteNonQuery();
                string precio = "" + Math.Round(Convert.ToDouble(dataGridView1[3, i].Value.ToString()), 2);
                if (dataGridView1[7, i].Value.ToString() == "IVA(16)")
                {
                    IVA += Convert.ToDouble(precio) - (Convert.ToDouble(precio) / 1.16);
                }
            }
            double UtilidadTotal = 0;
            total = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                double venta = Convert.ToDouble(dataGridView1[2, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                double compra = Convert.ToDouble(dataGridView1[8, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                UtilidadTotal = UtilidadTotal + (venta - compra);
            }

            //Area para imprimir ticket
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

            // 3. Tomamos el método de pago principal (o "MIXTO") para la tabla general y el ticket
            string tipoPagoVenta = pagosFinales.Count > 1 ? "MIXTO" : pagosFinales.First().Key;

            if (Conexion.impresionMediaCarta)
            {
                try
                {
                    DialogResult respuesta = MessageBox.Show(
                            "¿Deseas imprimir?",
                            "IMPRESIÓN",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                    if (respuesta == DialogResult.Yes)
                    {
                        TicketMediaCarta pdfTicket = new TicketMediaCarta(
                             productos,
                             lblFolio.Text,
                             descuento,
                             total,
                             lblCliente.Text,
                             idCliente,
                             tipoPagoVenta, // Pasamos la nueva variable
                             datos,
                             observaciones,
                             Conexion.lugar,
                             Conexion.logoPath,    // <--- Logo
                             Conexion.datosTicket, // <--- Encabezado del negocio
                             Conexion.pieDeTicket  // <--- Pie de página
                         );

                        pdfTicket.ImprimirDirectamente(Conexion.impresora);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al imprimir PDF Media Carta: " + ex.Message);
                }
            }
            else
            {
                TicketPrinter ticketPrinter = new TicketPrinter(Conexion.datosTicket, Conexion.pieDeTicket, Conexion.logoPath, productos, lblFolio.Text, "", "", total, false, totales, tipoPagoVenta);
                ticketPrinter.ImprimirTicket();
            }

            cmd = new OleDbCommand("insert into Ventas(Monto,Fecha,Folio,Estatus, Descuento, Pago) values('" + (total - descuento) + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + lblFolio.Text + "','COBRADO','" + descuento + "','" + tipoPagoVenta + "');", conectar);
            cmd.ExecuteNonQuery();

            // 4. Mágia: Recorremos cada pago del diccionario FINAL (fuera del using) y lo metemos independiente a tu tabla CORTE. 
            foreach (var pago in pagosFinales)
            {
                if (pago.Value > 0)
                {
                    cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Venta a contado folio " + lblFolio.Text + "','" + pago.Value + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + pago.Key + "');", conectar);
                    cmd.ExecuteNonQuery();
                }
            }

            cmd = new OleDbCommand("insert into VentasCajero(IdUsuario,Usuario,FolioVenta,Total,Fecha,Cajero) values('" + idUsuario + "','" + lblUsuario.Text + "','" + lblFolio.Text + "','" + (total - descuento) + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + lblCajero.Text + "');", conectar);
            cmd.ExecuteNonQuery();

            foli = foli + 1;
            cmd = new OleDbCommand("UPDATE Folios set Numero=" + foli + " where Folio='FolioContado';", conectar);
            cmd.ExecuteNonQuery();

            ReiniciarForm();
            MessageBox.Show(this, "Venta realizada con exito", "VENTA REALIZADA", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void frmVentas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                Venta();
            }
            if (e.KeyCode == Keys.F1)
            {
                checkBox1.Checked = !checkBox1.Checked; // Alterna el estado
            }
            if (e.KeyCode == Keys.Delete)
            {
                EliminarProductos();
            }
        }
        // Función para dividir texto en múltiples líneas
        private List<string> DivideTexto(Graphics g, string texto, Font font, float maxWidth)
        {
            List<string> lineas = new List<string>();
            string[] palabras = texto.Split(' ');
            string lineaActual = "";

            foreach (string palabra in palabras)
            {
                string prueba = lineaActual + (lineaActual.Length > 0 ? " " : "") + palabra;
                if (g.MeasureString(prueba, font).Width <= maxWidth)
                {
                    lineaActual = prueba;
                }
                else
                {
                    if (lineaActual.Length > 0)
                    {
                        lineas.Add(lineaActual);
                        lineaActual = palabra;
                    }
                    else
                    {
                        // Palabra demasiado larga, partirla
                        for (int i = 0; i < palabra.Length; i++)
                        {
                            prueba = lineaActual + palabra[i];
                            if (g.MeasureString(prueba, font).Width > maxWidth)
                            {
                                lineas.Add(lineaActual);
                                lineaActual = "";
                                i--; // Reintentar este carácter
                            }
                            else
                            {
                                lineaActual = prueba;
                            }
                        }
                    }
                }
            }

            if (lineaActual.Length > 0)
                lineas.Add(lineaActual);

            return lineas;
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

                // Panel inferior (Fijo para los botones)
                Panel pnlBotones = new Panel();
                pnlBotones.Height = 70;
                pnlBotones.Dock = DockStyle.Bottom;

                // Panel superior (Dinámico para los TextBoxes con barra de scroll)
                Panel pnlCampos = new Panel();
                pnlCampos.Dock = DockStyle.Fill;
                pnlCampos.AutoScroll = true;

                // Es importante el orden en que se agregan al formulario
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

                                    // Se estira solo a los lados dentro de su panel
                                    txt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                                    EstilizarTextBox(txt);

                                    if (valoresExistentes.ContainsKey(etiqueta))
                                    {
                                        txt.Text = valoresExistentes[etiqueta];
                                    }

                                    // IMPORTANTE: Se agregan al Panel, no al Form
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
                    Text = observaciones
                };

                // Se estira hacia los lados
                txtObs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                EstilizarTextBox(txtObs);

                // Se agregan al Panel de scroll
                pnlCampos.Controls.Add(lblObs);
                pnlCampos.Controls.Add(txtObs);

                // --- 5. BOTONES EN EL PANEL FIJO ---
                Button btnAceptar = new Button() { Text = "Aceptar", Width = 90, Height = 40, DialogResult = DialogResult.OK };
                Button btnCancelar = new Button() { Text = "Cancelar", Width = 90, Height = 40, DialogResult = DialogResult.Cancel };

                // Los ubicamos en el panel inferior (Top = 15 da un pequeño margen desde arriba del panel)
                btnAceptar.Top = 15;
                btnCancelar.Top = 15;

                // Los alineamos a la derecha
                btnAceptar.Left = pnlBotones.ClientSize.Width - 210;
                btnCancelar.Left = pnlBotones.ClientSize.Width - 110;

                // Anclamos arriba y a la derecha (relativo al panel inferior)
                btnAceptar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                EstilizarBotonPrimario(btnAceptar);
                EstilizarBotonPeligro(btnCancelar);

                // Se agregan al panel de botones
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
