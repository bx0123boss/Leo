using System;
using System.Data.SqlClient;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmConsignaDevolver : frmBase
    {
        // Variables que recibimos del formulario padre
        private int _idCliente;
        private string _idProducto;
        private string _nombreProducto;
        private decimal _precioCongelado;
        private int _cantEnConsigna;

        // Constructor modificado para recibir toda la info del renglón seleccionado
        public frmConsignaDevolver(int idCliente, string idProducto, string nombreProducto, decimal precioCongelado, int cantEnConsigna)
        {
            InitializeComponent();

            _idCliente = idCliente;
            _idProducto = idProducto;
            _nombreProducto = nombreProducto;
            _precioCongelado = precioCongelado;
            _cantEnConsigna = cantEnConsigna;
        }

        private void frmConsignaDevolver_Load(object sender, EventArgs e)
        {
            // Mostramos los datos visuales al cajero
            lblProducto.Text = "Producto: " + _nombreProducto;
            lblPrecio.Text = "Precio Acordado: " + _precioCongelado.ToString("C2");
            lblEnConsigna.Text = "El cliente tiene en sus manos: " + _cantEnConsigna.ToString() + " piezas.";

            // Configuramos el selector numérico
            // El máximo que puede devolver es lo que tiene en consigna
            nudCantidad.Maximum = _cantEnConsigna;
            nudCantidad.Minimum = 1;
            EstilizarBotonPrimario(btnAceptar);
            EstilizarBotonAdvertencia(btnCancelar);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            int cantidadDevolver = (int)nudCantidad.Value;

            var confirmacion = MessageBox.Show($"¿Confirmas que el cliente regresa {cantidadDevolver} piezas a la tienda?",
                                               "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.No) return;

            using (SqlConnection con = new SqlConnection(Conexion.CadSQL)) // Tu cadena de Access
            {
                con.Open();

                // Iniciamos la transacción en OleDb
                using (SqlTransaction transaccion = con.BeginTransaction())
                {
                    try
                    {
                        // 1. BALANCE: Restar a EnConsigna y sumar a Devueltos
                        string queryBalance = @"
                    UPDATE ConsignaCliente 
                    SET EnConsigna = EnConsigna - @Cantidad, 
                        Devueltos = Devueltos + @CantidadDevuelta 
                    WHERE ClienteId = @IdCliente AND ProductoId = @IdProducto AND PrecioCongelado = @PrecioCongelado";

                        using (SqlCommand cmd = new SqlCommand(queryBalance, con, transaccion))
                        {
                            // EL ORDEN ES VITAL AQUÍ (Coincidiendo con los parámetros)
                            cmd.Parameters.AddWithValue("@Cantidad", cantidadDevolver); // 1er param (EnConsigna)
                            cmd.Parameters.AddWithValue("@CantidadDevuelta", cantidadDevolver); // 2do param (Devueltos)
                            cmd.Parameters.AddWithValue("@IdCliente", _idCliente);       // 3er param
                            cmd.Parameters.AddWithValue("@IdProducto", _idProducto);      // 4to param
                            cmd.Parameters.AddWithValue("@PrecioCongelado", _precioCongelado); // 5to param
                            cmd.ExecuteNonQuery();
                        }

                        // 2. KÁRDEX: Usamos GETDATE() en lugar de GETDATE()
                        string queryKardex = @"
                    INSERT INTO ConsignaMovimientos (ClienteId, ProductoId, TipoMovimiento, Cantidad, PrecioVigente, Fecha) 
                    VALUES (@IdCliente, @IdProducto, 'DEVOLUCION', @Cantidad, @PrecioCongelado, GETDATE())";

                        using (SqlCommand cmd = new SqlCommand(queryKardex, con, transaccion))
                        {
                            cmd.Parameters.AddWithValue("@IdCliente", _idCliente);
                            cmd.Parameters.AddWithValue("@IdProducto", _idProducto);
                            cmd.Parameters.AddWithValue("@Cantidad", cantidadDevolver);
                            cmd.Parameters.AddWithValue("@PrecioCongelado", _precioCongelado);
                            cmd.ExecuteNonQuery();
                        }

                        bool manejaInventario = true;
                        string queryManejo = "SELECT ManejaInventario FROM Inventario WHERE Id = @IdProducto";
                        using (SqlCommand cmdM = new SqlCommand(queryManejo, con, transaccion))
                        {
                            cmdM.Parameters.AddWithValue("@IdProducto", _idProducto);
                            object resM = cmdM.ExecuteScalar();
                            if (resM != null && resM != DBNull.Value) manejaInventario = Convert.ToBoolean(resM);
                        }

                        // 3. INVENTARIO REAL: Sumamos el stock
                        // (Verifica si tu columna de stock se llama 'Existencia' o 'Cantidad' en la tabla Inventario)
                        if (manejaInventario)
                        {
                            string queryInventario = @"
                        UPDATE Inventario 
                        SET Existencia = Existencia + @Cantidad 
                        WHERE Id = @IdProducto AND ManejaInventario = 1";

                            using (SqlCommand cmd = new SqlCommand(queryInventario, con, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@Cantidad", cantidadDevolver);
                                cmd.Parameters.AddWithValue("@IdProducto", _idProducto);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        if (manejaInventario)
                        {
                            int existenciaAntes = 0;
                            string queryExistencia = "SELECT Existencia FROM Inventario WHERE Id = @IdProducto";
                            using (SqlCommand cmdEx = new SqlCommand(queryExistencia, con, transaccion))
                            {
                                cmdEx.Parameters.AddWithValue("@IdProducto", _idProducto);
                                object res = cmdEx.ExecuteScalar();
                                if (res != null && res != DBNull.Value) existenciaAntes = Convert.ToInt32(res);
                            }

                            int existenciaDespues = existenciaAntes + cantidadDevolver; // Aquí sumamos porque regresó
                            string fechaKardex = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string descripcionKardex = "DEVOLUCIÓN DE CONSIGNA. CLIENTE ID: " + _idCliente.ToString();
                            string queryKardexGen = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) VALUES (@IdProducto, 'ENTRADA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
                            using (SqlCommand cmdK = new SqlCommand(queryKardexGen, con, transaccion))
                            {
                                cmdK.Parameters.AddWithValue("@IdProducto", _idProducto);
                                cmdK.Parameters.AddWithValue("@Descripcion", descripcionKardex);
                                cmdK.Parameters.AddWithValue("@ExistenciaAntes", existenciaAntes);
                                cmdK.Parameters.AddWithValue("@ExistenciaDespues", existenciaDespues);
                                cmdK.Parameters.AddWithValue("@Fecha", fechaKardex);
                                cmdK.ExecuteNonQuery();
                            }
                        }
                        transaccion.Commit();

                        MessageBox.Show("Devolución registrada correctamente y el producto regresó al inventario general.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        MessageBox.Show("Error al registrar la devolución: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close(); // Solo cierra el modal sin hacer nada
        }
    }
}