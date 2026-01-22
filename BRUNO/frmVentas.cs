using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
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
        int foli;
        string direccion = "";
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
            cmbPago.SelectedIndex = 0;
            if (Conexion.lugar == "DEPORTES LEO")
            {
                dataGridView1.Columns[2].ReadOnly = false;
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
                            using (frmPrecio buscar = new frmPrecio())
                            {
                                        double preci = Convert.ToDouble(reader[3].ToString());
                                        dataGridView1.Rows.Add("1", Convert.ToString(reader[1].ToString()), String.Format("{0:0.00}", preci), String.Format("{0:0.00}", preci), Convert.ToString(reader[4].ToString()), Convert.ToString(reader[0].ToString()), origen, Convert.ToString(reader[8].ToString()), Convert.ToString(reader[7].ToString()), "","X");
                                  
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
                                dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen, buscar.IVA, buscar.compra, "","X");

                            }
                        }

                        lblTotal.Text = $"{RecalcularTotal:C}";
                        textBox1.Text = "";
                        //}
                        //}
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
                // Eliminar filas del origen de datos (depende de tu implementación)
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    // Aquí debes implementar la lógica para eliminar del origen de datos
                    // Por ejemplo, si está vinculado a un DataTable:
                    if (row.DataBoundItem != null)
                    {
                        // Lógica para eliminar del origen de datos
                        // Ejemplo para DataTable:
                        ((DataRowView)row.DataBoundItem).Row.Delete();
                    }

                    // Eliminar del DataGridView
                    dataGridView1.Rows.Remove(row);
                }
                lblTotal.Text = $"{RecalcularTotal:C}";
                textBox1.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar filas: {ex.Message}");
            }
            //dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
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

        static double CalcularImpuesto(double baseValue, double percentage)
        {
            return baseValue * (percentage / 100.0);
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
            using (frmPago ori = new frmPago())
            {

                ori.total = (total - descuento);
                ori.txtTotal.Text = $"{ori.total:C}";
                if (ori.ShowDialog() == DialogResult.OK)
                {
                    efectivo = ori.efectivo;
                    cambio = ori.cambio;
                }
                else
                    return;
            }
            cmd = new OleDbCommand("select Numero from Folios where Folio='FolioContado';", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                foli = Convert.ToInt32(Convert.ToString(reader[0].ToString()));
            }
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
                //Obtenemos existencias del articulo
                cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[5, i].Value.ToString() + "';", conectar);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    exis = Convert.ToDouble(Convert.ToString(reader[4].ToString()));
                    unidad = Convert.ToString(reader[9].ToString());
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
                //MessageBox.Show("Utilidad" + nuevaUtilidad);
                cmd = new OleDbCommand("insert into VentasContado(FolioVenta,IdProducto,Cantidad,Producto,MontoTotal,idCliente,Fecha,Utilidad) values('" + lblFolio.Text + "','" + dataGridView1[5, i].Value.ToString() + "','" + dataGridView1[0, i].Value.ToString() + "','" + dataGridView1[1, i].Value.ToString() + "','" + dataGridView1[3, i].Value.ToString() + "','" + (string.IsNullOrEmpty(idCliente) ? "0" : idCliente) + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + nuevaUtilidad + "');", conectar);
                cmd.ExecuteNonQuery();
                string precio = "" + Math.Round(Convert.ToDouble(dataGridView1[3, i].Value.ToString()), 2);
                if (dataGridView1[7, i].Value.ToString() == "IVA(16)")
                {
                    IVA += Convert.ToDouble(precio) - (Convert.ToDouble(precio) / 1.16);
                }

                //agregar item al ticket (productos)
                //ticket.AddItem(dataGridView1[0, i].Value.ToString() + " " + unidad, "      " + dataGridView1[1, i].Value.ToString(), "   $" + dataGridView1[3, i].Value.ToString());
                //MessageBox.Show("Se vendera el numero:"+i+"\nCantidad: "+dataGridView1[0, i].Value.ToString()+"\nProducto: "+ dataGridView1[1, i].Value.ToString()+"\nPrecio: "+dataGridView1[2, i].Value.ToString()+"\nMonto :" + dataGridView1[3, i].Value.ToString() +"\nExistencias :"+ dataGridView1[4, i].Value.ToString()+"\nID :"+dataGridView1[5, i].Value.ToString());

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
                total=total - descuento;
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

            if (Conexion.impresionMediaCarta)
            {
                try
                {
                    TicketMediaCarta pdfTicket = new TicketMediaCarta(
                         productos,
                         lblFolio.Text,
                         total,
                         lblCliente.Text,
                         cmbPago.Text,
                         Conexion.lugar,
                         Conexion.logoPath,    // <--- Logo
                         Conexion.datosTicket, // <--- Encabezado del negocio
                         Conexion.pieDeTicket  // <--- Pie de página
                     );

                    pdfTicket.ImprimirDirectamente(Conexion.impresora);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al imprimir PDF Media Carta: " + ex.Message);
                }
            }
            else
            {
                TicketPrinter ticketPrinter = new TicketPrinter(Conexion.datosTicket, Conexion.pieDeTicket, Conexion.logoPath, productos, lblFolio.Text, "", "", total, false, totales, cmbPago.Text);
                ticketPrinter.ImprimirTicket();
            }
           
            cmd = new OleDbCommand("insert into Ventas(Monto,Fecha,Folio,Estatus, Descuento, Pago) values('" + (total - descuento) + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + lblFolio.Text + "','COBRADO','" + descuento + "','" + cmbPago.Text + "');", conectar);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Venta a contado folio " + lblFolio.Text + "','" + (total - descuento) + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + cmbPago.Text + "');", conectar);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("insert into VentasCajero(IdUsuario,Usuario,FolioVenta,Total,Fecha,Cajero) values('" + idUsuario + "','" + lblUsuario.Text + "','" + lblFolio.Text + "','" + (total - descuento) + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + lblCajero.Text + "');", conectar);
            cmd.ExecuteNonQuery();

            foli = foli + 1;
            cmd = new OleDbCommand("UPDATE Folios set Numero=" + foli + " where Folio='FolioContado';", conectar);
            cmd.ExecuteNonQuery();
           
            ReiniciarForm();
            MessageBox.Show(this,"Venta realizada con exito", "VENTA REALIZADA", MessageBoxButtons.OK, MessageBoxIcon.Information);

          


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
	                    DC.Importe
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
        }

        private void AgregarProductoAVenta(string codigoProd, double cantidad, string descripcion, double precioUni, double tolProducto)
        {
            cmd = new OleDbCommand("select * from Inventario where Id='" + codigoProd + "';", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                using (frmPrecio buscar = new frmPrecio())
                {
                    dataGridView1.Rows.Add(cantidad, descripcion, String.Format("{0:0.00}", precioUni), String.Format("{0:0.00}", tolProducto), Convert.ToString(reader[4].ToString()), codigoProd, origen, Convert.ToString(reader[8].ToString()), Convert.ToString(reader[7].ToString()), "", "X");
                }

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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == dataGridView1.Columns["btnEliminar"].Index && e.RowIndex >= 0)
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                lblTotal.Text = $"{RecalcularTotal:C}";
            }
        }

    }
}
