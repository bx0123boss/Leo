using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace BRUNO
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

        public frmVentasCredito()
        {
            InitializeComponent();
            conectar.Open();
            AplicarEstilos();
        }

        private void AplicarEstilos()
        {
            cmbPago.SelectedIndex = 0;

            if (Conexion.lugar == "SANJUAN" && usuario == "Admin")
            {
                dataGridView1.Columns[2].ReadOnly = false;
            }
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (lblCliente.Text == "NO SELECCIONADO")
            {
                MessageBox.Show("Ingrese el cliente para poder hacer la venta", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                double adeudo = Convert.ToDouble(lblAdeudo.Text);
                double limite = Convert.ToDouble(lblLimite.Text);
                double totalVentaCalculo = Convert.ToDouble(lblFinal.Text);
                if (limite < (adeudo + totalVentaCalculo))
                {
                    MessageBox.Show("El limite sera excedido con la compra actual, favor de verificar", "ALTO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    double existencia = 0;
                    bool mamo = false;
                    string productosText = "En los siguientes productos se encuentra un error al vender las existencias: ";
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        existencia = Convert.ToDouble(dataGridView1[4, i].Value.ToString()) - Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                        if (existencia < 0)
                        {
                            mamo = true;
                            productosText = productosText + "\n" + dataGridView1[1, i].Value.ToString();
                        }
                    }
                    if (mamo)
                    {
                        MessageBox.Show(productosText + "\nVerifique sus almacenes", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        cmd = new OleDbCommand("select Numero from Folios where Folio='FolioCredito';", conectar);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            foli = Convert.ToInt32(Convert.ToString(reader[0].ToString()));
                        }

                        lblFolio.Text = "C" + String.Format("{0:0000}", foli);

                        List<Producto> productos = new List<Producto>();
                        existencia = 0;
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            string unidad = "0";
                            //Obtenemos existencias del articulo
                            cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[5, i].Value.ToString() + "';", conectar);
                            reader = cmd.ExecuteReader();
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
                                unidad = "";

                            //Obtenemos existencias del articulo
                            existencia = exis - Convert.ToDouble(dataGridView1[0, i].Value.ToString());

                            //Actualizamos existencias
                            cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existencia + "' Where Id='" + dataGridView1[5, i].Value.ToString() + "';", conectar);
                            cmd.ExecuteNonQuery();

                            //Insertamos en la venta a credito
                            cmd = new OleDbCommand("insert into VentasCredito(FolioVenta,IdProducto,Cantidad,Producto,MontoTotal,IdCliente,Fecha) values('" + lblFolio.Text + "','" + dataGridView1[5, i].Value.ToString() + "','" + dataGridView1[0, i].Value.ToString() + "','" + dataGridView1[1, i].Value.ToString() + "','" + dataGridView1[3, i].Value.ToString() + "','" + idCliente + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                            cmd.ExecuteNonQuery();

                            cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[5, i].Value.ToString() + "','SALIDA','VENTA DE ARTICULO FOLIO: " + lblFolio.Text + "'," + exis + ",'" + existencia + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                            cmd.ExecuteNonQuery();

                            productos.Add(new Producto
                            {
                                Nombre = dataGridView1[1, i].Value.ToString(),
                                Cantidad = Convert.ToDouble(dataGridView1[0, i].Value.ToString()),
                                PrecioUnitario = Convert.ToDouble(dataGridView1[2, i].Value.ToString()),
                                Total = Convert.ToDouble(dataGridView1[3, i].Value.ToString()),
                            });
                        }

                        total = Convert.ToDouble(lblFinal.Text);
                        adeudo = Convert.ToDouble(lblFinal.Text) + Convert.ToDouble(lblAdeudo.Text);
                        double saldo = total - Convert.ToDouble(txtAbono.Text);
                        if(saldo>0)
                        {
                            cmd = new OleDbCommand("insert into Abonos(Abono,idCliente,Fecha,Nombre,Folio,Estatus) Values('" + txtAbono.Text + "','" + idCliente + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + lblCliente.Text + "','" + lblFolio.Text + "','PAGADO');", conectar);
                            cmd.ExecuteNonQuery();
                        }
                        
                        cmd = new OleDbCommand("insert into Ventas2(Monto,Fecha,Folio,IdCliente,Estuatus, Saldo) values('" + lblTotal.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + lblFolio.Text + "','" + idCliente + "','COBRADO','" + total + "');", conectar);
                        cmd.ExecuteNonQuery();
                        cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Abono de la venta a credito folio " + lblFolio.Text + "','" + txtAbono.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + cmbPago.Text + "');", conectar);
                        cmd.ExecuteNonQuery();
                        cmd = new OleDbCommand("UPDATE Clientes set Adeudo=" + adeudo + " Where Id=" + idCliente + ";", conectar);
                        cmd.ExecuteNonQuery();

                        foli = foli + 1;
                        cmd = new OleDbCommand("UPDATE Folios set Numero=" + foli + " where Folio='FolioCredito';", conectar);
                        cmd.ExecuteNonQuery();

                        // -- ÁREA DE IMPRESIÓN --
                        Dictionary<string, double> totales = new Dictionary<string, double>();
                        double totalVentaGrid = Convert.ToDouble(lblTotal.Text);
                        double subTotalImpresion = totalVentaGrid - descuento;

                        if (descuento != 0)
                        {
                            totales.Add("Descuento", descuento);
                        }
                        if (Conexion.ConIva)
                        {
                            totales.Add("Subtotal", subTotalImpresion / 1.16);
                            totales.Add("IVA", (subTotalImpresion / 1.16) * 0.16);
                        }
                        totales.Add("Total", subTotalImpresion);
                        totales.Add("Abono", Convert.ToDouble(txtAbono.Text));
                        totales.Add("Restante", Convert.ToDouble(lblFinal.Text));

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
                                         subTotalImpresion,
                                         lblCliente.Text,
                                         idCliente,
                                         cmbPago.Text,
                                         "", // datos extra (en blanco para ventas a crédito como default)
                                         "", // observaciones
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
                            TicketPrinter ticketPrinter = new TicketPrinter(Conexion.datosTicket, Conexion.pieDeTicket, Conexion.logoPath, productos, lblFolio.Text, "", "", subTotalImpresion, false, totales, cmbPago.Text);
                            ticketPrinter.ImprimirTicket();
                        }
                        // -- FIN ÁREA DE IMPRESIÓN --

                        MessageBox.Show("Venta realizada con exito", "VENTA REALIZADA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        frmVentasCredito credito = new frmVentasCredito();
                        credito.Show();
                        this.Close();

                    }
                }
            }
        }

        private void frmVentasCredito_Load(object sender, EventArgs e)
        {
            cmbPago.SelectedIndex = 0;

            if (Conexion.lugar == "SANJUAN" && usuario == "Admin")
            {
                dataGridView1.Columns[2].ReadOnly = false;
            }
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
            if (dataGridView1.Rows[e.RowIndex].Cells[0].Value != null &&
                dataGridView1.Rows[e.RowIndex].Cells[2].Value != null)
            {
                double cantidad = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                double precio = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                double monto = cantidad * precio;

                dataGridView1.Rows[e.RowIndex].Cells[3].Value = monto;
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
    }
}