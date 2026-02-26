using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmAgregarPoliza : frmBase
    {
        string idProv = "0", nombreProv = "";

        public frmAgregarPoliza()
        {
            InitializeComponent();
        }

        private void frmAgregarPoliza_Load(object sender, EventArgs e)
        {
            rdContado.Checked = true;

            // ESTILOS HEREDADOS DE frmBase
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPrimario(this.button2); // Botón Guardar
            EstilizarBotonPrimario(this.button1); // Botón Agregar
            EstilizarBotonAdvertencia(this.button4); // Buscar Producto
            EstilizarBotonAdvertencia(this.button5); // Buscar Proveedor

            // Ocultamos el botón calcular viejo ya que ahora todo es automático
            this.button3.Visible = false;

            EstilizarTextBox(txtFolio);
            EstilizarTextBox(txtID);
            EstilizarTextBox(txtCantidad);
            EstilizarTextBox(txtCosto);
            EstilizarTextBox(txtVenta);
            EstilizarTextBox(txtMenudeo);
            EstilizarTextBox(textBox1); // Costos Extra
        }

        // =========================================================
        // MOTOR DE CÁLCULO AUTOMÁTICO (Sin presionar botones)
        // =========================================================
        private void CalcularTotales()
        {
            double totalCostoGrid = 0;
            double totalIvaGrid = 0;

            double.TryParse(textBox1.Text, out double costosExtra);

            // Sumamos los totales 
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                double cant = Convert.ToDouble(dataGridView1[2, i].Value);
                double costo = Convert.ToDouble(dataGridView1[3, i].Value);
                double ivaU = Convert.ToDouble(dataGridView1[6, i].Value);

                totalCostoGrid += (costo * cant);
                totalIvaGrid += (ivaU * cant);
            }

            // Prorrateo de costos extra
            double porcentaje = totalCostoGrid > 0 ? (costosExtra / totalCostoGrid) : 0;

            // Repartir el costo extra
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                double costo = Convert.ToDouble(dataGridView1[3, i].Value);
                double iv = Convert.ToDouble(dataGridView1[6, i].Value);

                double costoE = costo * porcentaje;
                double costoR = costo + costoE + iv;

                dataGridView1[4, i].Value = Math.Round(costoE, 3);
                dataGridView1[5, i].Value = Math.Round(costoR, 2);
            }

            // Actualizar Interfaz
            lblTotal.Text = (totalCostoGrid + costosExtra).ToString("0.00", CultureInfo.InvariantCulture);
            lblIVA.Text = totalIvaGrid.ToString("0.00", CultureInfo.InvariantCulture);
            lblTotalSi.Text = (totalCostoGrid + costosExtra + totalIvaGrid).ToString("0.00", CultureInfo.InvariantCulture);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CalcularTotales();
        }

        // =========================================================
        // AGREGAR, BUSCAR Y ELIMINAR (UX)
        // =========================================================
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Busque y seleccione un producto primero.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtCantidad.Text, out double cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Introduzca una cantidad válida mayor a cero.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCantidad.Focus();
                return;
            }

            double.TryParse(txtCosto.Text, out double costo);
            double.TryParse(txtVenta.Text, out double iva);
            double.TryParse(txtMenudeo.Text, out double menudeo);

            dataGridView1.Rows.Add(txtID.Text, txtNombre.Text, cantidad, costo, 0, 0, iva, menudeo);

            txtID.Clear(); txtNombre.Clear(); txtCantidad.Text = "0";
            txtCosto.Clear(); txtVenta.Clear(); txtMenudeo.Clear();
            txtID.Focus();

            CalcularTotales();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            // FUNCIONALIDAD REAL DE BORRAR CON TECLA "SUPR" (DELETE)
            if (e.KeyCode == Keys.Delete && dataGridView1.CurrentRow != null)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                CalcularTotales();
            }
        }

        private void txtID_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text)) return;

            using (OleDbConnection conn = new OleDbConnection(Conexion.CadCon))
            {
                conn.Open();
                string query = "SELECT * FROM Inventario WHERE Id = @Id";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", txtID.Text);
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtNombre.Text = Convert.ToString(reader[1]);
                            txtMenudeo.Text = Convert.ToString(reader[2]);
                            txtCantidad.Focus();
                        }
                        else
                        {
                            txtNombre.Focus();
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (frmBuscarProductos buscar = new frmBuscarProductos())
            {
                buscar.poliza = -1;
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    txtID.Text = buscar.ID;
                    txtNombre.Text = buscar.producto;
                    txtMenudeo.Text = buscar.precio;
                    txtCantidad.Focus();
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (frmBuscarProveedor buscar = new frmBuscarProveedor())
            {
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    nombreProv = buscar.Nombre;
                    idProv = buscar.ID;
                    lblProveedor.Text = nombreProv;
                }
            }
        }

        private void txtCosto_Leave(object sender, EventArgs e)
        {
            if (double.TryParse(txtCosto.Text, out double costo))
            {
                txtVenta.Text = Math.Round(costo * 0.16, 2).ToString();
            }
            else
            {
                txtCosto.Text = "0";
                txtVenta.Text = "0";
            }
        }

        // Validación global para que solo acepten números y decimales
        private void ValidarNumeros(KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e) { ValidarNumeros(e); }
        private void txtCosto_KeyPress(object sender, KeyPressEventArgs e) { ValidarNumeros(e); }
        private void txtVenta_KeyPress(object sender, KeyPressEventArgs e) { ValidarNumeros(e); }
        private void txtMenudeo_KeyPress(object sender, KeyPressEventArgs e) { ValidarNumeros(e); }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) { ValidarNumeros(e); }

        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) SendKeys.Send("+{TAB}");
        }

        // =========================================================
        // GUARDADO EN BASE DE DATOS (CON TRANSACCIÓN SEGURA)
        // =========================================================
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFolio.Text))
            {
                MessageBox.Show("Ingrese un Folio para guardar la compra.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtFolio.Focus(); return;
            }
            if (idProv == "0")
            {
                MessageBox.Show("Debe seleccionar un proveedor.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dataGridView1.RowCount == 0)
            {
                MessageBox.Show("No hay productos en la póliza.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CalcularTotales(); // Aseguramos montos correctos

            using (OleDbConnection conn = new OleDbConnection(Conexion.CadCon))
            {
                conn.Open();
                using (OleDbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string fechaActual = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                        // 1. RECORRER PRODUCTOS DEL GRID
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            string idProd = dataGridView1[0, i].Value.ToString();
                            string nombreProd = dataGridView1[1, i].Value.ToString();
                            double cant = Convert.ToDouble(dataGridView1[2, i].Value);
                            double costo = Convert.ToDouble(dataGridView1[3, i].Value);
                            double costoExtra = Convert.ToDouble(dataGridView1[4, i].Value);
                            double costoReal = Convert.ToDouble(dataGridView1[5, i].Value);
                            double iva = Convert.ToDouble(dataGridView1[6, i].Value);
                            double precioVenta = Convert.ToDouble(dataGridView1[7, i].Value);

                            // Insertar detalle poliza
                            string queryProdPoliza = "INSERT INTO productosPoliza VALUES (@Id, @Nom, @Cant, @Costo, @CExtra, @CReal, @Iva, @Folio)";
                            using (OleDbCommand cmd = new OleDbCommand(queryProdPoliza, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", idProd);
                                cmd.Parameters.AddWithValue("@Nom", nombreProd);
                                cmd.Parameters.AddWithValue("@Cant", cant);
                                cmd.Parameters.AddWithValue("@Costo", costo);
                                cmd.Parameters.AddWithValue("@CExtra", costoExtra);
                                cmd.Parameters.AddWithValue("@CReal", costoReal);
                                cmd.Parameters.AddWithValue("@Iva", iva);
                                cmd.Parameters.AddWithValue("@Folio", txtFolio.Text);
                                cmd.ExecuteNonQuery();
                            }

                            // Comprobar existencia en inventario
                            bool productoExiste = false;
                            double invActual = 0;
                            double precioDB = 0;

                            string queryCheck = "SELECT Existencia, Especial FROM Inventario WHERE Id = @Id";
                            using (OleDbCommand cmdCheck = new OleDbCommand(queryCheck, conn, transaction))
                            {
                                cmdCheck.Parameters.AddWithValue("@Id", idProd);
                                using (OleDbDataReader reader = cmdCheck.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        productoExiste = true;
                                        invActual = Convert.ToDouble(reader[0]);
                                        precioDB = Convert.ToDouble(reader[1]);
                                    }
                                }
                            }

                            double nuevas = invActual + cant;

                            if (productoExiste)
                            {
                                // Actualizar Inventario (Costo Real actualiza Especial si es mayor)
                                double nuevoEspecial = costoReal > precioDB ? costoReal : precioDB;

                                string updInv = "UPDATE Inventario SET Especial = @Esp, Existencia = @Exis, PrecioVenta = @PVenta WHERE Id = @Id";
                                using (OleDbCommand cmdUpd = new OleDbCommand(updInv, conn, transaction))
                                {
                                    cmdUpd.Parameters.AddWithValue("@Esp", nuevoEspecial);
                                    cmdUpd.Parameters.AddWithValue("@Exis", nuevas);
                                    cmdUpd.Parameters.AddWithValue("@PVenta", precioVenta);
                                    cmdUpd.Parameters.AddWithValue("@Id", idProd);
                                    cmdUpd.ExecuteNonQuery();
                                }

                                // Kardex de entrada (Producto existente)
                                string insKardex1 = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) VALUES (@Id, 'ENTRADA', @Desc, @EA, @ED, @Fech, @Pre)";
                                using (OleDbCommand cmdKardex = new OleDbCommand(insKardex1, conn, transaction))
                                {
                                    cmdKardex.Parameters.AddWithValue("@Id", idProd);
                                    cmdKardex.Parameters.AddWithValue("@Desc", "COMPRA DE ARTICULO FOLIO: " + txtFolio.Text);
                                    cmdKardex.Parameters.AddWithValue("@EA", invActual);
                                    cmdKardex.Parameters.AddWithValue("@ED", nuevas);
                                    cmdKardex.Parameters.AddWithValue("@Fech", fechaActual);
                                    cmdKardex.Parameters.AddWithValue("@Pre", precioVenta);
                                    cmdKardex.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Insertar nuevo en inventario (Seguimos tu orden exacto de indices: Id, Nombre, IVA(6), Especial(5), Existencia(2), '0')
                                string insInv = "INSERT INTO Inventario VALUES (@Id, @Nom, @Iva, @CReal, @Exis, '0')";
                                using (OleDbCommand cmdIns = new OleDbCommand(insInv, conn, transaction))
                                {
                                    cmdIns.Parameters.AddWithValue("@Id", idProd);
                                    cmdIns.Parameters.AddWithValue("@Nom", nombreProd);
                                    cmdIns.Parameters.AddWithValue("@Iva", iva);
                                    cmdIns.Parameters.AddWithValue("@CReal", costoReal);
                                    cmdIns.Parameters.AddWithValue("@Exis", cant);
                                    cmdIns.ExecuteNonQuery();
                                }

                                // Kardex de entrada (Producto nuevo)
                                string insKardex2 = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, idProveedor, Proveedor) VALUES (@Id, 'ENTRADA', @Desc, 0, @ED, @Fech, @idProv, @NomProv)";
                                using (OleDbCommand cmdKardex = new OleDbCommand(insKardex2, conn, transaction))
                                {
                                    cmdKardex.Parameters.AddWithValue("@Id", idProd);
                                    cmdKardex.Parameters.AddWithValue("@Desc", "COMPRA DE ARTICULO FOLIO: " + txtFolio.Text);
                                    cmdKardex.Parameters.AddWithValue("@ED", cant);
                                    cmdKardex.Parameters.AddWithValue("@Fech", fechaActual);
                                    cmdKardex.Parameters.AddWithValue("@idProv", idProv);
                                    cmdKardex.Parameters.AddWithValue("@NomProv", nombreProv);
                                    cmdKardex.ExecuteNonQuery();
                                }
                            }
                        }

                        // 2. ACTUALIZAR PROVEEDOR (Si es a crédito)
                        if (rdCredito.Checked)
                        {
                            double adeudoActual = 0;
                            string qProv = "SELECT Adeudo FROM Proveedores WHERE Id = @IdP";
                            using (OleDbCommand cmdProv = new OleDbCommand(qProv, conn, transaction))
                            {
                                cmdProv.Parameters.AddWithValue("@IdP", Convert.ToInt32(idProv));
                                object result = cmdProv.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                    adeudoActual = Convert.ToDouble(result);
                            }

                            // Sumamos el total de esta póliza al adeudo (Costo Total + Extra)
                            double totalCompra = Convert.ToDouble(lblTotal.Text) + Convert.ToDouble(textBox1.Text);
                            adeudoActual += totalCompra;

                            string uProv = "UPDATE Proveedores SET Adeudo = @Adeudo WHERE Id = @IdP";
                            using (OleDbCommand cmdUProv = new OleDbCommand(uProv, conn, transaction))
                            {
                                cmdUProv.Parameters.AddWithValue("@Adeudo", adeudoActual);
                                cmdUProv.Parameters.AddWithValue("@IdP", Convert.ToInt32(idProv));
                                cmdUProv.ExecuteNonQuery();
                            }
                        }

                        // 3. INSERTAR PÓLIZA GENERAL
                        string insPoliza = "INSERT INTO Poliza(Folio, Fecha, FechaCaptura, CostoTotal, CostoExtra, IdProv, Proveedor, IVA, Total) VALUES (@Folio, @Fech1, @Fech2, @CostoT, @CostoE, @IdP, @Prov, @Iva, @Tot)";
                        using (OleDbCommand cmdPol = new OleDbCommand(insPoliza, conn, transaction))
                        {
                            cmdPol.Parameters.AddWithValue("@Folio", txtFolio.Text);
                            cmdPol.Parameters.AddWithValue("@Fech1", dateTimePicker1.Value.Date.ToString("MM/dd/yyyy 00:00:00"));
                            cmdPol.Parameters.AddWithValue("@Fech2", fechaActual);
                            cmdPol.Parameters.AddWithValue("@CostoT", lblTotal.Text);
                            cmdPol.Parameters.AddWithValue("@CostoE", textBox1.Text);
                            cmdPol.Parameters.AddWithValue("@IdP", idProv);
                            cmdPol.Parameters.AddWithValue("@Prov", nombreProv);
                            cmdPol.Parameters.AddWithValue("@Iva", lblIVA.Text);
                            cmdPol.Parameters.AddWithValue("@Tot", lblTotalSi.Text);
                            cmdPol.ExecuteNonQuery();
                        }

                        // SI TODO SALIÓ BIEN HASTA AQUÍ, SE CONFIRMAN LOS CAMBIOS
                        transaction.Commit();

                        MessageBox.Show("Póliza guardada y procesada correctamente", "COMPRA REGISTRADA", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        frmPolizas inv = new frmPolizas();
                        inv.Show();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        // SI HAY UN ERROR, SE DESHACEN TODOS LOS INSERT Y UPDATES
                        transaction.Rollback();
                        MessageBox.Show("Ocurrió un error al procesar la compra.\nLa base de datos no fue afectada.\n\nError: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        // Fin del botón guardar

        // Mantengo tu evento de TextChanged vacío original por si el diseñador visual lo requiere
        private void txtCosto_TextChanged(object sender, EventArgs e) { }
    }
}