using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmDetalleCorte : frmBase
    {
        public int ID;
        public DataSet ds;
        public string usuario = "";
        public bool EsCorteB { get; set; } = false;

        // Variables de cálculo
        double apertura = 0;
        double mas = 0;
        double entradas = 0;
        double salidas = 0;
        double cancelaciones = 0;
        double tarjeta = 0;
        double trans = 0;
        double otros = 0;
        double total = 0, utilidad = 0, inversion = 0, gastos = 0;
        private decimal porcentaje = 0m;

        private string fechaInicioTurno = "";
        private string fechaFinTurno = "";

        public frmDetalleCorte()
        {
            InitializeComponent();
            this.MinimumSize = new Size(969, 772);
        }

        private bool EsCeldaValida(DataGridView dgv, DataGridViewCellFormattingEventArgs e)
        {
            return dgv != null
                   && !dgv.IsDisposed
                   && !this.Disposing
                   && e.RowIndex >= 0
                   && e.ColumnIndex >= 0
                   && e.ColumnIndex < dgv.Columns.Count;
        }

        private void FormatearColumnaMoneda(DataGridView dgv, DataGridViewCellFormattingEventArgs e)
        {
            if (!EsCeldaValida(dgv, e)) return;
            if (dgv.Columns[e.ColumnIndex].Name == "Monto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void dgvCorte_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) => FormatearColumnaMoneda(dgvCorte, e);
        private void dgvEntradas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) => FormatearColumnaMoneda(dgvEntradas, e);
        private void dgvSalidas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) => FormatearColumnaMoneda(dgvSalidas, e);
        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) => FormatearColumnaMoneda(dataGridView3, e);
        private void dataGridView4_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) => FormatearColumnaMoneda(dataGridView4, e);
        private void dgvOtrosPagos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) => FormatearColumnaMoneda(dgvOtrosPagos, e);

        private void dgvFolios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!EsCeldaValida(dgvFolios, e)) return;
            string col = dgvFolios.Columns[e.ColumnIndex].Name;
            if (col == "Monto" || col == "Descuento" || col == "Monto sin IVA" || col == "IVA")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void dataGridView6_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!EsCeldaValida(dataGridView6, e)) return;
            if (dataGridView6.Columns[e.ColumnIndex].Name == "Total")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
            else if (dataGridView6.Columns[e.ColumnIndex].Name == "Piezas")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("N0");
                    e.FormattingApplied = true;
                }
            }

            if (e.RowIndex >= 0 && dataGridView6.Rows[e.RowIndex].Cells["Categoria"].Value?.ToString() == "TOTAL GENERAL")
            {
                dataGridView6.Rows[e.RowIndex].DefaultCellStyle.Font = new System.Drawing.Font(dataGridView6.Font, FontStyle.Bold);
                dataGridView6.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                dataGridView6.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
        }

        private void dgvCategoriasAgrupadas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!EsCeldaValida(dgvCategoriasAgrupadas, e)) return;
            if (dgvCategoriasAgrupadas.Columns[e.ColumnIndex].Name == "Total")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
            else if (dgvCategoriasAgrupadas.Columns[e.ColumnIndex].Name == "Piezas")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("N0");
                    e.FormattingApplied = true;
                }
            }

            if (e.RowIndex >= 0 && dgvCategoriasAgrupadas.Rows[e.RowIndex].Cells["Grupo"].Value?.ToString() == "TOTAL GENERAL")
            {
                dgvCategoriasAgrupadas.Rows[e.RowIndex].DefaultCellStyle.Font = new System.Drawing.Font(dgvCategoriasAgrupadas.Font, FontStyle.Bold);
                dgvCategoriasAgrupadas.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                dgvCategoriasAgrupadas.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
        }

        private void frmDetalleCorte_Load(object sender, EventArgs e)
        {
            if (usuario == "Invitado")
            {
                label9.Hide();
                lblInversion.Hide();
                label14.Hide();
                lblUtilidad.Hide();
                label10.Hide();
                lblGastos.Hide();
                label11.Hide();
                lblBruta.Hide();
            }
            if (Sesion.TienePermiso("UTILIDAD_CORTE"))
            {
                UtilidadPanel.Show();
            }

            string tablaDetalle = EsCorteB ? "CortesB" : "Cortes";
            string tablaHisto = EsCorteB ? "histocortesB" : "histocortes";
            string tablaVentas = EsCorteB ? "VentasB" : "Ventas";

            this.Text = EsCorteB ? $"Detalle de Corte B - Folio #{ID}" : $"Detalle de Corte - Folio #{ID}";

            using (SqlConnection conectar = new SqlConnection(Conexion.CadSQL))
            {
                conectar.Open();

                // 0. Porcentaje de comisión tarjeta
                using (SqlCommand cmd = new SqlCommand("SELECT Porcentaje FROM Tarjeta WHERE Id=1;", conectar))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        porcentaje = Convert.ToDecimal(reader[0].ToString());
                    }
                }
                lblComision.Text = $"{porcentaje:F2}% Comisión:";

                // 0.1 Obtener fechas de cierre actual y corte anterior para reconstruir rango de folios/categorías
                DateTime dtFin = DateTime.Now;
                DateTime dtInicio = DateTime.Now.Date;

                using (SqlCommand cmdHisto = new SqlCommand($"SELECT Fecha, Utilidad, Inversion FROM {tablaHisto} WHERE Id = @Id;", conectar))
                {
                    cmdHisto.Parameters.AddWithValue("@Id", ID);
                    using (SqlDataReader reader = cmdHisto.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["Fecha"] != DBNull.Value && DateTime.TryParse(reader["Fecha"].ToString(), out DateTime fCierre))
                            {
                                dtFin = fCierre;
                                lblFecha.Text = fCierre.ToString("dd/MM/yyyy HH:mm:ss");
                            }
                            if (reader["Utilidad"] != DBNull.Value)
                                double.TryParse(Regex.Replace(reader["Utilidad"].ToString(), @"[^\d.-]", ""), out utilidad);
                            if (reader["Inversion"] != DBNull.Value)
                                double.TryParse(Regex.Replace(reader["Inversion"].ToString(), @"[^\d.-]", ""), out inversion);
                        }
                    }
                }

                // Buscar la fecha del corte inmediatamente anterior como inicio de turno
                using (SqlCommand cmdPrev = new SqlCommand($"SELECT TOP 1 Fecha FROM {tablaHisto} WHERE Id < @Id ORDER BY Id DESC;", conectar))
                {
                    cmdPrev.Parameters.AddWithValue("@Id", ID);
                    object prevObj = cmdPrev.ExecuteScalar();
                    if (prevObj != null && prevObj != DBNull.Value && DateTime.TryParse(prevObj.ToString(), out DateTime fPrev))
                    {
                        dtInicio = fPrev.AddSeconds(1);
                    }
                    else
                    {
                        // Si es el primer corte registrado, usar el primer movimiento o inicio del día
                        using (SqlCommand cmdMinMov = new SqlCommand($"SELECT MIN(FechaHora) FROM {tablaDetalle} WHERE idCorte = @Id;", conectar))
                        {
                            cmdMinMov.Parameters.AddWithValue("@Id", ID);
                            object minObj = cmdMinMov.ExecuteScalar();
                            if (minObj != null && minObj != DBNull.Value && DateTime.TryParse(minObj.ToString(), out DateTime fMin))
                                dtInicio = fMin.AddMinutes(-5);
                            else
                                dtInicio = dtFin.Date;
                        }
                    }
                }

                fechaInicioTurno = dtInicio.ToString("yyyy-MM-dd HH:mm:ss");
                fechaFinTurno = dtFin.ToString("yyyy-MM-dd HH:mm:ss");

                // 1. EFECTIVO (Columna de pago en Cortes es 'Tipo')
                ds = new DataSet();
                string qEfectivo = $@"
                    SELECT * FROM {tablaDetalle} 
                    WHERE idCorte = @Id
                      AND (Tipo='01=EFECTIVO' OR Tipo='Efectivo') 
                      AND Concepto NOT LIKE 'Abono%' 
                      AND Concepto <> 'APERTURA DE CAJA'
                      AND Concepto NOT LIKE 'Entrada en caja por concepto%' 
                      AND Concepto NOT LIKE 'Salida en caja por concepto%';";
                using (SqlDataAdapter da = new SqlDataAdapter(qEfectivo, conectar))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", ID);
                    da.Fill(ds, "Efectivo");
                    dgvCorte.DataSource = ds.Tables["Efectivo"];
                    OcultarColumnasInternas(dgvCorte);
                }

                // 1.1 ENTRADAS (Incluye APERTURA DE CAJA y Entradas manuales)
                ds = new DataSet();
                string qEntradas = $@"
                    SELECT * FROM {tablaDetalle} 
                    WHERE idCorte = @Id
                      AND (Concepto LIKE 'Entrada en caja por concepto%' OR Concepto = 'APERTURA DE CAJA');";
                using (SqlDataAdapter da = new SqlDataAdapter(qEntradas, conectar))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", ID);
                    da.Fill(ds, "Entradas");
                    dgvEntradas.DataSource = ds.Tables["Entradas"];
                    OcultarColumnasInternas(dgvEntradas);
                }

                // 1.2 SALIDAS
                ds = new DataSet();
                string qSalidas = $@"
                    SELECT * FROM {tablaDetalle} 
                    WHERE idCorte = @Id
                      AND Concepto LIKE 'Salida en caja por concepto%';";
                using (SqlDataAdapter da = new SqlDataAdapter(qSalidas, conectar))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", ID);
                    da.Fill(ds, "Salidas");
                    dgvSalidas.DataSource = ds.Tables["Salidas"];
                    OcultarColumnasInternas(dgvSalidas);
                }

                // 2. TARJETAS
                ds = new DataSet();
                string qTarjetas = $@"
                    SELECT * FROM {tablaDetalle} 
                    WHERE idCorte = @Id
                      AND (Tipo='04=TARJETA DE CREDITO' OR Tipo='28=TARJETA DE DEBITO' OR Tipo='Tarjeta') 
                      AND Concepto NOT LIKE 'Abono%';";
                using (SqlDataAdapter da = new SqlDataAdapter(qTarjetas, conectar))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", ID);
                    da.Fill(ds, "Tarjetas");
                    dataGridView3.DataSource = ds.Tables["Tarjetas"];
                    OcultarColumnasInternas(dataGridView3);
                }

                // 3. TRANSFERENCIAS
                ds = new DataSet();
                string qTrans = $@"
                    SELECT * FROM {tablaDetalle} 
                    WHERE idCorte = @Id
                      AND (Tipo='03=TRANFERENCIA ELECTRONICA DE FONDOS' OR Tipo='Transferencia') 
                      AND Concepto NOT LIKE 'Abono%';";
                using (SqlDataAdapter da = new SqlDataAdapter(qTrans, conectar))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", ID);
                    da.Fill(ds, "Trans");
                    dataGridView4.DataSource = ds.Tables["Trans"];
                    OcultarColumnasInternas(dataGridView4);
                }

                // 4. OTROS PAGOS
                ds = new DataSet();
                string qOtros = $@"
                    SELECT * FROM {tablaDetalle} 
                    WHERE idCorte = @Id
                      AND Tipo NOT IN ('01=EFECTIVO','Transferencia','Tarjeta','Efectivo','04=TARJETA DE CREDITO','28=TARJETA DE DEBITO','03=TRANFERENCIA ELECTRONICA DE FONDOS') 
                      AND Concepto NOT LIKE 'Abono%' 
                      AND Concepto NOT LIKE 'Entrada en caja por concepto%' 
                      AND Concepto NOT LIKE 'Salida en caja por concepto%';";
                using (SqlDataAdapter da = new SqlDataAdapter(qOtros, conectar))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", ID);
                    da.Fill(ds, "Otros");
                    dgvOtrosPagos.DataSource = ds.Tables["Otros"];
                    OcultarColumnasInternas(dgvOtrosPagos);
                }

                // 5. ABONOS
                ds = new DataSet();
                string qAbonos = $@"
                    SELECT * FROM {tablaDetalle} 
                    WHERE idCorte = @Id
                      AND Concepto LIKE 'Abono%';";
                using (SqlDataAdapter da = new SqlDataAdapter(qAbonos, conectar))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", ID);
                    da.Fill(ds, "Abonos");
                    dataGridView5.DataSource = ds.Tables["Abonos"];
                    OcultarColumnasInternas(dataGridView5);
                }

                // 6. POR CATEGORÍA INDIVIDUAL
                ds = new DataSet();
                string queryCat = $@"
                    SELECT IIf(Categoria Is Null OR Categoria = '', 'SIN CATEGORIA', Categoria) AS Categoria, 
                           SUM(Cantidad) AS Piezas, 
                           SUM(MontoTotal) AS Total 
                    FROM VentasContado 
                    WHERE Fecha >= '{fechaInicioTurno}' AND Fecha <= '{fechaFinTurno}' 
                      AND FolioVenta IN (SELECT Folio FROM {tablaVentas} WHERE Estatus = 'COBRADO') 
                    GROUP BY IIf(Categoria Is Null OR Categoria = '', 'SIN CATEGORIA', Categoria) 
                    UNION ALL 
                    SELECT 'TOTAL GENERAL' AS Categoria, 
                           ISNULL(SUM(Cantidad), 0) AS Piezas, 
                           ISNULL(SUM(MontoTotal), 0) AS Total 
                    FROM VentasContado 
                    WHERE Fecha >= '{fechaInicioTurno}' AND Fecha <= '{fechaFinTurno}' 
                      AND FolioVenta IN (SELECT Folio FROM {tablaVentas} WHERE Estatus = 'COBRADO');";
                using (SqlDataAdapter da = new SqlDataAdapter(queryCat, conectar))
                {
                    da.Fill(ds, "Categorias");
                    dataGridView6.DataSource = ds.Tables["Categorias"];
                }

                // 7. CATEGORÍAS AGRUPADAS
                ds = new DataSet();
                string queryAgrupadas = $@"
                    SELECT Grupo, Piezas, Total
                    FROM (
                        SELECT 
                            ISNULL(g.Nombre, 'Sin Grupo') AS Grupo,
                            SUM(vc.Cantidad) AS Piezas,
                            SUM(vc.MontoTotal) AS Total,
                            0 AS Orden
                        FROM VentasContado vc
                        INNER JOIN {tablaVentas} v ON v.Folio = vc.FolioVenta
                        INNER JOIN INVENTARIO i ON vc.IdProducto = i.Id
                        LEFT JOIN Categorias c ON c.Nombre = i.Categoria
                        LEFT JOIN GruposCategoriasDetalle d ON c.Id = d.IdCategoria
                        LEFT JOIN GruposCategorias g ON d.IdGrupo = g.IdGrupo AND g.Activo = 1
                        WHERE v.Estatus = 'Cobrado'
                          AND v.Fecha >= '{fechaInicioTurno}' 
                          AND v.Fecha <= '{fechaFinTurno}'
                        GROUP BY ISNULL(g.Nombre, 'Sin Grupo')

                        UNION ALL

                        SELECT 
                            'TOTAL GENERAL' AS Grupo,
                            ISNULL(SUM(vc.Cantidad), 0) AS Piezas,
                            ISNULL(SUM(vc.MontoTotal), 0) AS Total,
                            1 AS Orden
                        FROM VentasContado vc
                        INNER JOIN {tablaVentas} v ON v.Folio = vc.FolioVenta
                        INNER JOIN INVENTARIO i ON vc.IdProducto = i.Id
                        LEFT JOIN Categorias c ON c.Nombre = i.Categoria
                        LEFT JOIN GruposCategoriasDetalle d ON c.Id = d.IdCategoria
                        LEFT JOIN GruposCategorias g ON d.IdGrupo = g.IdGrupo AND g.Activo = 1
                        WHERE v.Estatus = 'Cobrado'
                          AND v.Fecha >= '{fechaInicioTurno}' 
                          AND v.Fecha <= '{fechaFinTurno}'
                    ) AS Resumen
                    ORDER BY Orden ASC, Total DESC;";
                using (SqlDataAdapter da = new SqlDataAdapter(queryAgrupadas, conectar))
                {
                    da.Fill(ds, "Agrupadas");
                    dgvCategoriasAgrupadas.DataSource = ds.Tables["Agrupadas"];
                }

                // 8. FOLIOS DEL CORTE
                ds = new DataSet();
                string colFolio = EsCorteB ? "FolioB AS Folio" : "Folio";
                string queryFolios = $@"
                    SELECT Id, {colFolio}, 
                           CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)) / (1 + (16.0 / 100)) AS [Monto sin IVA], 
                           CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)) - (CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)) / (1 + (16.0 / 100))) AS [IVA], 
                           Monto, Descuento, Fecha, Estatus, Pago 
                    FROM {tablaVentas} 
                    WHERE Estatus = 'COBRADO' AND Fecha >= '{fechaInicioTurno}' AND Fecha <= '{fechaFinTurno}';";
                using (SqlDataAdapter da = new SqlDataAdapter(queryFolios, conectar))
                {
                    da.Fill(ds, "Folios");
                    dgvFolios.DataSource = ds.Tables["Folios"];
                    if (dgvFolios.Columns.Count > 0)
                        dgvFolios.Columns[0].Visible = false;
                }

                // 9. GASTOS DEL TURNO
                ds = new DataSet();
                using (SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM GastosDetallados WHERE Fecha >= '{fechaInicioTurno}' AND Fecha <= '{fechaFinTurno}';", conectar))
                {
                    da.Fill(ds, "Gastos");
                    dataGridView2.DataSource = ds.Tables["Gastos"];
                }
            }

            // ======================================================================
            // CÁLCULOS DE MONTOS (Esquema Cortes: [1]=Concepto, [2]=Monto, [5]=Tipo)
            // ======================================================================

            // Efectivo
            for (int i = 0; i < dgvCorte.RowCount; i++)
            {
                if (dgvCorte.Rows[i].IsNewRow) continue;
                if (dgvCorte[2, i].Value != null && dgvCorte[2, i].Value != DBNull.Value)
                {
                    double valorNumerico = Convert.ToDouble(dgvCorte[2, i].Value);
                    string concepto = dgvCorte[1, i].Value?.ToString().Trim() ?? "";

                    if (concepto.IndexOf("Cancelacion", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        concepto.IndexOf("Cancelación", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        cancelaciones += Math.Abs(valorNumerico);
                    }
                    else
                    {
                        if (valorNumerico > 0) mas += valorNumerico;
                        else if (valorNumerico < 0) salidas += Math.Abs(valorNumerico);
                    }
                }
            }

            // Entradas (Separando Apertura de Caja vs Entradas)
            for (int i = 0; i < dgvEntradas.RowCount; i++)
            {
                if (dgvEntradas.Rows[i].IsNewRow) continue;
                if (dgvEntradas[2, i].Value != null && dgvEntradas[2, i].Value != DBNull.Value)
                {
                    double valorNumerico = Convert.ToDouble(dgvEntradas[2, i].Value);
                    string concepto = dgvEntradas[1, i].Value?.ToString().Trim() ?? "";

                    if (concepto.Equals("APERTURA DE CAJA", StringComparison.OrdinalIgnoreCase))
                        apertura += valorNumerico;
                    else
                        entradas += Math.Abs(valorNumerico);
                }
            }

            // Salidas
            for (int i = 0; i < dgvSalidas.RowCount; i++)
            {
                if (dgvSalidas.Rows[i].IsNewRow) continue;
                if (dgvSalidas[2, i].Value != null && dgvSalidas[2, i].Value != DBNull.Value)
                {
                    salidas += Math.Abs(Convert.ToDouble(dgvSalidas[2, i].Value));
                }
            }

            // Tarjetas
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                if (dataGridView3.Rows[i].IsNewRow) continue;
                if (dataGridView3[2, i].Value != null && dataGridView3[2, i].Value != DBNull.Value)
                    tarjeta += Convert.ToDouble(dataGridView3[2, i].Value);
            }

            // Transferencias
            for (int i = 0; i < dataGridView4.RowCount; i++)
            {
                if (dataGridView4.Rows[i].IsNewRow) continue;
                if (dataGridView4[2, i].Value != null && dataGridView4[2, i].Value != DBNull.Value)
                    trans += Convert.ToDouble(dataGridView4[2, i].Value);
            }

            // Otros Pagos
            for (int i = 0; i < dgvOtrosPagos.RowCount; i++)
            {
                if (dgvOtrosPagos.Rows[i].IsNewRow) continue;
                if (dgvOtrosPagos[2, i].Value != null && dgvOtrosPagos[2, i].Value != DBNull.Value)
                    otros += Convert.ToDouble(dgvOtrosPagos[2, i].Value);
            }

            // Abonos
            for (int i = 0; i < dataGridView5.RowCount; i++)
            {
                if (dataGridView5.Rows[i].IsNewRow) continue;
                if (dataGridView5[2, i].Value != null && dataGridView5[2, i].Value != DBNull.Value)
                {
                    double valorNumerico = Convert.ToDouble(dataGridView5[2, i].Value);
                    string tipoPago = dataGridView5.Columns.Contains("Tipo") && dataGridView5.Rows[i].Cells["Tipo"].Value != null
                                      ? dataGridView5.Rows[i].Cells["Tipo"].Value.ToString().ToUpper()
                                      : "";

                    if (tipoPago.Contains("EFECTIVO") || tipoPago.Contains("01="))
                    {
                        if (valorNumerico < 0) salidas += Math.Abs(valorNumerico);
                        else if (valorNumerico > 0) mas += valorNumerico;
                    }
                    else if (tipoPago.Contains("TARJETA") || tipoPago.Contains("04=") || tipoPago.Contains("28="))
                        tarjeta += valorNumerico;
                    else if (tipoPago.Contains("TRANFERENCIA") || tipoPago.Contains("TRANSFERENCIA") || tipoPago.Contains("03="))
                        trans += valorNumerico;
                    else
                        otros += valorNumerico;
                }
            }

            // Gastos
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                if (dataGridView2.Rows[i].IsNewRow) continue;
                if (dataGridView2[2, i].Value != null && dataGridView2[2, i].Value != DBNull.Value)
                    gastos += Convert.ToDouble(dataGridView2[2, i].Value);
            }

            // ======================================================================
            // ASIGNACIÓN DE TEXTOS A LAS ETIQUETAS
            // ======================================================================
            lblApertura.Text = $"{apertura:C}";
            lblCorte.Text = $"{mas:C}";
            lblEntrada.Text = $"{entradas:C}";
            lblSalida.Text = $"{salidas:C}";
            lblCancelaciones.Text = $"{cancelaciones:C}";

            double efectivoEnCaja = apertura + mas + entradas - salidas - cancelaciones;
            lblECaja.Text = $"{efectivoEnCaja:C}";

            decimal factorPorcentaje = porcentaje / 100m;
            decimal factorRestante = 1m - factorPorcentaje;

            decimal tarjetaSinComision = (decimal)tarjeta * factorRestante;
            decimal comisionTarjeta = (decimal)tarjeta * factorPorcentaje;

            lblCredito.Text = $"{tarjetaSinComision:C2}";
            lbl5por.Text = $"{comisionTarjeta:C2}";
            lblTrans.Text = $"{trans:C}";
            lblOtros.Text = $"{otros:C}";

            decimal tn = Convert.ToDecimal(mas) + tarjetaSinComision + comisionTarjeta + Convert.ToDecimal(trans) + Convert.ToDecimal(otros);
            decimal subtotal = tn / 1.16m;
            decimal iva = tn * 0.16m;

            lblNoIva.Text = $"{subtotal:C2}";
            lblIVA.Text = $"{iva:C2}";
            lblTotal.Text = $"{tn:C2}";

            // Sincronizar también propiedades legadas por compatibilidad
            lblMonto.Text = lblTotal.Text;
            lblEfectivo.Text = lblECaja.Text;
            lblTarjetas.Text = lblCredito.Text;
            lblTransferencias.Text = lblTrans.Text;
            lblSalidas.Text = lblSalida.Text;

            // Bloque de Utilidades (tomadas directamente de histocortes)
            lblInversion.Text = $"{inversion:C}";
            lblBruta.Text = $"{utilidad - gastos:C}";
            lblGastos.Text = $"{gastos:C}";
            lblUtilidad.Text = $"{utilidad:C}";

            // Contadores en pestañas
            tabPageEfectivo.Text = $"Efectivo ({dgvCorte.Rows.Count})";
            tabPageEntradas.Text = $"Entradas ({dgvEntradas.Rows.Count})";
            tabPageSalidas.Text = $"Salidas ({dgvSalidas.Rows.Count})";
            tabPageTarjeta.Text = $"Tarjetas ({dataGridView3.Rows.Count})";
            tabPageTrans.Text = $"Transferencias ({dataGridView4.Rows.Count})";
            tabPageOtros.Text = $"Otros Pagos ({dgvOtrosPagos.Rows.Count})";
            tabPageAbonos.Text = $"Abonos ({dataGridView5.Rows.Count})";
            tabPageFolios.Text = $"Folios ({dgvFolios.Rows.Count})";

            // ESTILIZACIÓN DE GRIDS Y BOTONES
            EstilizarDataGridView(this.dgvFolios);
            EstilizarDataGridView(this.dgvCorte);
            EstilizarDataGridView(this.dgvEntradas);
            EstilizarDataGridView(this.dgvSalidas);
            EstilizarDataGridView(this.dgvOtrosPagos);
            EstilizarDataGridView(this.dataGridView5);
            EstilizarDataGridView(this.dataGridView2);
            EstilizarDataGridView(this.dataGridView3);
            EstilizarDataGridView(this.dataGridView4);
            EstilizarDataGridView(this.dataGridView6);
            EstilizarDataGridView(this.dgvCategoriasAgrupadas);

            EstilizarBotonPrimario(this.button1);
            EstilizarBotonPrimario(this.button2);
        }

        private void OcultarColumnasInternas(DataGridView dgv)
        {
            if (dgv.Columns.Contains("Id")) dgv.Columns["Id"].Visible = false;
            if (dgv.Columns.Contains("idCorte")) dgv.Columns["idCorte"].Visible = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
        }

        // ======================================================================
        // BOTÓN 1: REIMPRIMIR CORTE (CON SELECCIÓN DE TICKETS)
        // ======================================================================
        private void button1_Click(object sender, EventArgs e)
        {
            bool impResumen = false;
            bool detallado = false;
            bool impGrupos = false;
            bool impCategorias = false;
            bool impSalidas = false;

            using (frmOpcionesImpresionCorte frmOpciones = new frmOpcionesImpresionCorte(
                Sesion.TienePermiso("IMPRESION_CORTE"),
                dgvSalidas.Rows.Count > 0))
            {
                if (frmOpciones.ShowDialog() != DialogResult.OK)
                    return;

                impResumen = frmOpciones.ImprimirResumen;
                detallado = frmOpciones.ImprimirFoliosDetallados;
                impGrupos = frmOpciones.ImprimirGrupos;
                impCategorias = frmOpciones.ImprimirCategorias;
                impSalidas = frmOpciones.ImprimirSalidas;
            }

            if (!Conexion.Ticket)
            {
                MessageBox.Show("La impresión de tickets está desactivada en la configuración.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int totalArticulos = 0;
                foreach (DataGridViewRow row in dataGridView6.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (row.Cells["Categoria"].Value?.ToString() == "TOTAL GENERAL")
                    {
                        string piezasStr = row.Cells["Piezas"].Value?.ToString().Replace(",", "") ?? "0";
                        int.TryParse(piezasStr, out totalArticulos);
                        break;
                    }
                }

                string tituloTicket = EsCorteB
                    ? $"***** REIMPRESION CORTE B #{ID} *****"
                    : $"***** REIMPRESION CORTE #{ID} ******";

                string[] encabezados = new string[]
                {
                    tituloTicket,
                    "Inicio turno:", fechaInicioTurno,
                    "Cierre corte:", lblFecha.Text,
                    "TOTAL DE ARTICULOS: " + totalArticulos.ToString()
                };

                string GetNumericValue(string input) => Regex.Replace(input, @"[^\d.-]", "");

                // TICKET 1: Resumen Financiero
                if (impResumen)
                {
                    List<Producto> productos = new List<Producto>();
                    if (detallado)
                    {
                        foreach (DataGridViewRow row in dgvFolios.Rows)
                        {
                            if (row.IsNewRow) continue;
                            productos.Add(new Producto
                            {
                                Cantidad = 0,
                                Nombre = row.Cells["Folio"].Value?.ToString() ?? "",
                                PrecioUnitario = Convert.ToDouble(row.Cells["Monto sin IVA"].Value ?? 0),
                                Total = Convert.ToDouble(row.Cells["Monto"].Value ?? 0)
                            });
                        }
                    }

                    Dictionary<string, double> totales = new Dictionary<string, double>
                    {
                        { "Apertura Caja", Convert.ToDouble(GetNumericValue(lblApertura.Text)) },
                        { "Efectivo", Convert.ToDouble(GetNumericValue(lblCorte.Text)) },
                        { "Entradas", Convert.ToDouble(GetNumericValue(lblEntrada.Text)) },
                        { "Salidas", Convert.ToDouble(GetNumericValue(lblSalida.Text)) },
                        { "Cancelaciones", Convert.ToDouble(GetNumericValue(lblCancelaciones.Text)) },
                        { "Efectivo en Caja", Convert.ToDouble(GetNumericValue(lblECaja.Text)) },
                        { "Tarjetas", Convert.ToDouble(GetNumericValue(lblCredito.Text)) },
                        { "Comision", Convert.ToDouble(GetNumericValue(lbl5por.Text)) },
                        { "Transferencias", Convert.ToDouble(GetNumericValue(lblTrans.Text)) },
                        { "Otros Pagos", Convert.ToDouble(GetNumericValue(lblOtros.Text)) },
                        { "Sin IVA", Convert.ToDouble(GetNumericValue(lblNoIva.Text)) },
                        { "IVA", Convert.ToDouble(GetNumericValue(lblIVA.Text)) },
                        { "Total", Convert.ToDouble(GetNumericValue(lblTotal.Text)) }
                    };

                    TicketPrinter ticketResumen = new TicketPrinter(encabezados, Conexion.pieDeTicket, Conexion.logoPath, productos, "", "", "", total, true, totales);
                    ticketResumen.ImprimirTicket();
                }

                // TICKET 2A: Grupos de Categorías
                if (impGrupos)
                {
                    List<ItemCorteCategoria> listaGrupos = new List<ItemCorteCategoria>();
                    foreach (DataGridViewRow row in dgvCategoriasAgrupadas.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string grupo = row.Cells["Grupo"].Value?.ToString() ?? "";
                        double pzs = row.Cells["Piezas"].Value != null && row.Cells["Piezas"].Value != DBNull.Value
                                     ? Convert.ToDouble(row.Cells["Piezas"].Value) : 0;
                        double tot = row.Cells["Total"].Value != null && row.Cells["Total"].Value != DBNull.Value
                                     ? Convert.ToDouble(row.Cells["Total"].Value) : 0;

                        listaGrupos.Add(new ItemCorteCategoria { Nombre = grupo, Piezas = pzs, Total = tot });
                    }

                    TicketPrinter ticketGrupos = new TicketPrinter(new List<Producto>(), "", "");
                    ticketGrupos.ImprimirCategoriasCorte($"REIMP. GRUPOS CORTE #{ID}", fechaInicioTurno, listaGrupos);
                }

                // TICKET 2B: Categorías Individuales
                if (impCategorias)
                {
                    List<ItemCorteCategoria> listaCategorias = new List<ItemCorteCategoria>();
                    foreach (DataGridViewRow row in dataGridView6.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string cat = row.Cells["Categoria"].Value?.ToString() ?? "";
                        double pzs = row.Cells["Piezas"].Value != null && row.Cells["Piezas"].Value != DBNull.Value
                                     ? Convert.ToDouble(row.Cells["Piezas"].Value) : 0;
                        double tot = row.Cells["Total"].Value != null && row.Cells["Total"].Value != DBNull.Value
                                     ? Convert.ToDouble(row.Cells["Total"].Value) : 0;

                        listaCategorias.Add(new ItemCorteCategoria { Nombre = cat, Piezas = pzs, Total = tot });
                    }

                    TicketPrinter ticketCats = new TicketPrinter(new List<Producto>(), "", "");
                    ticketCats.ImprimirCategoriasCorte($"REIMP. CATEGORIAS CORTE #{ID}", fechaInicioTurno, listaCategorias);
                }

                // TICKET 3: Salidas Detalladas
                if (impSalidas)
                {
                    List<ItemCorteSalida> listaSalidas = new List<ItemCorteSalida>();
                    double totalTicketSalidas = 0;
                    foreach (DataGridViewRow row in dgvSalidas.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string concepto = row.Cells[1].Value?.ToString() ?? "";
                        double monto = row.Cells[2].Value != null && row.Cells[2].Value != DBNull.Value
                                       ? Math.Abs(Convert.ToDouble(row.Cells[2].Value)) : 0;

                        totalTicketSalidas += monto;
                        listaSalidas.Add(new ItemCorteSalida { Concepto = concepto, Monto = monto });
                    }

                    TicketPrinter ticketSalidas = new TicketPrinter(new List<Producto>(), "", "");
                    ticketSalidas.ImprimirSalidasCorte(fechaInicioTurno, listaSalidas, totalTicketSalidas);
                }

                MessageBox.Show("Reimpresión completada con éxito.", "Impresión", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir el ticket: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ======================================================================
        // BOTÓN 2: EXPORTAR A EXCEL (TODOS LOS MOVIMIENTOS DEL CORTE)
        // ======================================================================
        private void button2_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;

            xla.Visible = true;

            ws.Cells[1, 1] = "Sección";
            ws.Cells[1, 2] = "Concepto";
            ws.Cells[1, 3] = "Monto Total";
            ws.Cells[1, 4] = "Fecha/Hora";
            ws.Cells[1, 5] = "Tipo Pago";
            ws.Cells[1, 6] = (EsCorteB ? $"Corte B #{ID}: " : $"Corte #{ID}: ") + lblFecha.Text;

            int fila = 2;
            void AgregarGridAExcel(string seccion, DataGridView dgv)
            {
                for (int i = 0; i < dgv.RowCount; i++)
                {
                    if (dgv.Rows[i].IsNewRow) continue;
                    ws.Cells[fila, 1] = seccion;
                    ws.Cells[fila, 2] = dgv[1, i].Value?.ToString() ?? "";
                    ws.Cells[fila, 3] = dgv[2, i].Value?.ToString() ?? "0";
                    ws.Cells[fila, 4] = dgv.Columns.Count > 3 ? (dgv[3, i].Value?.ToString() ?? "") : "";
                    ws.Cells[fila, 5] = dgv.Columns.Count > 5 ? (dgv[5, i].Value?.ToString() ?? "") : "";
                    fila++;
                }
            }

            AgregarGridAExcel("Efectivo", dgvCorte);
            AgregarGridAExcel("Entradas", dgvEntradas);
            AgregarGridAExcel("Salidas", dgvSalidas);
            AgregarGridAExcel("Tarjetas", dataGridView3);
            AgregarGridAExcel("Transferencias", dataGridView4);
            AgregarGridAExcel("Otros Pagos", dgvOtrosPagos);
            AgregarGridAExcel("Abonos", dataGridView5);
        }
    }
}