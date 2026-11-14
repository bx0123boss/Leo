using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmDetalleProduccion : frmBase
    {
        private int _idLote;
        private string _folioLote;
        private string _idProductoFinal = "";
        private string _nombreProductoFinal = "";
        private double _cantidadProducida = 0;
        private bool _cancelado = false;
        private DateTime? _fechaLote;
        private string _usuarioLote = "";

        public bool HuboCancelacion { get; private set; } = false;

        public frmDetalleProduccion(int idLote, string folioLote)
        {
            InitializeComponent();
            this._idLote = idLote;
            this._folioLote = folioLote;
            AplicarEstilosPersonalizados();
        }

        private void AplicarEstilosPersonalizados()
        {
            EstilizarDataGridView(dgvInsumos);
            EstilizarDataGridView(dgvKardex);
            EstilizarBotonPeligro(btnCancelar);
        }

        private void frmDetalleProduccion_Load(object sender, EventArgs e)
        {
            CargarDetalle();
        }

        private void CargarDetalle()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
                {
                    con.Open();

                    // 1. Cargar información de la cabecera del lote
                    string queryLote = @"
                        SELECT L.Id, L.Folio, L.IdProductoFinal, L.CantidadProducida, L.Fecha, L.Usuario, 
                               ISNULL(L.Cancelado, 0) AS Cancelado, L.FechaCancelado, L.UsuarioCancelado,
                               I.Nombre AS ProductoFinal
                        FROM ProduccionLotes L
                        LEFT JOIN Inventario I ON L.IdProductoFinal = I.Id
                        WHERE L.Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(queryLote, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", _idLote);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _folioLote = reader["Folio"]?.ToString() ?? "";
                                _idProductoFinal = reader["IdProductoFinal"]?.ToString() ?? "";
                                _nombreProductoFinal = reader["ProductoFinal"]?.ToString() ?? "Producto Desconocido";

                                if (reader["CantidadProducida"] != DBNull.Value)
                                    double.TryParse(reader["CantidadProducida"].ToString(), out _cantidadProducida);

                                if (reader["Fecha"] != DBNull.Value)
                                    _fechaLote = Convert.ToDateTime(reader["Fecha"]);

                                _usuarioLote = reader["Usuario"]?.ToString() ?? "";
                                _cancelado = Convert.ToBoolean(reader["Cancelado"]);

                                string fechaCancStr = reader["FechaCancelado"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["FechaCancelado"]).ToString("yyyy-MM-dd HH:mm")
                                    : "";
                                string usuCanc = reader["UsuarioCancelado"]?.ToString() ?? "";

                                ActualizarCabeceraUI(fechaCancStr, usuCanc);
                            }
                        }
                    }

                    // 2. Cargar Insumos consumidos
                    CargarInsumos(con);

                    // 3. Cargar Kardex asociado a este lote
                    CargarKardex(con);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalle de producción: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarCabeceraUI(string fechaCanc, string usuCanc)
        {
            lblFolio.Text = $"Folio: {_folioLote}";
            lblProducto.Text = $"Producto Elaborado: [{_idProductoFinal}] {_nombreProductoFinal}";
            lblCantidad.Text = $"Cantidad Producida: {_cantidadProducida:N0} piezas";
            lblFecha.Text = $"Fecha: {(_fechaLote.HasValue ? _fechaLote.Value.ToString("yyyy-MM-dd HH:mm:ss") : "N/D")}";
            lblUsuario.Text = $"Productor / Panadero: {_usuarioLote}";

            if (_cancelado)
            {
                lblBadgeEstado.Text = "CANCELADO";
                lblBadgeEstado.BackColor = Color.FromArgb(192, 57, 43);
                lblCancelacionInfo.Visible = true;
                lblCancelacionInfo.Text = $"⚠️ Lote Cancelado el {fechaCanc} por: {usuCanc}";

                btnCancelar.Enabled = false;
                btnCancelar.Text = "⚠️ ESTE LOTE YA ESTÁ CANCELADO";
                btnCancelar.BackColor = Color.FromArgb(70, 70, 70);
                btnCancelar.ForeColor = Color.DarkGray;
            }
            else
            {
                lblBadgeEstado.Text = "ACTIVO";
                lblBadgeEstado.BackColor = Color.FromArgb(39, 174, 96);
                lblCancelacionInfo.Visible = false;

                btnCancelar.Enabled = true;
                btnCancelar.Text = "❌ CANCELAR PRODUCCIÓN (REVERTIR)";
                EstilizarBotonPeligro(btnCancelar);
            }
        }

        private void CargarInsumos(SqlConnection con)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT R.IdInsumo, I.Nombre AS Insumo, I.Uni AS Unidad, 
                       R.CantidadRequerida, 
                       (R.CantidadRequerida * @CantidadProducida) AS CantidadTotal, 
                       ISNULL(I.Especial, 0) AS CostoUnitario,
                       (R.CantidadRequerida * @CantidadProducida * ISNULL(I.Especial, 0)) AS CostoTotal,
                       ISNULL(I.Existencia, 0) AS StockActual
                FROM ProductoReceta R
                INNER JOIN Inventario I ON R.IdInsumo = I.Id
                WHERE R.IdProductoFinal = @IdPan
                ORDER BY I.Nombre ASC";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@CantidadProducida", _cantidadProducida);
                cmd.Parameters.AddWithValue("@IdPan", _idProductoFinal);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            dgvInsumos.DataSource = dt;

            // Formatear columnas
            if (dgvInsumos.Columns.Count > 0)
            {
                if (dgvInsumos.Columns.Contains("IdInsumo"))
                {
                    dgvInsumos.Columns["IdInsumo"].HeaderText = "Código";
                    dgvInsumos.Columns["IdInsumo"].Width = 90;
                }
                if (dgvInsumos.Columns.Contains("Insumo"))
                    dgvInsumos.Columns["Insumo"].HeaderText = "Materia Prima / Insumo";

                if (dgvInsumos.Columns.Contains("Unidad"))
                {
                    dgvInsumos.Columns["Unidad"].HeaderText = "Unidad";
                    dgvInsumos.Columns["Unidad"].Width = 80;
                }

                if (dgvInsumos.Columns.Contains("CantidadRequerida"))
                {
                    dgvInsumos.Columns["CantidadRequerida"].HeaderText = "Cant. x Pieza";
                    dgvInsumos.Columns["CantidadRequerida"].DefaultCellStyle.Format = "N4";
                }

                if (dgvInsumos.Columns.Contains("CantidadTotal"))
                {
                    dgvInsumos.Columns["CantidadTotal"].HeaderText = "Total Descontado";
                    dgvInsumos.Columns["CantidadTotal"].DefaultCellStyle.Format = "N3";
                    dgvInsumos.Columns["CantidadTotal"].DefaultCellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                }

                if (dgvInsumos.Columns.Contains("CostoUnitario"))
                {
                    dgvInsumos.Columns["CostoUnitario"].HeaderText = "Costo Unit.";
                    dgvInsumos.Columns["CostoUnitario"].DefaultCellStyle.Format = "C2";
                }

                if (dgvInsumos.Columns.Contains("CostoTotal"))
                {
                    dgvInsumos.Columns["CostoTotal"].HeaderText = "Costo Total";
                    dgvInsumos.Columns["CostoTotal"].DefaultCellStyle.Format = "C2";
                }

                if (dgvInsumos.Columns.Contains("StockActual"))
                {
                    dgvInsumos.Columns["StockActual"].HeaderText = "Stock Actual";
                    dgvInsumos.Columns["StockActual"].DefaultCellStyle.Format = "N2";
                }

                dgvInsumos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }

            // Calcular costo total
            decimal sumaCostos = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["CostoTotal"] != DBNull.Value)
                    sumaCostos += Convert.ToDecimal(row["CostoTotal"]);
            }
            lblCostoTotal.Text = $"Costo Total Estimado Insumos: {sumaCostos:C2}";
        }

        private void CargarKardex(SqlConnection con)
        {
            DataTable dtKardex = new DataTable();
            string queryKardex = @"
                SELECT K.Id, K.Fecha, K.Tipo, K.IdProducto, I.Nombre AS Producto, K.Descripcion, 
                       (K.ExistenciaDespues - K.ExistenciaAntes) AS Movimiento,
                       K.ExistenciaAntes, K.ExistenciaDespues
                FROM Kardex K
                LEFT JOIN Inventario I ON K.IdProducto = I.Id
                WHERE K.Descripcion LIKE @DescFiltro
                ORDER BY K.Id ASC";

            using (SqlCommand cmd = new SqlCommand(queryKardex, con))
            {
                cmd.Parameters.AddWithValue("@DescFiltro", $"%LOTE: {_folioLote}%");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtKardex);
            }

            dgvKardex.DataSource = dtKardex;

            if (dgvKardex.Columns.Count > 0)
            {
                if (dgvKardex.Columns.Contains("Id")) dgvKardex.Columns["Id"].Width = 60;
                if (dgvKardex.Columns.Contains("Fecha")) dgvKardex.Columns["Fecha"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
                if (dgvKardex.Columns.Contains("Tipo")) dgvKardex.Columns["Tipo"].Width = 90;
                if (dgvKardex.Columns.Contains("IdProducto")) dgvKardex.Columns["IdProducto"].Width = 90;
                if (dgvKardex.Columns.Contains("Movimiento")) dgvKardex.Columns["Movimiento"].DefaultCellStyle.Format = "N2";
                if (dgvKardex.Columns.Contains("ExistenciaAntes")) dgvKardex.Columns["ExistenciaAntes"].DefaultCellStyle.Format = "N2";
                if (dgvKardex.Columns.Contains("ExistenciaDespues")) dgvKardex.Columns["ExistenciaDespues"].DefaultCellStyle.Format = "N2";

                dgvKardex.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (_cancelado)
            {
                MessageBox.Show("Este lote ya fue cancelado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string mensaje = $"¿Desea cancelar la producción del lote '{_folioLote}'?\n\n" +
                             $"Producto: {_nombreProductoFinal}\n" +
                             $"Cantidad Producida: {_cantidadProducida} pz\n\n" +
                             "Acciones a realizar:\n" +
                             "1. Restar las piezas elaboradas del inventario y registrar SALIDA en Kardex.\n" +
                             "2. Devolver los insumos consumidos al inventario y registrar ENTRADA en Kardex.\n" +
                             "3. Marcar el lote como CANCELADO permanentemente.\n\n" +
                             "¿Desea continuar?";

            if (MessageBox.Show(mensaje, "Confirmar Cancelación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
                {
                    con.Open();
                    using (SqlTransaction trans = con.BeginTransaction())
                    {
                        try
                        {
                            // Verificar si ya fue cancelado
                            string qCheck = "SELECT Cancelado FROM ProduccionLotes WHERE Id = @Id";
                            using (SqlCommand cmdChk = new SqlCommand(qCheck, con, trans))
                            {
                                cmdChk.Parameters.AddWithValue("@Id", _idLote);
                                object resChk = cmdChk.ExecuteScalar();
                                if (resChk != null && resChk != DBNull.Value && Convert.ToBoolean(resChk))
                                {
                                    trans.Rollback();
                                    MessageBox.Show("El lote ya se encuentra cancelado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    CargarDetalle();
                                    return;
                                }
                            }

                            string fechaStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string usuarioActual = string.IsNullOrEmpty(Sesion.NombreUsuario) ? "Sistema" : Sesion.NombreUsuario;

                            // 1. REVERTIR PRODUCTO ELABORADO (Restar del inventario)
                            double stockPanActual = 0;
                            string qStockPan = "SELECT Existencia FROM Inventario WHERE Id = @Id";
                            using (SqlCommand cmdPan = new SqlCommand(qStockPan, con, trans))
                            {
                                cmdPan.Parameters.AddWithValue("@Id", _idProductoFinal);
                                object objStock = cmdPan.ExecuteScalar();
                                if (objStock != null && objStock != DBNull.Value)
                                    double.TryParse(objStock.ToString(), out stockPanActual);
                            }

                            double nuevoStockPan = stockPanActual - _cantidadProducida;

                            if (nuevoStockPan < 0)
                            {
                                DialogResult warnRes = MessageBox.Show(
                                    $"Aviso: La existencia actual de '{_nombreProductoFinal}' es {stockPanActual} pz.\n" +
                                    $"Al restar {_cantidadProducida} pz, el stock quedará en negativo ({nuevoStockPan} pz).\n\n" +
                                    "¿Desea continuar con la cancelación de todas formas?",
                                    "Stock insuficiente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (warnRes != DialogResult.Yes)
                                {
                                    trans.Rollback();
                                    return;
                                }
                            }

                            string qUpdPan = "UPDATE Inventario SET Existencia = @Existencia WHERE Id = @Id";
                            using (SqlCommand cmdUpdPan = new SqlCommand(qUpdPan, con, trans))
                            {
                                cmdUpdPan.Parameters.AddWithValue("@Existencia", nuevoStockPan);
                                cmdUpdPan.Parameters.AddWithValue("@Id", _idProductoFinal);
                                cmdUpdPan.ExecuteNonQuery();
                            }

                            string qKardexPan = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                                  VALUES (@IdProducto, 'SALIDA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
                            using (SqlCommand cmdKPan = new SqlCommand(qKardexPan, con, trans))
                            {
                                cmdKPan.Parameters.AddWithValue("@IdProducto", _idProductoFinal);
                                cmdKPan.Parameters.AddWithValue("@Descripcion", $"CANCELACION PRODUCCION LOTE: {_folioLote} ({_nombreProductoFinal})");
                                cmdKPan.Parameters.AddWithValue("@ExistenciaAntes", stockPanActual);
                                cmdKPan.Parameters.AddWithValue("@ExistenciaDespues", nuevoStockPan);
                                cmdKPan.Parameters.AddWithValue("@Fecha", fechaStr);
                                cmdKPan.ExecuteNonQuery();
                            }

                            // 2. REVERTIR INSUMOS (Devolver al stock de inventario)
                            List<Tuple<string, double, string>> insumosAReintegrar = new List<Tuple<string, double, string>>();

                            // Buscar primero en Kardex los consumos específicos de este folio
                            string qKInsumos = @"SELECT K.IdProducto, (K.ExistenciaAntes - K.ExistenciaDespues) AS Consumo, I.Nombre 
                                                 FROM Kardex K 
                                                 INNER JOIN Inventario I ON K.IdProducto = I.Id
                                                 WHERE K.Descripcion LIKE @DescLote AND K.Tipo = 'SALIDA'";
                            using (SqlCommand cmdK = new SqlCommand(qKInsumos, con, trans))
                            {
                                cmdK.Parameters.AddWithValue("@DescLote", $"CONSUMO PRODUCCION LOTE: {_folioLote}%");
                                using (SqlDataReader rdr = cmdK.ExecuteReader())
                                {
                                    while (rdr.Read())
                                    {
                                        string idIn = rdr["IdProducto"].ToString();
                                        double cant = Convert.ToDouble(rdr["Consumo"]);
                                        string nom = rdr["Nombre"].ToString();
                                        insumosAReintegrar.Add(new Tuple<string, double, string>(idIn, cant, nom));
                                    }
                                }
                            }

                            // Si no hubo registros en Kardex, calculamos con la receta
                            if (insumosAReintegrar.Count == 0)
                            {
                                string qReceta = @"SELECT R.IdInsumo, (R.CantidadRequerida * @CantProd) AS Consumo, I.Nombre 
                                                   FROM ProductoReceta R
                                                   INNER JOIN Inventario I ON R.IdInsumo = I.Id
                                                   WHERE R.IdProductoFinal = @IdPan";
                                using (SqlCommand cmdR = new SqlCommand(qReceta, con, trans))
                                {
                                    cmdR.Parameters.AddWithValue("@CantProd", _cantidadProducida);
                                    cmdR.Parameters.AddWithValue("@IdPan", _idProductoFinal);
                                    using (SqlDataReader rdr = cmdR.ExecuteReader())
                                    {
                                        while (rdr.Read())
                                        {
                                            string idIn = rdr["IdInsumo"].ToString();
                                            double cant = Convert.ToDouble(rdr["Consumo"]);
                                            string nom = rdr["Nombre"].ToString();
                                            insumosAReintegrar.Add(new Tuple<string, double, string>(idIn, cant, nom));
                                        }
                                    }
                                }
                            }

                            foreach (var ins in insumosAReintegrar)
                            {
                                string idIns = ins.Item1;
                                double cantReintegrar = ins.Item2;
                                string nomIns = ins.Item3;

                                double stockInsActual = 0;
                                string qStk = "SELECT Existencia FROM Inventario WHERE Id = @Id";
                                using (SqlCommand cmdS = new SqlCommand(qStk, con, trans))
                                {
                                    cmdS.Parameters.AddWithValue("@Id", idIns);
                                    object objS = cmdS.ExecuteScalar();
                                    if (objS != null && objS != DBNull.Value)
                                        double.TryParse(objS.ToString(), out stockInsActual);
                                }

                                double nuevoStockIns = stockInsActual + cantReintegrar;

                                string qUpdIns = "UPDATE Inventario SET Existencia = @Existencia WHERE Id = @Id";
                                using (SqlCommand cmdUpdIn = new SqlCommand(qUpdIns, con, trans))
                                {
                                    cmdUpdIn.Parameters.AddWithValue("@Existencia", nuevoStockIns);
                                    cmdUpdIn.Parameters.AddWithValue("@Id", idIns);
                                    cmdUpdIn.ExecuteNonQuery();
                                }

                                string qKIn = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                               VALUES (@IdProducto, 'ENTRADA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
                                using (SqlCommand cmdK = new SqlCommand(qKIn, con, trans))
                                {
                                    cmdK.Parameters.AddWithValue("@IdProducto", idIns);
                                    cmdK.Parameters.AddWithValue("@Descripcion", $"CANCELACION CONSUMO PRODUCCION LOTE: {_folioLote} ({_nombreProductoFinal})");
                                    cmdK.Parameters.AddWithValue("@ExistenciaAntes", stockInsActual);
                                    cmdK.Parameters.AddWithValue("@ExistenciaDespues", nuevoStockIns);
                                    cmdK.Parameters.AddWithValue("@Fecha", fechaStr);
                                    cmdK.ExecuteNonQuery();
                                }
                            }

                            // 3. MARCAR LOTE COMO CANCELADO
                            string qCancel = @"UPDATE ProduccionLotes 
                                               SET Cancelado = 1, FechaCancelado = GETDATE(), UsuarioCancelado = @Usuario 
                                               WHERE Id = @Id";
                            using (SqlCommand cmdCanc = new SqlCommand(qCancel, con, trans))
                            {
                                cmdCanc.Parameters.AddWithValue("@Usuario", usuarioActual);
                                cmdCanc.Parameters.AddWithValue("@Id", _idLote);
                                cmdCanc.ExecuteNonQuery();
                            }

                            trans.Commit();
                            HuboCancelacion = true;
                            MessageBox.Show("¡Lote cancelado correctamente!\n\nSe descontaron las piezas de producto terminado y se reintegraron los insumos al stock.", "Cancelación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            CargarDetalle();
                        }
                        catch (Exception exTrans)
                        {
                            trans.Rollback();
                            MessageBox.Show("Error durante la transacción de cancelación: " + exTrans.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception exCon)
            {
                MessageBox.Show("Error de conexión al cancelar el lote: " + exCon.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            if (HuboCancelacion)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;

            this.Close();
        }
    }
}
