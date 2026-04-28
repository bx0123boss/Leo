using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BRUNO
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
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            int cantidadDevolver = (int)nudCantidad.Value;

            var confirmacion = MessageBox.Show($"¿Confirmas que el cliente regresa {cantidadDevolver} piezas a la tienda?",
                                               "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.No) return;

            using (OleDbConnection con = new OleDbConnection(Conexion.CadCon)) // Tu cadena de Access
            {
                con.Open();

                // Iniciamos la transacción en OleDb
                using (OleDbTransaction transaccion = con.BeginTransaction())
                {
                    try
                    {
                        // 1. BALANCE: Restar a EnConsigna y sumar a Devueltos
                        string queryBalance = @"
                    UPDATE ConsignaCliente 
                    SET EnConsigna = EnConsigna - ?, 
                        Devueltos = Devueltos + ? 
                    WHERE ClienteId = ? AND ProductoId = ? AND PrecioCongelado = ?";

                        using (OleDbCommand cmd = new OleDbCommand(queryBalance, con, transaccion))
                        {
                            // EL ORDEN ES VITAL AQUÍ (Coincidiendo con los '?')
                            cmd.Parameters.AddWithValue("?", cantidadDevolver); // 1er ? (EnConsigna)
                            cmd.Parameters.AddWithValue("?", cantidadDevolver); // 2do ? (Devueltos)
                            cmd.Parameters.AddWithValue("?", _idCliente);       // 3er ?
                            cmd.Parameters.AddWithValue("?", _idProducto);      // 4to ?
                            cmd.Parameters.AddWithValue("?", _precioCongelado); // 5to ?
                            cmd.ExecuteNonQuery();
                        }

                        // 2. KÁRDEX: Usamos NOW() en lugar de GETDATE()
                        string queryKardex = @"
                    INSERT INTO ConsignaMovimientos (ClienteId, ProductoId, TipoMovimiento, Cantidad, PrecioVigente, Fecha) 
                    VALUES (?, ?, 'DEVOLUCION', ?, ?, NOW())";

                        using (OleDbCommand cmd = new OleDbCommand(queryKardex, con, transaccion))
                        {
                            cmd.Parameters.AddWithValue("?", _idCliente);
                            cmd.Parameters.AddWithValue("?", _idProducto);
                            cmd.Parameters.AddWithValue("?", cantidadDevolver);
                            cmd.Parameters.AddWithValue("?", _precioCongelado);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. INVENTARIO REAL: Sumamos el stock
                        // (Verifica si tu columna de stock se llama 'Existencia' o 'Cantidad' en la tabla Inventario)
                        string queryInventario = @"
                    UPDATE Inventario 
                    SET Existencia = Existencia + ? 
                    WHERE Id = ?";

                        using (OleDbCommand cmd = new OleDbCommand(queryInventario, con, transaccion))
                        {
                            cmd.Parameters.AddWithValue("?", cantidadDevolver);
                            cmd.Parameters.AddWithValue("?", _idProducto);
                            cmd.ExecuteNonQuery();
                        }
                        int existenciaAntes = 0;
                        string queryExistencia = "SELECT Existencia FROM Inventario WHERE Id = ?";
                        using (OleDbCommand cmdEx = new OleDbCommand(queryExistencia, con, transaccion))
                        {
                            cmdEx.Parameters.AddWithValue("?", _idProducto);
                            object res = cmdEx.ExecuteScalar();
                            if (res != null && res != DBNull.Value) existenciaAntes = Convert.ToInt32(res);
                        }

                        int existenciaDespues = existenciaAntes + cantidadDevolver; // Aquí sumamos porque regresó
                        string fechaKardex = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                        string descripcionKardex = "DEVOLUCIÓN DE CONSIGNA. CLIENTE ID: " + _idCliente.ToString();
                        string queryKardexGen = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) VALUES (?, 'ENTRADA', ?, ?, ?, ?)";
                        using (OleDbCommand cmdK = new OleDbCommand(queryKardexGen, con, transaccion))
                        {
                            cmdK.Parameters.AddWithValue("?", _idProducto);
                            cmdK.Parameters.AddWithValue("?", descripcionKardex);
                            cmdK.Parameters.AddWithValue("?", existenciaAntes);
                            cmdK.Parameters.AddWithValue("?", existenciaDespues);
                            cmdK.Parameters.AddWithValue("?", fechaKardex);
                            cmdK.ExecuteNonQuery();
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