using LibPrintTicket;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmCorte : frmBase
    {
        double apertura = 0;      // Fondo inicial de caja
        double mas = 0;           // Únicamente ventas en efectivo
        double entradas = 0;      // "Entrada en caja por concepto..."
        double salidas = 0;       // "Salida en caja por concepto..."
        double cancelaciones = 0; // Movimientos con "Cancelacion"
        double tarjeta = 0;
        double trans = 0;
        double otros = 0;         // Nueva variable para los demás métodos de pago
        public string fechaApertura = "";
        public string usuario = "";
        private DataSet ds;
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL);
        SqlDataAdapter da;
        SqlCommand cmd;
        double total = 0, utilidad = 0, inversion = 0, gastos = 0;
        int folio;
        private decimal porcentaje;

        public frmCorte()
        {
            InitializeComponent();
            this.MinimumSize = new Size(969, 772);
        }

        private void dgvCorte_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!EsCeldaValida(dgvCorte, e)) return;
            if (dgvCorte.Columns[e.ColumnIndex].Name == "Monto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void dgvEntradas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!EsCeldaValida(dgvEntradas, e)) return;
            if (dgvEntradas.Columns[e.ColumnIndex].Name == "Monto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void dgvSalidas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!EsCeldaValida(dgvSalidas, e)) return;
            if (dgvSalidas.Columns[e.ColumnIndex].Name == "Monto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void dataGridView4_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!EsCeldaValida(dataGridView4, e)) return;
            if (dataGridView4.Columns[e.ColumnIndex].Name == "Monto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!EsCeldaValida(dataGridView3, e)) return;
            if (dataGridView3.Columns[e.ColumnIndex].Name == "Monto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }

        // Evento de formato para la nueva tabla de Otros Pagos
        private void dgvOtrosPagos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!EsCeldaValida(dgvOtrosPagos, e)) return;
            if (dgvOtrosPagos.Columns[e.ColumnIndex].Name == "Monto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void dgvFolios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!EsCeldaValida(dgvFolios, e)) return;
            if (dgvFolios.Columns[e.ColumnIndex].Name == "Monto" || dgvFolios.Columns[e.ColumnIndex].Name == "Descuento" || dgvFolios.Columns[e.ColumnIndex].Name == "Monto sin IVA" || dgvFolios.Columns[e.ColumnIndex].Name == "IVA")
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
                dataGridView6.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(dataGridView6.Font, FontStyle.Bold);
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
                dgvCategoriasAgrupadas.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(dgvCategoriasAgrupadas.Font, FontStyle.Bold);
                dgvCategoriasAgrupadas.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                dgvCategoriasAgrupadas.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
        }

        private void frmCorte_Load(object sender, EventArgs e)
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
            conectar.Open();

            cmd = new SqlCommand("select Porcentaje from Tarjeta where Id=1;", conectar);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                porcentaje = Convert.ToDecimal(reader[0].ToString());
            }
            reader.Close();
            lblComision.Text = $"{porcentaje:F2}% Comisión:";

            cmd = new SqlCommand("select * from Fech where Id=1;", conectar);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int caja = Convert.ToInt32(Convert.ToString(reader[1].ToString()));
                fechaApertura = Convert.ToString(reader[2].ToString());
            }
            reader.Close();

            // 1. EFECTIVO (Solo ventas y cancelaciones, sin Apertura, Entradas ni Salidas)
            ds = new DataSet();
            da = new SqlDataAdapter(@"
            SELECT * FROM Corte 
            WHERE (Pago='01=EFECTIVO' OR Pago ='Efectivo') 
              AND Concepto NOT LIKE 'Abono%' 
              AND Concepto <> 'APERTURA DE CAJA'
              AND Concepto NOT LIKE 'Entrada en caja por concepto%' 
              AND Concepto NOT LIKE 'Salida en caja por concepto%';", conectar);
            da.Fill(ds, "Id");
            dgvCorte.DataSource = ds.Tables["Id"];
            dgvCorte.Columns[0].Visible = false;

            // 1.1 ENTRADAS (Incluye APERTURA DE CAJA y Entradas manuales)
            ds = new DataSet();
            da = new SqlDataAdapter(@"
            SELECT * FROM Corte 
            WHERE Concepto LIKE 'Entrada en caja por concepto%' 
               OR Concepto = 'APERTURA DE CAJA';", conectar);
            da.Fill(ds, "Entradas");
            dgvEntradas.DataSource = ds.Tables["Entradas"];
            dgvEntradas.Columns[0].Visible = false;

            // 1.2 SALIDAS
            ds = new DataSet();
            da = new SqlDataAdapter(@"
                SELECT * FROM Corte 
                WHERE Concepto LIKE 'Salida en caja por concepto%';", conectar);
            da.Fill(ds, "Salidas");
            dgvSalidas.DataSource = ds.Tables["Salidas"];
            dgvSalidas.Columns[0].Visible = false;

            // 2. TARJETAS
            ds = new DataSet();
            da = new SqlDataAdapter("select * from Corte where (Pago='04=TARJETA DE CREDITO' or Pago='28=TARJETA DE DEBITO' or Pago='Tarjeta') AND Concepto NOT LIKE 'Abono%';", conectar);
            da.Fill(ds, "Id");
            dataGridView3.DataSource = ds.Tables["Id"];
            dataGridView3.Columns[0].Visible = false;

            // 3. TRANSFERENCIAS
            ds = new DataSet();
            da = new SqlDataAdapter("select * from Corte where (Pago='03=TRANFERENCIA ELECTRONICA DE FONDOS' OR Pago = 'Transferencia') AND Concepto NOT LIKE 'Abono%';", conectar);
            da.Fill(ds, "Id");
            dataGridView4.DataSource = ds.Tables["Id"];
            dataGridView4.Columns[0].Visible = false;

            // 4. OTROS PAGOS
            ds = new DataSet();
            da = new SqlDataAdapter("select * from Corte where Pago NOT IN ('01=EFECTIVO','Transferencia', 'Tarjeta','Efectivo','04=TARJETA DE CREDITO', '28=TARJETA DE DEBITO', '03=TRANFERENCIA ELECTRONICA DE FONDOS') AND Concepto NOT LIKE 'Abono%' AND Concepto NOT LIKE 'Entrada en caja por concepto%' AND Concepto NOT LIKE 'Salida en caja por concepto%';", conectar);
            da.Fill(ds, "Id");
            dgvOtrosPagos.DataSource = ds.Tables["Id"];
            dgvOtrosPagos.Columns[0].Visible = false;

            // 5. ABONOS
            ds = new DataSet();
            da = new SqlDataAdapter("select * from Corte where Concepto LIKE 'Abono%';", conectar);
            da.Fill(ds, "Id");
            dataGridView5.DataSource = ds.Tables["Id"];
            dataGridView5.Columns[0].Visible = false;

            DateTime fechaDate;
            if (!DateTime.TryParse(this.fechaApertura, out fechaDate))
            {
                fechaDate = DateTime.Now.Date;
            }
            string fechaInicioSQL = fechaDate.ToString("yyyy-MM-dd HH:mm:ss");
            string fechaFinSQL = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 6. POR CATEGORÍA INDIVIDUAL
            ds = new DataSet();
            string query = "SELECT IIf(Categoria Is Null OR Categoria = '', 'SIN CATEGORIA', Categoria) as Categoria, " +
                  "SUM(Cantidad) as Piezas, " +
                  "SUM(MontoTotal) as Total " +
                  "FROM VentasContado " +
                  "WHERE Fecha >= '" + fechaInicioSQL + "' and Fecha <= '" + fechaFinSQL + "' " +
                  "AND FolioVenta IN (SELECT Folio FROM Ventas WHERE Estatus = 'COBRADO') " +
                  "GROUP BY IIf(Categoria Is Null OR Categoria = '', 'SIN CATEGORIA', Categoria) " +
                  "UNION ALL " +
                  "SELECT 'TOTAL GENERAL' as Categoria, " +
                  "SUM(Cantidad) as Piezas, " +
                  "SUM(MontoTotal) as Total " +
                  "FROM VentasContado " +
                  "WHERE Fecha >= '" + fechaInicioSQL + "' and Fecha <= '" + fechaFinSQL + "' " +
                  "AND FolioVenta IN (SELECT Folio FROM Ventas WHERE Estatus = 'COBRADO')";

            da = new SqlDataAdapter(query, conectar);
            da.Fill(ds, "Id");
            dataGridView6.DataSource = ds.Tables["Id"];

            // 7. CATEGORÍAS AGRUPADAS (Filtrado por ventas.Fecha)
            ds = new DataSet();
            string queryAgrupadas = @"
                SELECT Grupo, Piezas, Total
                FROM (
                    SELECT 
                        ISNULL(g.Nombre, 'Sin Grupo') AS Grupo,
                        SUM(vc.Cantidad) AS Piezas,
                        SUM(vc.MontoTotal) AS Total,
                        0 AS Orden
                    FROM VentasContado vc
                    INNER JOIN ventas v ON v.Folio = vc.FolioVenta
                    INNER JOIN INVENTARIO i ON vc.IdProducto = i.Id
                    LEFT JOIN Categorias c ON c.Nombre = i.Categoria
                    LEFT JOIN GruposCategoriasDetalle d ON c.Id = d.IdCategoria
                    LEFT JOIN GruposCategorias g ON d.IdGrupo = g.IdGrupo AND g.Activo = 1
                    WHERE v.Estatus = 'Cobrado'
                      AND v.Fecha >= '" + fechaInicioSQL + @"' 
                      AND v.Fecha <= '" + fechaFinSQL + @"'
                    GROUP BY ISNULL(g.Nombre, 'Sin Grupo')

                    UNION ALL

                    SELECT 
                        'TOTAL GENERAL' AS Grupo,
                        ISNULL(SUM(vc.Cantidad), 0) AS Piezas,
                        ISNULL(SUM(vc.MontoTotal), 0) AS Total,
                        1 AS Orden
                    FROM VentasContado vc
                    INNER JOIN ventas v ON v.Folio = vc.FolioVenta
                    INNER JOIN INVENTARIO i ON vc.IdProducto = i.Id
                    LEFT JOIN Categorias c ON c.Nombre = i.Categoria
                    LEFT JOIN GruposCategoriasDetalle d ON c.Id = d.IdCategoria
                    LEFT JOIN GruposCategorias g ON d.IdGrupo = g.IdGrupo AND g.Activo = 1
                    WHERE v.Estatus = 'Cobrado'
                      AND v.Fecha >= '" + fechaInicioSQL + @"' 
                      AND v.Fecha <= '" + fechaFinSQL + @"'
                ) AS Resumen
                ORDER BY Orden ASC, Total DESC;";

            da = new SqlDataAdapter(queryAgrupadas, conectar);
            da.Fill(ds, "Agrupadas");
            dgvCategoriasAgrupadas.DataSource = ds.Tables["Agrupadas"];

            // ======================================================================
            // CÁLCULOS DE MONTOS
            // ======================================================================
            // Efectivo (Cancelaciones y Ventas en Efectivo)
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

            // Entradas (Separando Apertura de Caja vs Entradas de efectivo)
            for (int i = 0; i < dgvEntradas.RowCount; i++)
            {
                if (dgvEntradas.Rows[i].IsNewRow) continue;
                if (dgvEntradas[2, i].Value != null && dgvEntradas[2, i].Value != DBNull.Value)
                {
                    double valorNumerico = Convert.ToDouble(dgvEntradas[2, i].Value);
                    string concepto = dgvEntradas[1, i].Value?.ToString().Trim() ?? "";

                    if (concepto.Equals("APERTURA DE CAJA", StringComparison.OrdinalIgnoreCase))
                    {
                        apertura += valorNumerico;
                    }
                    else
                    {
                        entradas += Math.Abs(valorNumerico);
                    }
                }
            }
            // Salidas (Desde dgvSalidas)
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
                {
                    tarjeta += Convert.ToSingle(dataGridView3[2, i].Value);
                }
            }

            // Transferencias
            for (int i = 0; i < dataGridView4.RowCount; i++)
            {
                if (dataGridView4.Rows[i].IsNewRow) continue;
                if (dataGridView4[2, i].Value != null && dataGridView4[2, i].Value != DBNull.Value)
                {
                    trans += Convert.ToSingle(dataGridView4[2, i].Value);
                }
            }

            // Otros Pagos
            for (int i = 0; i < dgvOtrosPagos.RowCount; i++)
            {
                if (dgvOtrosPagos.Rows[i].IsNewRow) continue;
                if (dgvOtrosPagos[2, i].Value != null && dgvOtrosPagos[2, i].Value != DBNull.Value)
                {
                    otros += Convert.ToSingle(dgvOtrosPagos[2, i].Value);
                }
            }

            // Abonos
            for (int i = 0; i < dataGridView5.RowCount; i++)
            {
                if (dataGridView5.Rows[i].IsNewRow) continue;

                if (dataGridView5[2, i].Value != null && dataGridView5[2, i].Value != DBNull.Value)
                {
                    double valorNumerico = Convert.ToDouble(dataGridView5[2, i].Value);
                    string tipoPago = "";

                    if (dataGridView5[4, i].Value != null && dataGridView5[4, i].Value != DBNull.Value)
                    {
                        tipoPago = dataGridView5[4, i].Value.ToString().ToUpper();
                    }

                    if (tipoPago.Contains("EFECTIVO") || tipoPago.Contains("01="))
                    {
                        if (valorNumerico < 0) salidas += Math.Abs(valorNumerico);
                        else if (valorNumerico > 0) mas += Convert.ToSingle(valorNumerico);
                    }
                    else if (tipoPago.Contains("TARJETA") || tipoPago.Contains("04=") || tipoPago.Contains("28="))
                    {
                        tarjeta += Convert.ToSingle(valorNumerico);
                    }
                    else if (tipoPago.Contains("TRANFERENCIA") || tipoPago.Contains("TRANSFERENCIA") || tipoPago.Contains("03="))
                    {
                        trans += Convert.ToSingle(valorNumerico);
                    }
                    else
                    {
                        otros += Convert.ToSingle(valorNumerico);
                    }
                }
            }

            using (SqlCommand cmdMax = new SqlCommand("SELECT MAX(Id) FROM histocortes", conectar))
            {
                object result = cmdMax.ExecuteScalar();
                folio = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
            }

            ds = new DataSet();
            da = new SqlDataAdapter("select MontoTotal,Utilidad,Id from VentasContado Where Fecha>= '" + fechaInicioSQL + "' and Fecha <= '" + fechaFinSQL + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];

            // Utilidades
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].IsNewRow) continue;
                if (dataGridView1[1, i].Value != null && dataGridView1[1, i].Value != DBNull.Value &&
                    dataGridView1[0, i].Value != null && dataGridView1[0, i].Value != DBNull.Value)
                {
                    double valorUtilidad = Convert.ToDouble(dataGridView1[1, i].Value);
                    double valorMonto = Convert.ToDouble(dataGridView1[0, i].Value);

                    if (valorUtilidad > 0)
                    {
                        total += valorMonto;
                        utilidad += valorUtilidad;
                    }
                }
            }

            ds = new DataSet();
            da = new SqlDataAdapter("select * from GastosDetallados Where Fecha>= '" + fechaInicioSQL + "' and Fecha <= '" + fechaFinSQL + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];

            // Gastos
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                if (dataGridView2.Rows[i].IsNewRow) continue;
                if (dataGridView2[2, i].Value != null && dataGridView2[2, i].Value != DBNull.Value)
                {
                    gastos += Convert.ToDouble(dataGridView2[2, i].Value);
                }
            }

            ds = new DataSet();
            da = new SqlDataAdapter("Select Id,Folio, CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)) / (1 + (16.0 / 100)) AS [Monto sin IVA], CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)) - (CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)) / (1 + (16.0 / 100))) AS [IVA], Monto,Descuento, Fecha, Estatus, Pago from Ventas where Estatus ='COBRADO' AND Fecha>= '" + fechaInicioSQL + "' and Fecha <= '" + fechaFinSQL + "';", conectar);
            da.Fill(ds, "Id");
            dgvFolios.DataSource = ds.Tables["Id"];
            dgvFolios.Columns[0].Visible = false;

            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.Name = "Seleccionar";
            checkColumn.HeaderText = "Seleccionar";
            checkColumn.Width = 80;
            checkColumn.ReadOnly = false;
            dgvFolios.Columns.Insert(0, checkColumn);

            foreach (DataGridViewRow row in dgvFolios.Rows)
            {
                row.Cells["Seleccionar"].Value = true;
            }

            foreach (DataGridViewColumn column in dgvFolios.Columns)
            {
                if (column.Name != "Seleccionar")
                {
                    column.ReadOnly = true;
                }
            }

            // ======================================================================
            // ASIGNACIÓN DE TEXTOS A LAS ETIQUETAS
            // ======================================================================

            // --- Sección de control de efectivo físico ---
            lblApertura.Text = $"{apertura:C}";
            lblCorte.Text = $"{mas:C}";               // Ventas en efectivo
            lblEntrada.Text = $"{entradas:C}";        // "Entrada en caja por concepto..."
            lblSalida.Text = $"{salidas:C}";          // "Salida en caja por concepto..."
            lblCancelaciones.Text = $"{cancelaciones:C}"; // "Cancelacion"

            double efectivoEnCaja = apertura + mas + entradas - salidas - cancelaciones;
            lblECaja.Text = $"{efectivoEnCaja:C}";

            // --- Desglose de métodos de pago ---
            decimal factorPorcentaje = porcentaje / 100m;
            decimal factorRestante = 1m - factorPorcentaje;

            decimal tarjetaSinComision = (decimal)tarjeta * factorRestante;
            decimal comisionTarjeta = (decimal)tarjeta * factorPorcentaje;

            lblCredito.Text = $"{tarjetaSinComision:C2}";
            lbl5por.Text = $"{comisionTarjeta:C2}";
            lblTrans.Text = $"{trans:C}";
            lblOtros.Text = $"{otros:C}";

            // --- FÓRMULAS SOLICITADAS ---
            decimal tn = Convert.ToDecimal(mas) + tarjetaSinComision + comisionTarjeta + Convert.ToDecimal(trans) + Convert.ToDecimal(otros);
            decimal subtotal = tn / 1.16m;
            decimal iva = tn * 0.16m;

            lblNoIva.Text = $"{subtotal:C2}";
            lblIVA.Text = $"{iva:C2}";
            lblTotal.Text = $"{tn:C2}";

            // --- Bloque de Utilidades ---
            inversion = total - utilidad;
            lblInversion.Text = $"{inversion:C}";
            lblBruta.Text = $"{utilidad - gastos:C}";
            lblGastos.Text = $"{gastos:C}";
            lblUtilidad.Text = $"{utilidad:C}";

            // ESTILIZACIÓN DE GRIDS
            EstilizarDataGridView(this.dgvFolios);
            EstilizarDataGridView(this.dgvCorte);
            EstilizarDataGridView(this.dgvEntradas);
            EstilizarDataGridView(this.dgvSalidas);
            EstilizarDataGridView(this.dgvOtrosPagos);
            EstilizarDataGridView(this.dataGridView5);
            EstilizarDataGridView(this.dataGridView1);
            EstilizarDataGridView(this.dataGridView2);
            EstilizarDataGridView(this.dataGridView3);
            EstilizarDataGridView(this.dataGridView4);
            EstilizarDataGridView(this.dataGridView6);
            EstilizarDataGridView(this.dgvCategoriasAgrupadas);

            EstilizarBotonPrimario(this.button1);
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool tieneMovimientos = false;

            // Revisar si hay ventas o cancelaciones en Efectivo
            foreach (DataGridViewRow row in dgvCorte.Rows)
            {
                if (row.IsNewRow) continue;
                tieneMovimientos = true;
                break;
            }

            // Revisar si hay entradas reales (distintas a APERTURA DE CAJA)
            if (!tieneMovimientos)
            {
                foreach (DataGridViewRow row in dgvEntradas.Rows)
                {
                    if (row.IsNewRow) continue;
                    string concepto = row.Cells[1].Value?.ToString().Trim() ?? "";
                    if (!concepto.Equals("APERTURA DE CAJA", StringComparison.OrdinalIgnoreCase))
                    {
                        tieneMovimientos = true;
                        break;
                    }
                }
            }

            // Revisar Salidas, Tarjetas, Transferencias, Otros o Abonos
            if (!tieneMovimientos && (dgvSalidas.Rows.Count > 0 || dataGridView3.Rows.Count > 0 || dataGridView4.Rows.Count > 0 || dgvOtrosPagos.Rows.Count > 0 || dataGridView5.Rows.Count > 0))
            {
                tieneMovimientos = true;
            }

            if (!tieneMovimientos)
            {
                MessageBox.Show("No hay artículos vendidos ni movimientos en este corte. No se puede realizar el corte de caja.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Mostrar modal de opciones de impresión antes de procesar el cierre
            bool deseaImprimir = false;
            bool impResumen = false;
            bool detallado = false;
            bool impGrupos = false;
            bool impCategorias = false;
            bool impSalidas = false;

            using (frmOpcionesImpresionCorte frmOpciones = new frmOpcionesImpresionCorte(
                Sesion.TienePermiso("IMPRESION_CORTE"),
                dgvSalidas.Rows.Count > 0))
            {
                if (frmOpciones.ShowDialog() == DialogResult.OK)
                {
                    deseaImprimir = true;
                    impResumen = frmOpciones.ImprimirResumen;
                    detallado = frmOpciones.ImprimirFoliosDetallados;
                    impGrupos = frmOpciones.ImprimirGrupos;
                    impCategorias = frmOpciones.ImprimirCategorias;
                    impSalidas = frmOpciones.ImprimirSalidas;
                }
            }

            #region folios nuevos
            int foli = 0;
            List<Producto> productos = new List<Producto>();
            cmd = new SqlCommand("select Numero from Folios where Folio='FolioVenta';", conectar);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                foli = Convert.ToInt32(Convert.ToString(reader[0].ToString()));
            }
            reader.Close();

            double masB = 0, tarjetaB = 0, transB = 0, otrosB = 0, totalB = 0, utilidadB = 0;
            string primerFolio = "";
            string ultimoFolio = "";
            foreach (DataGridViewRow row in dgvFolios.Rows)
            {
                if (detallado)
                    productos.Add(new Producto
                    {
                        Cantidad = 0,
                        Nombre = row.Cells["Folio"].Value.ToString(),
                        PrecioUnitario = Convert.ToDouble(row.Cells["Monto sin IVA"].Value.ToString()),
                        Total = Convert.ToDouble(row.Cells["Monto"].Value.ToString()),
                    });
                string folioVenta = row.Cells["Folio"].Value.ToString();

                if (row.Cells["Seleccionar"].Value != null && Convert.ToBoolean(row.Cells["Seleccionar"].Value))
                {
                    string montoLimpio = Regex.Replace(row.Cells["Monto"].Value.ToString(), @"[^\d.-]", "");
                    double montoVenta = Convert.ToDouble(montoLimpio);
                    string tipoPago = row.Cells["Pago"].Value.ToString().ToUpper();

                    totalB += montoVenta;

                    if (tipoPago.Contains("EFECTIVO") || tipoPago.Contains("01=")) masB += montoVenta;
                    else if (tipoPago.Contains("TARJETA") || tipoPago.Contains("04=") || tipoPago.Contains("28=")) tarjetaB += montoVenta;
                    else if (tipoPago.Contains("TRANFERENCIA") || tipoPago.Contains("03=")) transB += montoVenta;
                    else otrosB += montoVenta;

                    double utVenta = 0;
                    using (SqlCommand cmdUt = new SqlCommand("SELECT SUM(Utilidad) FROM VentasContado WHERE FolioVenta='" + folioVenta + "'", conectar))
                    {
                        object res = cmdUt.ExecuteScalar();
                        if (res != DBNull.Value && res != null) utVenta = Convert.ToDouble(res);
                    }
                    utilidadB += utVenta;
                    if (string.IsNullOrEmpty(primerFolio)) primerFolio = foli.ToString("D6");
                    ultimoFolio = foli.ToString("D6");
                    using (SqlCommand cmdCB = new SqlCommand("INSERT INTO CortesB (Concepto, Monto, idCorte, Tipo) VALUES ('Venta a Contado Folio " + foli.ToString("D6") + "', '" + montoVenta + "', '" + folio + "', '" + tipoPago + "')", conectar))
                    {
                        cmdCB.ExecuteNonQuery();
                    }
                    string queryFacturacion = "INSERT INTO VentasB (Monto, Fecha, FolioA, Estatus, Descuento, Pago, FolioB) VALUES (" +
                        "'" + row.Cells["Monto"].Value.ToString() + "', '" + row.Cells["Fecha"].Value.ToString() + "', '" + row.Cells["Folio"].Value.ToString() + "', " +
                        "'" + row.Cells["Estatus"].Value.ToString() + "', " +
                        "'" + row.Cells["Descuento"].Value.ToString() + "', '" + row.Cells["Pago"].Value.ToString() + "', '" + foli + "');";
                    using (SqlCommand cmdFactura = new SqlCommand(queryFacturacion, conectar))
                    {
                        cmdFactura.ExecuteNonQuery();
                    }
                    foli++;
                }
            }
            cmd = new SqlCommand("UPDATE Folios set Numero=" + foli + " where Folio='FolioVenta';", conectar);
            cmd.ExecuteNonQuery();
            if (totalB > 0)
            {
                double inversionB = totalB - utilidadB;
                string fechaCorteB = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string rangoFolios = primerFolio + "-" + ultimoFolio;
                string queryHistoB = "INSERT INTO histocortesB (Id, Monto, Fecha, Mas, Menos, Tarjeta, utilidad, inversion, CubreFolio) VALUES ('" + folio + "', '" + totalB + "', '" + fechaCorteB + "', '" + masB + "', '0', '" + tarjetaB + "', '" + utilidadB + "', '" + inversionB + "','" + rangoFolios + "');";
                using (SqlCommand cmdHistoB = new SqlCommand(queryHistoB, conectar))
                {
                    cmdHistoB.ExecuteNonQuery();
                }
            }
            #endregion 

            button1.Visible = false;
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
            string[] encabezados = new string[] { "********** CORTE DE CAJA  ********",
                                                "Apertura de caja:", fechaApertura,
                                                "Corte de caja:", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                                                 "TOTAL DE ARTICULOS: "+totalArticulos.ToString()};

            // Guardado en historial Cortes
            for (int i = 0; i < dgvCorte.RowCount; i++)
            {
                if (dgvCorte.Rows[i].IsNewRow) continue;
                cmd = new SqlCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dgvCorte[1, i].Value.ToString() + "','" + dgvCorte[2, i].Value.ToString() + "','" + folio + "','" + dgvCorte[4, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            for (int i = 0; i < dgvEntradas.RowCount; i++)
            {
                if (dgvEntradas.Rows[i].IsNewRow) continue;
                cmd = new SqlCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dgvEntradas[1, i].Value.ToString() + "','" + dgvEntradas[2, i].Value.ToString() + "','" + folio + "','" + dgvEntradas[4, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            for (int i = 0; i < dgvSalidas.RowCount; i++)
            {
                if (dgvSalidas.Rows[i].IsNewRow) continue;
                cmd = new SqlCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dgvSalidas[1, i].Value.ToString() + "','" + dgvSalidas[2, i].Value.ToString() + "','" + folio + "','" + dgvSalidas[4, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                if (dataGridView3.Rows[i].IsNewRow) continue;
                cmd = new SqlCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dataGridView3[1, i].Value.ToString() + "','" + dataGridView3[2, i].Value.ToString() + "','" + folio + "','" + dataGridView3[4, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            for (int i = 0; i < dataGridView4.RowCount; i++)
            {
                if (dataGridView4.Rows[i].IsNewRow) continue;
                cmd = new SqlCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dataGridView4[1, i].Value.ToString() + "','" + dataGridView4[2, i].Value.ToString() + "','" + folio + "','" + dataGridView4[4, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            for (int i = 0; i < dgvOtrosPagos.RowCount; i++)
            {
                if (dgvOtrosPagos.Rows[i].IsNewRow) continue;
                cmd = new SqlCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dgvOtrosPagos[1, i].Value.ToString() + "','" + dgvOtrosPagos[2, i].Value.ToString() + "','" + folio + "','" + dgvOtrosPagos[4, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }

            double totalSalidasHistorico = -(salidas + cancelaciones);
            cmd = new SqlCommand("INSERT INTO histocortes(Id,Monto,Fecha,Mas,Menos,Tarjeta,utilidad,inversion) VALUES ('" + folio + "','" + lblTotal.Text + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + (apertura + mas + entradas) + "','" + totalSalidasHistorico + "','" + tarjeta + "','" + utilidad + "','" + inversion + "');", conectar);
            cmd.ExecuteNonQuery();
            folio++;
            cmd = new SqlCommand("UPDATE Folios set Numero=" + folio + " where Folio='Corte';", conectar);
            cmd.ExecuteNonQuery();
            cmd = new SqlCommand("UPDATE Folios set Numero=0 where Folio='Inicio';", conectar);
            cmd.ExecuteNonQuery();
            cmd = new SqlCommand("truncate table Corte;", conectar);
            cmd.ExecuteNonQuery();
            cmd = new SqlCommand("UPDATE Fech set Caja='0' where Id=1;", conectar);
            cmd.ExecuteNonQuery();
            cmd = new SqlCommand("truncate table Credito;", conectar);
            cmd.ExecuteNonQuery();

            string GetNumericValue(string input)
            {
                return Regex.Replace(input, @"[^\d.-]", "");
            }

            // 2. Ejecución de impresiones seleccionadas (cada una sale en ticket separado)
            if (deseaImprimir && Conexion.Ticket)
            {
                // TICKET 1: Resumen Financiero
                if (impResumen)
                {
                    Dictionary<string, double> totales = new Dictionary<string, double>();
                    totales.Add("Apertura Caja", Convert.ToDouble(GetNumericValue(lblApertura.Text)));
                    totales.Add("Efectivo", Convert.ToDouble(GetNumericValue(lblCorte.Text)));
                    totales.Add("Entradas", Convert.ToDouble(GetNumericValue(lblEntrada.Text)));
                    totales.Add("Salidas", Convert.ToDouble(GetNumericValue(lblSalida.Text)));
                    totales.Add("Cancelaciones", Convert.ToDouble(GetNumericValue(lblCancelaciones.Text)));
                    totales.Add("Efectivo en Caja", Convert.ToDouble(GetNumericValue(lblECaja.Text)));
                    totales.Add("Tarjetas", Convert.ToDouble(GetNumericValue(lblCredito.Text)));
                    totales.Add("Comision", Convert.ToDouble(GetNumericValue(lbl5por.Text)));
                    totales.Add("Transferencias", Convert.ToDouble(GetNumericValue(lblTrans.Text)));
                    totales.Add("Otros Pagos", Convert.ToDouble(GetNumericValue(lblOtros.Text)));
                    totales.Add("Sin IVA", Convert.ToDouble(GetNumericValue(lblNoIva.Text)));
                    totales.Add("IVA", Convert.ToDouble(GetNumericValue(lblIVA.Text)));
                    totales.Add("Total", Convert.ToDouble(GetNumericValue(lblTotal.Text)));

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
                    ticketGrupos.ImprimirCategoriasCorte("POR GRUPOS DE CATEGORÍA", fechaApertura, listaGrupos);
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
                    ticketCats.ImprimirCategoriasCorte("POR CATEGORÍA", fechaApertura, listaCategorias);
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
                    ticketSalidas.ImprimirSalidasCorte(fechaApertura, listaSalidas, totalTicketSalidas);
                }
            }

            MessageBox.Show("CORTE REALIZADO CON EXITO", "EXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            foreach (Form form in Application.OpenForms)
            {
                if (form is frmPrincipal principal)
                {
                    principal.DetenerServidorWeb();
                    break;
                }
            }

            Application.Exit();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            folio++;
            Ticket ticket = new Ticket();
            ticket.MaxChar = Conexion.MaxChar;
            ticket.FontSize = Conexion.FontSize;
            ticket.MaxCharDescription = Conexion.MaxCharDescription;
            if (Conexion.Font != "")
            {
                ticket.FontName = Conexion.Font;
            }
            ticket.AddHeaderLine("******** CORTE PARCIAL  *******");
            ticket.AddSubHeaderLine("FECHA Y HORA:");
            ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            for (int i = 0; i < dgvCorte.RowCount; i++)
            {
                if (dgvCorte.Rows[i].IsNewRow) continue;
                ticket.AddItem("1", dgvCorte[1, i].Value.ToString(), "   $" + dgvCorte[2, i].Value.ToString());
            }
            for (int i = 0; i < dgvEntradas.RowCount; i++)
            {
                if (dgvEntradas.Rows[i].IsNewRow) continue;
                ticket.AddItem("1", dgvEntradas[1, i].Value.ToString(), "   $" + dgvEntradas[2, i].Value.ToString());
            }
            for (int i = 0; i < dgvSalidas.RowCount; i++)
            {
                if (dgvSalidas.Rows[i].IsNewRow) continue;
                ticket.AddItem("1", dgvSalidas[1, i].Value.ToString(), "   $" + dgvSalidas[2, i].Value.ToString());
            }
            ticket.AddTotal("Efectivo", lblCorte.Text);
            ticket.AddTotal("Tarjetas", lblCredito.Text);
            ticket.AddTotal("Entradas", lblEntrada.Text);
            ticket.AddTotal("Salidas", lblSalida.Text);
            ticket.AddTotal("Cancelaciones", lblCancelaciones.Text);
            ticket.AddTotal("Total", lblTotal.Text);
            MessageBox.Show("CORTE REALIZADO CON EXITO", "EXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
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
    }

}