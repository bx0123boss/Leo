using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmConsignaEntregar : frmBase
    {
        private int _idCliente;
        private string _nombreCliente;

        // Variable para guardar en memoria qué producto encontró el buscador
        private string _idProductoSeleccionado = "";

        public frmConsignaEntregar(int idCliente, string nombreCliente)
        {
            InitializeComponent();
            _idCliente = idCliente;
            _nombreCliente = nombreCliente;
        }

        private void frmConsignaEntregar_Load(object sender, EventArgs e)
        {
            EstilizarBotonPeligro(btnCancelar);
            EstilizarBotonAdvertencia(btnBuscar);
            EstilizarBotonPrimario(btnGuardar);
            EstilizarTextBox(txtCodigo);
            EstilizarBotonPrimario(btnGuardar);
            lblCliente.Text = "Cliente: " + _nombreCliente;
            txtCodigo.Focus();
        }

        // =========================================================================
        // 2. GUARDAR (LA TRANSACCIÓN)
        // =========================================================================

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrEmpty(_idProductoSeleccionado))
            {
                MessageBox.Show("Debe buscar y seleccionar un producto primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precioCongelado) || precioCongelado < 0)
            {
                MessageBox.Show("Por favor ingrese un precio válido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int cantidad = (int)nudCantidad.Value;

            var confirmacion = MessageBox.Show($"¿Desea entregar {cantidad} piezas de este producto al cliente?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.No) return;

            // Inicia la transacción OleDb
            using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
            {
                con.Open();
                using (SqlTransaction transaccion = con.BeginTransaction())
                {
                    try
                    {
                        bool manejaInventario = true;
                        string queryManejo = "SELECT ManejaInventario FROM Inventario WHERE Id = @IdProducto";
                        using (SqlCommand cmdM = new SqlCommand(queryManejo, con, transaccion))
                        {
                            cmdM.Parameters.AddWithValue("@IdProducto", _idProductoSeleccionado);
                            object resM = cmdM.ExecuteScalar();
                            if (resM != null && resM != DBNull.Value) manejaInventario = Convert.ToBoolean(resM);
                        }

                        // A) RESTAR DEL INVENTARIO GENERAL 
                        // (Permitimos que quede en negativo, por eso no validamos stock previo)
                        if (manejaInventario)
                        {
                            string queryInventario = "UPDATE Inventario SET Existencia = Existencia - @Cantidad WHERE Id = @IdProducto AND ManejaInventario = 1";
                            using (SqlCommand cmd = new SqlCommand(queryInventario, con, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                                cmd.Parameters.AddWithValue("@IdProducto", _idProductoSeleccionado);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // B) ACTUALIZAR O INSERTAR EN EL BALANCE DE LA CONSIGNA (UPSERT Lógico)

                        // Primero vemos si ya existe este producto para este cliente a este mismo precio
                        int existeRegistro = 0;
                        string queryCheck = "SELECT COUNT(*) FROM ConsignaCliente WHERE ClienteId = @IdCliente AND ProductoId = @IdProducto AND PrecioCongelado = @PrecioCongelado";
                        using (SqlCommand cmdCheck = new SqlCommand(queryCheck, con, transaccion))
                        {
                            cmdCheck.Parameters.AddWithValue("@IdCliente", _idCliente);
                            cmdCheck.Parameters.AddWithValue("@IdProducto", _idProductoSeleccionado);
                            cmdCheck.Parameters.AddWithValue("@PrecioCongelado", precioCongelado);
                            existeRegistro = Convert.ToInt32(cmdCheck.ExecuteScalar());
                        }

                        if (existeRegistro > 0)
                        {
                            string queryUpdate = "UPDATE ConsignaCliente SET EnConsigna = EnConsigna + @Cantidad WHERE ClienteId = @IdCliente AND ProductoId = @IdProducto AND PrecioCongelado = @PrecioCongelado";
                            using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, con, transaccion))
                            {
                                cmdUpdate.Parameters.AddWithValue("@Cantidad", cantidad);
                                cmdUpdate.Parameters.AddWithValue("@IdCliente", _idCliente);
                                cmdUpdate.Parameters.AddWithValue("@IdProducto", _idProductoSeleccionado);
                                cmdUpdate.Parameters.AddWithValue("@PrecioCongelado", precioCongelado);
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string queryInsert = "INSERT INTO ConsignaCliente (ClienteId, ProductoId, PrecioCongelado, EnConsigna, Vendidos, Devueltos) VALUES (@IdCliente, @IdProducto, @PrecioCongelado, @Cantidad, 0, 0)";
                            using (SqlCommand cmdInsert = new SqlCommand(queryInsert, con, transaccion))
                            {
                                cmdInsert.Parameters.AddWithValue("@IdCliente", _idCliente);
                                cmdInsert.Parameters.AddWithValue("@IdProducto", _idProductoSeleccionado);
                                cmdInsert.Parameters.AddWithValue("@PrecioCongelado", precioCongelado);
                                cmdInsert.Parameters.AddWithValue("@Cantidad", cantidad);
                                cmdInsert.ExecuteNonQuery();
                            }
                        }

                        // C) INSERTAR EN EL KÁRDEX / HISTORIAL
                        string queryKardex = "INSERT INTO ConsignaMovimientos (ClienteId, ProductoId, TipoMovimiento, Cantidad, PrecioVigente, Fecha) VALUES (@IdCliente, @IdProducto, 'ENTREGA', @Cantidad, @PrecioCongelado, GETDATE())";
                        using (SqlCommand cmdKardex = new SqlCommand(queryKardex, con, transaccion))
                        {
                            cmdKardex.Parameters.AddWithValue("@IdCliente", _idCliente);
                            cmdKardex.Parameters.AddWithValue("@IdProducto", _idProductoSeleccionado);
                            cmdKardex.Parameters.AddWithValue("@Cantidad", cantidad);
                            cmdKardex.Parameters.AddWithValue("@PrecioCongelado", precioCongelado);
                            cmdKardex.ExecuteNonQuery();
                        }
                        if (manejaInventario)
                        {
                            int existenciaAntes = 0;
                            string queryExistencia = "SELECT Existencia FROM Inventario WHERE Id = @IdProducto";
                            using (SqlCommand cmdEx = new SqlCommand(queryExistencia, con, transaccion))
                            {
                                cmdEx.Parameters.AddWithValue("@IdProducto", _idProductoSeleccionado);
                                object res = cmdEx.ExecuteScalar();
                                if (res != null && res != DBNull.Value) existenciaAntes = Convert.ToInt32(res);
                            }

                            int existenciaDespues = existenciaAntes - cantidad;

                            string fechaKardex = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                            string descripcionKardex = "SALIDA A CONSIGNA. CLIENTE: " + _nombreCliente;
                            string queryKardexGen = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) VALUES (@IdProducto, 'SALIDA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
                            using (SqlCommand cmdK = new SqlCommand(queryKardexGen, con, transaccion))
                            {
                                cmdK.Parameters.AddWithValue("@IdProducto", _idProductoSeleccionado);
                                cmdK.Parameters.AddWithValue("@Descripcion", descripcionKardex);
                                cmdK.Parameters.AddWithValue("@ExistenciaAntes", existenciaAntes);
                                cmdK.Parameters.AddWithValue("@ExistenciaDespues", existenciaDespues);
                                cmdK.Parameters.AddWithValue("@Fecha", fechaKardex);
                                cmdK.ExecuteNonQuery();

                            }
                        }
                            transaccion.Commit();

                        MessageBox.Show("Mercancía entregada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK; // Le avisa a frmConsigna que sí hubo cambios
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        MessageBox.Show("Error al registrar la entrega: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(txtCodigo.Text)) return;

                string input = txtCodigo.Text.Trim();
                BuscarProductoDinamico(input);

                e.Handled = true;
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text)) return;

            string input = txtCodigo.Text.Trim();
            BuscarProductoDinamico(input);
        }

        private void BuscarProductoDinamico(string input)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
                {
                    con.Open();

                    // 1. Buscamos si hay una coincidencia exacta con el ID / Código
                    SqlCommand cmdCount = new SqlCommand("SELECT COUNT(*) FROM Inventario WHERE Id='" + input + "';", con);
                    int valor = int.Parse(cmdCount.ExecuteScalar().ToString());

                    if (valor == 1)
                    {
                        // 2. Si existe exactamente ese código, traemos los datos
                        SqlCommand cmdProd = new SqlCommand("SELECT * FROM Inventario WHERE Id='" + input + "';", con);
                        using (SqlDataReader reader = cmdProd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 3. Abrimos el formulario de Precio (Igual que en Ventas)
                                using (frmPrecio buscar = new frmPrecio())
                                {
                                    if (buscar.ShowDialog() == DialogResult.OK)
                                    {
                                        double preci = 0;
                                        // Si tipo es GEN usa índice 3, si no usa índice 2 (Según tu lógica)
                                        preci = (buscar.tipo == "GEN") ? Convert.ToDouble(reader[3]) : Convert.ToDouble(reader[2]);

                                        // Guardamos los datos en la pantalla de Consigna para que el usuario confirme cantidad
                                        _idProductoSeleccionado = reader[0].ToString();
                                        lblNombreProducto.Text = reader[1].ToString();
                                        txtPrecio.Text = String.Format("{0:0.00}", preci);

                                        nudCantidad.Focus(); // Mandamos el cursor directo a la cantidad
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // 4. Si no es exacto o no lo encontró directo, abrimos el buscador general
                        using (frmBuscarProductos buscar = new frmBuscarProductos())
                        {
                            buscar.textBox1.Text = input; // Le pasamos lo que escribió

                            if (buscar.ShowDialog() == DialogResult.OK)
                            {
                                // Cuando seleccionan algo del buscador y le dan OK
                                double precioUnitario = Convert.ToDouble(buscar.precio);

                                _idProductoSeleccionado = buscar.ID.ToString();
                                lblNombreProducto.Text = buscar.producto;
                                txtPrecio.Text = String.Format("{0:0.00}", precioUnitario);

                                nudCantidad.Focus();
                            }
                        }
                    }
                }

                txtCodigo.Text = ""; // Limpiamos la caja de texto después de buscar
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LimpiarProducto();
            }
        }

        private void LimpiarProducto()
        {
            _idProductoSeleccionado = "";
            lblNombreProducto.Text = "...";
            txtPrecio.Text = "";
            txtCodigo.Focus();
            txtCodigo.SelectAll();
        }
    }
}