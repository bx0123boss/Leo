using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace BRUNO
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
            using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
            {
                con.Open();
                using (OleDbTransaction transaccion = con.BeginTransaction())
                {
                    try
                    {
                        // A) RESTAR DEL INVENTARIO GENERAL 
                        // (Permitimos que quede en negativo, por eso no validamos stock previo)
                        string queryInventario = "UPDATE Inventario SET Existencia = Existencia - ? WHERE Id = ?";
                        using (OleDbCommand cmd = new OleDbCommand(queryInventario, con, transaccion))
                        {
                            cmd.Parameters.AddWithValue("?", cantidad);
                            cmd.Parameters.AddWithValue("?", _idProductoSeleccionado);
                            cmd.ExecuteNonQuery();
                        }

                        // B) ACTUALIZAR O INSERTAR EN EL BALANCE DE LA CONSIGNA (UPSERT Lógico)

                        // Primero vemos si ya existe este producto para este cliente a este mismo precio
                        int existeRegistro = 0;
                        string queryCheck = "SELECT COUNT(*) FROM ConsignaCliente WHERE ClienteId = ? AND ProductoId = ? AND PrecioCongelado = ?";
                        using (OleDbCommand cmdCheck = new OleDbCommand(queryCheck, con, transaccion))
                        {
                            cmdCheck.Parameters.AddWithValue("?", _idCliente);
                            cmdCheck.Parameters.AddWithValue("?", _idProductoSeleccionado);
                            cmdCheck.Parameters.AddWithValue("?", precioCongelado);
                            existeRegistro = Convert.ToInt32(cmdCheck.ExecuteScalar());
                        }

                        if (existeRegistro > 0)
                        {
                            string queryUpdate = "UPDATE ConsignaCliente SET EnConsigna = EnConsigna + ? WHERE ClienteId = ? AND ProductoId = ? AND PrecioCongelado = ?";
                            using (OleDbCommand cmdUpdate = new OleDbCommand(queryUpdate, con, transaccion))
                            {
                                cmdUpdate.Parameters.AddWithValue("?", cantidad);
                                cmdUpdate.Parameters.AddWithValue("?", _idCliente);
                                cmdUpdate.Parameters.AddWithValue("?", _idProductoSeleccionado);
                                cmdUpdate.Parameters.AddWithValue("?", precioCongelado);
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string queryInsert = "INSERT INTO ConsignaCliente (ClienteId, ProductoId, PrecioCongelado, EnConsigna, Vendidos, Devueltos) VALUES (?, ?, ?, ?, 0, 0)";
                            using (OleDbCommand cmdInsert = new OleDbCommand(queryInsert, con, transaccion))
                            {
                                cmdInsert.Parameters.AddWithValue("?", _idCliente);
                                cmdInsert.Parameters.AddWithValue("?", _idProductoSeleccionado);
                                cmdInsert.Parameters.AddWithValue("?", precioCongelado);
                                cmdInsert.Parameters.AddWithValue("?", cantidad);
                                cmdInsert.ExecuteNonQuery();
                            }
                        }

                        // C) INSERTAR EN EL KÁRDEX / HISTORIAL
                        string queryKardex = "INSERT INTO ConsignaMovimientos (ClienteId, ProductoId, TipoMovimiento, Cantidad, PrecioVigente, Fecha) VALUES (?, ?, 'ENTREGA', ?, ?, NOW())";
                        using (OleDbCommand cmdKardex = new OleDbCommand(queryKardex, con, transaccion))
                        {
                            cmdKardex.Parameters.AddWithValue("?", _idCliente);
                            cmdKardex.Parameters.AddWithValue("?", _idProductoSeleccionado);
                            cmdKardex.Parameters.AddWithValue("?", cantidad);
                            cmdKardex.Parameters.AddWithValue("?", precioCongelado);
                            cmdKardex.ExecuteNonQuery();
                        }
                        int existenciaAntes = 0;
                        string queryExistencia = "SELECT Existencia FROM Inventario WHERE Id = ?";
                        using (OleDbCommand cmdEx = new OleDbCommand(queryExistencia, con, transaccion))
                        {
                            cmdEx.Parameters.AddWithValue("?", _idProductoSeleccionado);
                            object res = cmdEx.ExecuteScalar();
                            if (res != null && res != DBNull.Value) existenciaAntes = Convert.ToInt32(res);
                        }

                        int existenciaDespues = existenciaAntes - cantidad;

                        string fechaKardex = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                        string descripcionKardex = "SALIDA A CONSIGNA. CLIENTE: " + _nombreCliente;
                        string queryKardexGen = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) VALUES (?, 'SALIDA', ?, ?, ?, ?)";
                        using (OleDbCommand cmdK = new OleDbCommand(queryKardexGen, con, transaccion))
                        {
                            cmdK.Parameters.AddWithValue("?", _idProductoSeleccionado);
                            cmdK.Parameters.AddWithValue("?", descripcionKardex);
                            cmdK.Parameters.AddWithValue("?", existenciaAntes);
                            cmdK.Parameters.AddWithValue("?", existenciaDespues);
                            cmdK.Parameters.AddWithValue("?", fechaKardex);
                            cmdK.ExecuteNonQuery();

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
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();

                    // 1. Buscamos si hay una coincidencia exacta con el ID / Código
                    OleDbCommand cmdCount = new OleDbCommand("SELECT COUNT(*) FROM Inventario WHERE Id='" + input + "';", con);
                    int valor = int.Parse(cmdCount.ExecuteScalar().ToString());

                    if (valor == 1)
                    {
                        // 2. Si existe exactamente ese código, traemos los datos
                        OleDbCommand cmdProd = new OleDbCommand("SELECT * FROM Inventario WHERE Id='" + input + "';", con);
                        using (OleDbDataReader reader = cmdProd.ExecuteReader())
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