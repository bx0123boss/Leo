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
        double mas = 0;
        double menos = 0;
        double tarjeta = 0;
        double trans = 0;
        double otros = 0; // Nueva variable para los demás métodos de pago
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
            if (dgvCorte.Columns[e.ColumnIndex].Name == "Monto")
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
            // Formato de moneda para el Total
            if (dataGridView6.Columns[e.ColumnIndex].Name == "Total")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
            // Formato numérico para Piezas
            else if (dataGridView6.Columns[e.ColumnIndex].Name == "Piezas")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("N0"); // Muestra sin decimales (ej. 1,250)
                    e.FormattingApplied = true;
                }
            }

            // Estilo especial para la fila de TOTAL GENERAL
            if (e.RowIndex >= 0 && dataGridView6.Rows[e.RowIndex].Cells["Categoria"].Value?.ToString() == "TOTAL GENERAL")
            {
                dataGridView6.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(dataGridView6.Font, FontStyle.Bold);
                dataGridView6.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
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
            lblComision.Text = $"{porcentaje:F2}% Comisión:";

            cmd = new SqlCommand("select * from Fech where Id=1;", conectar);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int caja = Convert.ToInt32(Convert.ToString(reader[1].ToString()));
                fechaApertura = Convert.ToString(reader[2].ToString());
            }

            // 1. EFECTIVO
            ds = new DataSet();
            da = new SqlDataAdapter("select * from Corte where (Pago='01=EFECTIVO' OR Pago ='Efectivo') AND Concepto NOT LIKE 'Abono%';", conectar);
            da.Fill(ds, "Id");
            dgvCorte.DataSource = ds.Tables["Id"];
            dgvCorte.Columns[0].Visible = false;

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

            // 4. OTROS PAGOS (La Magia de la nueva pestaña)
            ds = new DataSet();
            da = new SqlDataAdapter("select * from Corte where Pago NOT IN ('01=EFECTIVO','Transferencia', 'Tarjeta','Efectivo','04=TARJETA DE CREDITO', '28=TARJETA DE DEBITO', '03=TRANFERENCIA ELECTRONICA DE FONDOS') AND Concepto NOT LIKE 'Abono%';", conectar);
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

            // ======================================================================
            // CÁLCULOS DE MONTOS
            // ======================================================================

            // Efectivo
            for (int i = 0; i < dgvCorte.RowCount; i++)
            {
                if (dgvCorte.Rows[i].IsNewRow) continue;
                if (dgvCorte[2, i].Value != null && dgvCorte[2, i].Value != DBNull.Value)
                {
                    double valorNumerico = Convert.ToDouble(dgvCorte[2, i].Value);
                    if (valorNumerico < 0) menos += Convert.ToSingle(valorNumerico);
                    else if (valorNumerico > 0) mas += Convert.ToSingle(valorNumerico);
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

            // Otros Pagos (Nueva Pestaña)
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
                        if (valorNumerico < 0) menos += Convert.ToSingle(valorNumerico);
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
                        // Si el abono fue con Vales o Monedero, se va a Otros
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

            // ASIGNACIÓN DE TEXTOS A LAS ETIQUETAS
            lblCorte.Text = $"{mas:C}";
            lblECaja.Text = $"{mas + menos:C}";
            lblBruta.Text = $"{utilidad - gastos:C}";
            lblGastos.Text = $"{gastos:C}";

            inversion = total - utilidad;
            lblInversion.Text = $"{inversion:C}";
            lblUtilidad.Text = $"{utilidad:C}";

            // Se agregan los Otros al cálculo de Entradas Generales
            lblEntrada.Text = $"{mas + tarjeta + trans + otros:C}";
            lblSalida.Text = $"{menos * -1:C}";

            decimal factorPorcentaje = porcentaje / 100m;
            decimal factorRestante = 1m - factorPorcentaje;
            lblCredito.Text = $"{(decimal)tarjeta * factorRestante:F2}";
            lbl5por.Text = $"{(decimal)tarjeta * factorPorcentaje:F2}";
            lblTrans.Text = $"{trans:C}";
            lblOtros.Text = $"{otros:C}"; // Etiqueta Nueva en Pantalla

            // MÁS IMPORTANTE: Que los Otros Pagos se sumen a tu Total del Día
            decimal monto = Convert.ToDecimal(tarjeta + mas + menos + trans + otros);

            decimal tasaIVA = 0.16m;
            decimal montoSinIVA = monto / (1 + tasaIVA);
            decimal iva = monto - montoSinIVA;

            lblNoIva.Text = $"{montoSinIVA:C2}";
            lblIVA.Text = $"{iva:C2}";
            lblTotal.Text = $"{monto:C2}";
           
            // ESTILIZACIÓN DE GRIDS
            EstilizarDataGridView(this.dgvFolios);
            EstilizarDataGridView(this.dgvCorte);
            EstilizarDataGridView(this.dgvOtrosPagos); // <--
            EstilizarDataGridView(this.dataGridView5);
            EstilizarDataGridView(this.dataGridView1);
            EstilizarDataGridView(this.dataGridView2);
            EstilizarDataGridView(this.dataGridView3);
            EstilizarDataGridView(this.dataGridView4);
            EstilizarDataGridView(this.dataGridView6);

            EstilizarBotonPrimario(this.button1);
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool tieneTotalGeneral = false;

            foreach (DataGridViewRow row in dgvCorte.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells["Concepto"].Value?.ToString() != "APERTURA DE CAJA")
                {
                    tieneTotalGeneral = true;
                    break;
                }
                
            }

            if (!tieneTotalGeneral)
            {
                MessageBox.Show("No hay artículos vendidos en este corte. No se puede realizar el corte de caja.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
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
            bool detallado = false;
            DialogResult dialogResult;
            if (Sesion.TienePermiso("IMPRESION_CORTE"))
            {
                dialogResult = MessageBox.Show("¿Desea imprimir el corte detallado?", "Alto!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    detallado = true;
                }
            }
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
                    // Extraemos el monto y el tipo de pago
                    string montoLimpio = Regex.Replace(row.Cells["Monto"].Value.ToString(), @"[^\d.-]", "");
                    double montoVenta = Convert.ToDouble(montoLimpio);
                    string tipoPago = row.Cells["Pago"].Value.ToString().ToUpper();

                    // --- SUMATORIAS PARA EL CORTE B ---
                    totalB += montoVenta;

                    if (tipoPago.Contains("EFECTIVO") || tipoPago.Contains("01=")) masB += montoVenta;
                    else if (tipoPago.Contains("TARJETA") || tipoPago.Contains("04=") || tipoPago.Contains("28=")) tarjetaB += montoVenta;
                    else if (tipoPago.Contains("TRANFERENCIA") || tipoPago.Contains("03=")) transB += montoVenta;
                    else otrosB += montoVenta;

                    // Buscamos la utilidad exacta de esta venta para que los reportes B cuadren perfecto
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
                        "'" + row.Cells["Descuento"].Value.ToString() + "', '" + row.Cells["Pago"].Value.ToString() + "', '"+foli+"');";
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
                // Insertamos el resumen (la sumatoria total) en histocortesB
                string queryHistoB = "INSERT INTO histocortesB (Id, Monto, Fecha, Mas, Menos, Tarjeta, utilidad, inversion, CubreFolio) VALUES ('" + folio + "', '" + totalB + "', '" + fechaCorteB + "', '" + masB + "', '0', '" + tarjetaB + "', '" + utilidadB + "', '" + inversionB + "','"+ rangoFolios+"');";
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

                // Identificamos la fila del gran total
                if (row.Cells["Categoria"].Value?.ToString() == "TOTAL GENERAL")
                {
                    // Limpiamos el formato (por si el CellFormatting le puso comas)
                    string piezasStr = row.Cells["Piezas"].Value?.ToString().Replace(",", "") ?? "0";
                    int.TryParse(piezasStr, out totalArticulos);
                    break; // Ya lo encontramos, dejamos de buscar
                }
            }
            string[] encabezados = new string[] { "********** CORTE DE CAJA  ********", 
                                                "Apertura de caja:", fechaApertura, 
                                                "Corte de caja:", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                                                 "TOTAL DE ARTICULOS: "+totalArticulos.ToString()};

            for (int i = 0; i < dgvCorte.RowCount; i++)
            {
                cmd = new SqlCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dgvCorte[1, i].Value.ToString() + "','" + dgvCorte[2, i].Value.ToString() + "','" + folio + "','" + dgvCorte[4, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                cmd = new SqlCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dataGridView3[1, i].Value.ToString() + "','" + dataGridView3[2, i].Value.ToString() + "','" + folio + "','" + dataGridView3[4, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            for (int i = 0; i < dataGridView4.RowCount; i++)
            {
                cmd = new SqlCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dataGridView4[1, i].Value.ToString() + "','" + dataGridView4[2, i].Value.ToString() + "','" + folio + "','" + dataGridView4[4, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }

            // Insertamos la nueva pestaña de OTROS al historial de cortes detallados
            for (int i = 0; i < dgvOtrosPagos.RowCount; i++)
            {
                cmd = new SqlCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dgvOtrosPagos[1, i].Value.ToString() + "','" + dgvOtrosPagos[2, i].Value.ToString() + "','" + folio + "','" + dgvOtrosPagos[4, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }

            cmd = new SqlCommand("INSERT INTO histocortes(Id,Monto,Fecha,Mas,Menos,Tarjeta,utilidad,inversion) VALUES ('" + folio + "','" + lblTotal.Text + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + mas + "','" + menos + "','" + tarjeta + "','" + utilidad + "','" + inversion + "');", conectar);
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

            dialogResult = MessageBox.Show("¿Desea imprimir el corte de caja?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Dictionary<string, double> totales = new Dictionary<string, double>();
                totales.Add("Efectivo", Convert.ToDouble(GetNumericValue(lblCorte.Text)));
                totales.Add("Tarjetas", Convert.ToDouble(GetNumericValue(lblCredito.Text)));
                totales.Add("Transferencias", Convert.ToDouble(GetNumericValue(lblTrans.Text)));

                // Que aparezca en el ticket impreso tu nueva categoría
                totales.Add("Otros Pagos", Convert.ToDouble(GetNumericValue(lblOtros.Text)));

                totales.Add("Sin IVA", Convert.ToDouble(GetNumericValue(lblNoIva.Text)));
                totales.Add("IVA", Convert.ToDouble(GetNumericValue(lblIVA.Text)));
                totales.Add("Total", Convert.ToDouble(GetNumericValue(lblTotal.Text)));
                string[] pieDePagina = new string[] { "" };
                TicketPrinter ticketPrinter = new TicketPrinter(encabezados, Conexion.pieDeTicket, Conexion.logoPath, productos, "", "", "", total, true, totales);
                if (Conexion.Ticket)
                    ticketPrinter.ImprimirTicket();
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
            if (Conexion.Font == "")
            {

            }
            else
                ticket.FontName = Conexion.Font;
            ticket.AddHeaderLine("******** CORTE PARCIAL  *******");
            ticket.AddSubHeaderLine("FECHA Y HORA:");
            ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            for (int i = 0; i < dgvCorte.RowCount; i++)
            {
                ticket.AddItem("1", dgvCorte[1, i].Value.ToString(), "   $" + dgvCorte[2, i].Value.ToString());
            }
            ticket.AddTotal("Efectivo", lblCorte.Text);
            ticket.AddTotal("Tarjetas", lblCredito.Text);
            ticket.AddTotal("Entradas", lblEntrada.Text);
            ticket.AddTotal("Salidas", lblSalida.Text);
            ticket.AddTotal("Total", lblTotal.Text);
            MessageBox.Show("CORTE REALIZADO CON EXITO", "EXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

    }
}