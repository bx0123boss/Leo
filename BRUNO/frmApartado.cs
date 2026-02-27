using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
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

            if (Conexion.lugar == "SANJUAN" && usuario == "Admin")
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

        // ---------- MÉTODO MAESTRO PARA EVITAR ERRORES DE CÁLCULO ----------
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
                    PrecioUnitario = Convert.ToDouble(dataGridView1[2, i].Value.ToString()),
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
                    if (Conexion.lugar == "TURBOLLANTAS")
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
                                 "", // Datos
                                 "APARTADO", // Observaciones / Tipo
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