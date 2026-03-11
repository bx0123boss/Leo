using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BRUNO
{
    public partial class frmCortePorFechas : frmBase
    {
        public frmCortePorFechas()
        {
            InitializeComponent();
        }

        private void frmCortePorFechas_Load(object sender, EventArgs e)
        {
            EstilizarBotonPrimario(button1);
            EstilizarDataGridView(dataGridView1);

            dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePicker2.Value = DateTime.Now;

            ConfigurarDisenoGrafico(chart1, "TENDENCIA DE VENTAS EN CAJA (+)");
            ConfigurarDisenoGrafico(chart2, "COSTO DE INVERSIÓN VENDIDA (-)");
            ConfigurarDisenoGrafico(chart3, "UTILIDAD BRUTA OBTENIDA (+)");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();

            DateTime fechaInicio = dateTimePicker1.Value.Date;
            DateTime fechaFin = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);

            // 1. OBTENER HISTORIAL DE CORTES PARA GRÁFICAS
            string queryCortes = "SELECT * FROM histocortes WHERE Fecha >= @inicio AND Fecha <= @fin ORDER BY Fecha ASC";

            using (OleDbConnection conectar = new OleDbConnection(Conexion.CadCon))
            {
                conectar.Open();
                using (OleDbCommand cmd = new OleDbCommand(queryCortes, conectar))
                {
                    cmd.Parameters.AddWithValue("@inicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fin", fechaFin);

                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        da.Fill(ds, "Id");
                    }
                }

                // 2. OBTENER TOTAL DE TICKETS PARA EL TICKET PROMEDIO
                string queryTickets = "SELECT COUNT(Id) AS NumTickets FROM Ventas WHERE Fecha >= ? AND Fecha <= ? AND Estatus = 'COBRADO'";
                int numTickets = 0;

                using (OleDbCommand cmdTickets = new OleDbCommand(queryTickets, conectar))
                {
                    cmdTickets.Parameters.AddWithValue("?", fechaInicio);
                    cmdTickets.Parameters.AddWithValue("?", fechaFin);

                    object result = cmdTickets.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        numTickets = Convert.ToInt32(result);
                    }
                }

                // PROCESAR DATOS DE CORTES
                dataGridView1.DataSource = ds.Tables["Id"];

                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns[0].Visible = false; // ID
                    dataGridView1.Columns[3].Visible = false;
                    dataGridView1.Columns[4].Visible = false;
                    dataGridView1.Columns[5].Visible = false;
                    dataGridView1.Columns[2].HeaderText = "Fecha de Corte";
                }

                decimal granTotalVentas = 0m;
                decimal granTotalInversion = 0m;
                decimal granTotalUtilidad = 0m;

                chart1.Series.Clear();
                chart2.Series.Clear();
                chart3.Series.Clear();

                Series serieVentas = chart1.Series.Add("Ventas");
                Series serieInversion = chart2.Series.Add("Inversión");
                Series serieUtilidad = chart3.Series.Add("Utilidad");

                ConfigurarSerieBarras(serieVentas, Color.MediumSeaGreen);
                ConfigurarSerieBarras(serieInversion, Color.Tomato);
                ConfigurarSerieBarras(serieUtilidad, Color.DeepSkyBlue);

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].IsNewRow) continue;

                    string strVenta = dataGridView1[1, i].Value?.ToString() ?? "0";
                    string strUtilidad = dataGridView1[6, i].Value?.ToString() ?? "0";
                    string strInversion = dataGridView1[7, i].Value?.ToString() ?? "0";
                    string fecha = dataGridView1[2, i].Value?.ToString() ?? "";

                    decimal ventaDia = Convert.ToDecimal(Regex.Replace(strVenta, @"[^\d.-]", ""));
                    decimal utilidadDia = Convert.ToDecimal(Regex.Replace(strUtilidad, @"[^\d.-]", ""));
                    decimal inversionDia = Convert.ToDecimal(Regex.Replace(strInversion, @"[^\d.-]", ""));

                    granTotalVentas += ventaDia;
                    granTotalUtilidad += utilidadDia;
                    granTotalInversion += inversionDia;

                    if (fecha.Length > 10) fecha = fecha.Substring(0, 10);

                    serieVentas.Points.AddXY(fecha, Math.Round(ventaDia));
                    serieInversion.Points.AddXY(fecha, Math.Round(inversionDia));
                    serieUtilidad.Points.AddXY(fecha, Math.Round(utilidadDia));
                }

                // 3. MOSTRAR KPIS FINANCIEROS (Fila de arriba)
                lblVentas.Text = granTotalVentas.ToString("C2");
                lblInversion.Text = granTotalInversion.ToString("C2");
                lblUtilidad.Text = granTotalUtilidad.ToString("C2");

                decimal margen = 0m;
                if (granTotalVentas > 0)
                {
                    margen = (granTotalUtilidad / granTotalVentas) * 100;
                }
                lblMargen.Text = margen.ToString("F2") + "%";

                // 4. MOSTRAR KPIS DE VENTAS (Fila de abajo: Ticket Promedio)
                decimal ticketPromedio = 0m;
                if (numTickets > 0)
                {
                    ticketPromedio = granTotalVentas / numTickets;
                }

                int diasConsultados = (int)Math.Ceiling((fechaFin - fechaInicio).TotalDays);
                if (diasConsultados <= 0) diasConsultados = 1;
                decimal ventaDiaria = granTotalVentas / diasConsultados;

                lblTotalTickets.Text = numTickets.ToString("N0");
                lblTicketPromedio.Text = ticketPromedio.ToString("C2");
                lblVentaDiaria.Text = ventaDiaria.ToString("C2");
            }
        }

        private void ConfigurarDisenoGrafico(Chart chart, string tituloTexto)
        {
            chart.BackColor = Color.FromArgb(35, 35, 35);
            chart.ChartAreas[0].BackColor = Color.FromArgb(45, 45, 45);

            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(60, 60, 60);
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(60, 60, 60);
            chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.LightGray;
            chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.LightGray;

            chart.Legends.Clear();

            chart.Titles.Clear();
            Title titulo = new Title(tituloTexto);
            titulo.Font = new Font("Segoe UI Semibold", 12f, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.Alignment = ContentAlignment.TopLeft;
            chart.Titles.Add(titulo);
        }

        private void ConfigurarSerieBarras(Series serie, Color color)
        {
            serie.ChartType = SeriesChartType.Column;
            serie.Color = color;
            serie.BorderColor = Color.FromArgb(50, 255, 255, 255);
            serie.BorderWidth = 1;
            serie.IsValueShownAsLabel = true;
            serie.LabelForeColor = Color.White;
            serie.LabelFormat = "C0";
            serie.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        }
    }
}