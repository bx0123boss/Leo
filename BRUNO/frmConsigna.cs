using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;
// using System.Data.OleDb; // o SqlClient, dependiendo de dónde crees las 2 tablas nuevas.

namespace BRUNO
{
    public partial class frmConsigna : frmBase
    {
        private int _idCliente;
        private string _nombreCliente;

        public frmConsigna(int idCliente, string nombreCliente)
        {
            InitializeComponent();
            _idCliente = idCliente;
            _nombreCliente = nombreCliente;
        }

        private void frmConsigna_Load(object sender, EventArgs e)
        {
            lblCliente.Text = "Cliente: " + _nombreCliente;
            CargarBalance();
            CargarKardex();
            EstilizarDataGridView(dgvBalance);
            EstilizarDataGridView(dgvKardex);
            EstilizarBotonPeligro(btnDeshacer);
            btnEntregar.Click += new EventHandler(btnEntregar_Click);
            btnDevolver.Click += new EventHandler(btnDevolver_Click);
            btnCobrar.Click += new EventHandler(btnCobrar_Click);
        }

        #region MÉTODOS DE DATOS
        private void CargarBalance()
        {
            try
            {
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                        c.ProductoId, 
                        i.Nombre AS Producto, 
                        c.PrecioCongelado AS Precio, 
                        c.EnConsigna AS [En Consigna], 
                        c.Vendidos, 
                        c.Devueltos 
                    FROM
                        ConsignaCliente c
                        INNER JOIN Inventario i ON c.ProductoId = i.Id
                    WHERE c.ClienteId = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        // En Access/OleDb, los parámetros van con '?' y se asignan en orden
                        cmd.Parameters.AddWithValue("?", _idCliente);

                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvBalance.DataSource = dt;

                        // Configuramos las columnas visualmente
                        if (dgvBalance.Columns.Contains("ProductoId"))
                        {
                            dgvBalance.Columns["ProductoId"].Visible = false; // Ocultamos el ID
                        }

                        // Formateamos como moneda
                        if (dgvBalance.Columns.Contains("Precio"))
                        {
                            dgvBalance.Columns["Precio"].DefaultCellStyle.Format = "C2";
                        }

                        // Bloquear grid
                        foreach (DataGridViewColumn col in dgvBalance.Columns)
                        {
                            col.ReadOnly = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el balance de la consigna: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CargarKardex()
        {
            try
            {
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            m.Id,
                            m.ProductoId,
                            m.Fecha, 
                            m.TipoMovimiento AS Movimiento, 
                            i.Nombre AS Producto, 
                            m.Cantidad, 
                            m.PrecioVigente AS Precio,
                            m.ReferenciaVentaId AS FolioTicket 
                        FROM ConsignaMovimientos m
                        INNER JOIN Inventario i ON m.ProductoId = i.Id
                        WHERE m.ClienteId = ? 
                        ORDER BY m.Fecha DESC";

                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("?", _idCliente);

                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvKardex.DataSource = dt;

                        // Ocultamos las columnas técnicas
                        if (dgvKardex.Columns.Contains("Id")) dgvKardex.Columns["Id"].Visible = false;
                        if (dgvKardex.Columns.Contains("ProductoId")) dgvKardex.Columns["ProductoId"].Visible = false;

                        if (dgvKardex.Columns.Contains("Precio"))
                            dgvKardex.Columns["Precio"].DefaultCellStyle.Format = "C2";

                        if (dgvKardex.Columns.Contains("Fecha"))
                            dgvKardex.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                        foreach (DataGridViewColumn col in dgvKardex.Columns)
                            col.ReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el historial de movimientos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region EVENTOS DE LOS BOTONES
        private void btnEntregar_Click_1(object sender, EventArgs e)
        {
            // Abrimos el modal pasándole los datos del cliente actual
            frmConsignaEntregar frm = new frmConsignaEntregar(_idCliente, _nombreCliente);

            // Si el modal devuelve "OK" (significa que sí se guardó algo y no solo se canceló)
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Recargamos las tablas automáticamente
                CargarBalance();
                CargarKardex();
            }
        }

        private void btnDeshacer_Click(object sender, EventArgs e)
        {
            if (dgvKardex.CurrentRow == null)
            {
                MessageBox.Show("Por favor, seleccione un movimiento del historial para deshacer.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string tipoMov = dgvKardex.CurrentRow.Cells["Movimiento"].Value.ToString();

            // Bloqueamos las ventas y los que ya están cancelados
            if (tipoMov == "VENTA")
            {
                MessageBox.Show("Las VENTAS no se pueden cancelar desde aquí. Debes ir al historial de ventas y cancelarla desde el ticket (eso devolverá automáticamente el producto a la consigna).", "Acción no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tipoMov == "CANCELADA" || tipoMov == "CANCELACION")
            {
                MessageBox.Show("Este movimiento ya fue cancelado previamente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idMovimiento = Convert.ToInt32(dgvKardex.CurrentRow.Cells["Id"].Value);
            string idProducto = dgvKardex.CurrentRow.Cells["ProductoId"].Value.ToString();
            string nombreProducto = dgvKardex.CurrentRow.Cells["Producto"].Value.ToString();
            int cantidad = Convert.ToInt32(dgvKardex.CurrentRow.Cells["Cantidad"].Value);
            decimal precio = Convert.ToDecimal(dgvKardex.CurrentRow.Cells["Precio"].Value);

            DialogResult resp = MessageBox.Show($"¿Estás seguro de deshacer este movimiento de {tipoMov} por {cantidad} piezas de {nombreProducto}?", "Confirmar Cancelación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.No) return;

            using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
            {
                con.Open();
                using (OleDbTransaction transaccion = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Obtener los saldos actuales del cliente para este producto
                        int enConsignaActual = 0;
                        using (OleDbCommand cmdSaldo = new OleDbCommand("SELECT EnConsigna FROM ConsignaCliente WHERE ClienteId = ? AND ProductoId = ? AND PrecioCongelado = ?", con, transaccion))
                        {
                            cmdSaldo.Parameters.AddWithValue("?", _idCliente);
                            cmdSaldo.Parameters.AddWithValue("?", idProducto);
                            cmdSaldo.Parameters.AddWithValue("?", precio);
                            using (OleDbDataReader reader = cmdSaldo.ExecuteReader())
                            {
                                if (reader.Read()) enConsignaActual = Convert.ToInt32(reader["EnConsigna"]);
                            }
                        }

                        // 2. Ejecutar lógica inversa según el tipo
                        if (tipoMov == "ENTREGA")
                        {
                            // Si deshacemos una entrega, le quitamos de 'EnConsigna'
                            if (enConsignaActual < cantidad)
                            {
                                MessageBox.Show("No puedes deshacer esta entrega completa porque el cliente ya devolvió o vendió una parte de estas piezas. El saldo actual es menor a lo entregado.", "Error de Lógica", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // A) Restar a ConsignaCliente
                            using (OleDbCommand cmd = new OleDbCommand("UPDATE ConsignaCliente SET EnConsigna = EnConsigna - ? WHERE ClienteId = ? AND ProductoId = ? AND PrecioCongelado = ?", con, transaccion))
                            {
                                cmd.Parameters.AddWithValue("?", cantidad);
                                cmd.Parameters.AddWithValue("?", _idCliente);
                                cmd.Parameters.AddWithValue("?", idProducto);
                                cmd.Parameters.AddWithValue("?", precio);
                                cmd.ExecuteNonQuery();
                            }

                            // B) Sumar de regreso al Inventario (Y Kardex)
                            double exis = 0;
                            using (OleDbCommand cmdEx = new OleDbCommand("SELECT Existencia FROM Inventario WHERE Id = ?", con, transaccion))
                            {
                                cmdEx.Parameters.AddWithValue("?", idProducto);
                                object res = cmdEx.ExecuteScalar();
                                if (res != null && res != DBNull.Value) exis = Convert.ToDouble(res);
                            }
                            double nuevaExis = exis + cantidad;

                            using (OleDbCommand cmd = new OleDbCommand("UPDATE Inventario SET Existencia = ? WHERE Id = ?", con, transaccion))
                            {
                                cmd.Parameters.AddWithValue("?", nuevaExis);
                                cmd.Parameters.AddWithValue("?", idProducto);
                                cmd.ExecuteNonQuery();
                            }

                            string fechaKardex = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                            using (OleDbCommand cmdK = new OleDbCommand("INSERT INTO Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) VALUES (?,'ENTRADA','CANCELACION DE ENTREGA A CONSIGNA',?,?,?)", con, transaccion))
                            {
                                cmdK.Parameters.AddWithValue("?", idProducto);
                                cmdK.Parameters.AddWithValue("?", exis);
                                cmdK.Parameters.AddWithValue("?", nuevaExis);
                                cmdK.Parameters.AddWithValue("?", fechaKardex);
                                cmdK.ExecuteNonQuery();
                            }
                        }
                        else if (tipoMov == "DEVOLUCION")
                        {
                            // Si deshacemos una devolución, le regresamos las piezas al cliente y las quitamos de la tienda

                            // A) Actualizar ConsignaCliente
                            using (OleDbCommand cmd = new OleDbCommand("UPDATE ConsignaCliente SET EnConsigna = EnConsigna + ?, Devueltos = Devueltos - ? WHERE ClienteId = ? AND ProductoId = ? AND PrecioCongelado = ?", con, transaccion))
                            {
                                cmd.Parameters.AddWithValue("?", cantidad);
                                cmd.Parameters.AddWithValue("?", cantidad);
                                cmd.Parameters.AddWithValue("?", _idCliente);
                                cmd.Parameters.AddWithValue("?", idProducto);
                                cmd.Parameters.AddWithValue("?", precio);
                                cmd.ExecuteNonQuery();
                            }

                            // B) Restar del Inventario (Y Kardex)
                            double exis = 0;
                            using (OleDbCommand cmdEx = new OleDbCommand("SELECT Existencia FROM Inventario WHERE Id = ?", con, transaccion))
                            {
                                cmdEx.Parameters.AddWithValue("?", idProducto);
                                object res = cmdEx.ExecuteScalar();
                                if (res != null && res != DBNull.Value) exis = Convert.ToDouble(res);
                            }
                            double nuevaExis = exis - cantidad;

                            using (OleDbCommand cmd = new OleDbCommand("UPDATE Inventario SET Existencia = ? WHERE Id = ?", con, transaccion))
                            {
                                cmd.Parameters.AddWithValue("?", nuevaExis);
                                cmd.Parameters.AddWithValue("?", idProducto);
                                cmd.ExecuteNonQuery();
                            }

                            string fechaKardex = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                            using (OleDbCommand cmdK = new OleDbCommand("INSERT INTO Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) VALUES (?,'SALIDA','CANCELACION DE DEVOLUCION DE CONSIGNA',?,?,?)", con, transaccion))
                            {
                                cmdK.Parameters.AddWithValue("?", idProducto);
                                cmdK.Parameters.AddWithValue("?", exis);
                                cmdK.Parameters.AddWithValue("?", nuevaExis);
                                cmdK.Parameters.AddWithValue("?", fechaKardex);
                                cmdK.ExecuteNonQuery();
                            }
                        }

                        // 3. Marcar el movimiento original como CANCELADO para que quede tachado/inhabilitado
                        using (OleDbCommand cmd = new OleDbCommand("UPDATE ConsignaMovimientos SET TipoMovimiento = 'CANCELADA' WHERE Id = ?", con, transaccion))
                        {
                            cmd.Parameters.AddWithValue("?", idMovimiento);
                            cmd.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        MessageBox.Show("Movimiento cancelado con éxito.", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        MessageBox.Show("Error al cancelar el movimiento: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            // Recargamos el grid para mostrar el cambio
            CargarBalance();
            CargarKardex();
        }
        private void btnEntregar_Click(object sender, EventArgs e)
        {
            // 1. Abrimos el modal para entregar nueva mercancía
            // frmConsignaEntregar frm = new frmConsignaEntregar(_idCliente, _nombreCliente);
            // frm.ShowDialog();

            // 2. Al cerrar el modal, recargamos las tablas para que el usuario vea el cambio al instante
            CargarBalance();
            CargarKardex();
        }

        private void btnDevolver_Click(object sender, EventArgs e)
        {
            // Validamos que haya algo en consigna para devolver
            if (dgvBalance.Rows.Count == 0)
            {
                MessageBox.Show("El cliente no tiene mercancía en consigna para devolver.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Validamos que haya seleccionado un renglón
            if (dgvBalance.CurrentRow != null)
            {
                string idProducto = dgvBalance.CurrentRow.Cells["ProductoId"].Value.ToString();
                string nombreProd = dgvBalance.CurrentRow.Cells["Producto"].Value.ToString();
                decimal precioCongelado = Convert.ToDecimal(dgvBalance.CurrentRow.Cells["PrecioCongelado"].Value);
                int cantEnConsigna = Convert.ToInt32(dgvBalance.CurrentRow.Cells["EnConsigna"].Value);

                // Abrir el modal
                frmConsignaDevolver frm = new frmConsignaDevolver(_idCliente, idProducto, nombreProd, precioCongelado, cantEnConsigna);
                frm.ShowDialog();
                CargarBalance();
                CargarKardex();
            }
            else
            {
                MessageBox.Show("Por favor seleccione un producto del balance (tabla superior) para devolver.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            if (dgvBalance.Rows.Count == 0)
            {
                MessageBox.Show("El cliente no tiene mercancía pendiente por pagar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            frmConsignaCobrar frm = new frmConsignaCobrar(_idCliente, _nombreCliente);
            frm.ShowDialog();

            CargarBalance();
            CargarKardex();
        }

        #endregion
      
        private void dgvKardex_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Validamos que le dio clic a un renglón válido y no al encabezado
            if (e.RowIndex >= 0)
            {
                string tipoMov = dgvKardex.Rows[e.RowIndex].Cells["Movimiento"].Value.ToString();

                // Solo abrimos el detalle si fue una venta
                if (tipoMov == "VENTA")
                {
                    string folioTicket = dgvKardex.Rows[e.RowIndex].Cells["FolioTicket"].Value.ToString();

                    if (!string.IsNullOrEmpty(folioTicket))
                    {
                        // Abrimos el formulario de detalle usando nuestro nuevo constructor
                        frmVentaDetallada frm = new frmVentaDetallada(folioTicket);
                        frm.ShowDialog();

                        // Al cerrar el detalle, recargamos el historial y balance
                        // ¡Por si acaso el usuario canceló la venta estando adentro!
                        CargarBalance();
                        CargarKardex();
                    }
                }
                else
                {
                    MessageBox.Show("Solo puedes ver el detalle de los movimientos de tipo 'VENTA'.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
    }