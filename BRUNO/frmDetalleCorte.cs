using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using LibPrintTicket;

namespace BRUNO
{
    public partial class frmDetalleCorte : frmBase
    {
        public int ID;
        public DataSet ds;
        // Uso de variables DECIMALES para que el dinero no falle en centavos
        public decimal mas = 0m;
        public  decimal menos = 0m;
        public decimal tarjeta = 0m;
        public decimal trans = 0m;
        public decimal granTotal = 0m;

        public frmDetalleCorte()
        {
            InitializeComponent();
        }

        private void frmDetalleCorte_Load(object sender, EventArgs e)
        {
            // 1. Estilos heredados del frmBase
            EstilizarBotonPrimario(button1);
            EstilizarBotonPrimario(button2);

            ds = new DataSet();

            // 2. Uso correcto de USING para no dejar conexiones bloqueadas
            using (OleDbConnection conectar = new OleDbConnection(Conexion.CadCon))
            {
                conectar.Open();
                using (OleDbDataAdapter da = new OleDbDataAdapter("select * from Cortes where idCorte='" + ID + "';", conectar))
                {
                    da.Fill(ds, "Id");
                }
            }

            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false; // Id general
            dataGridView1.Columns[3].Visible = false; // idCorte
            dataGridView1.Columns[4].Visible = false; // Tipo interno si aplica

            EstilizarDataGridView(dataGridView1);
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            // 3. CALCULAR LOS TOTALES RECORRIENDO EL GRID 
            // (Igual que se hace en frmCorte activo)
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].IsNewRow) continue;

                if (dataGridView1[2, i].Value != null && dataGridView1[2, i].Value != DBNull.Value)
                {
                    decimal monto = Convert.ToDecimal(dataGridView1[2, i].Value);

                    // Asegurarnos de buscar el tipo en la columna correcta (columna 5 suele ser 'Tipo')
                    string tipoPago = "";
                    if (dataGridView1[5, i].Value != null)
                        tipoPago = dataGridView1[5, i].Value.ToString().ToUpper();

                    // Clasificamos y sumamos a la variable correspondiente
                    if (tipoPago.Contains("TARJETA") || tipoPago.Contains("04=") || tipoPago.Contains("28="))
                    {
                        tarjeta += monto;
                    }
                    else if (tipoPago.Contains("TRANFERENCIA") || tipoPago.Contains("TRANSFERENCIA") || tipoPago.Contains("03="))
                    {
                        trans += monto;
                    }
                    else // Si es EFECTIVO, u otro método por defecto
                    {
                        if (monto < 0)
                            menos += monto;
                        else
                            mas += monto;
                    }
                }
            }

            // 4. ASIGNAR LOS VALORES A LA INTERFAZ
            granTotal = mas + menos + tarjeta + trans;

            lblMonto.Text = granTotal.ToString("C2");
            lblEfectivo.Text = mas.ToString("C2");
            lblTarjetas.Text = tarjeta.ToString("C2");
            lblTransferencias.Text = trans.ToString("C2");
            lblSalidas.Text = (menos * -1m).ToString("C2"); // Multiplicar por -1 para mostrarlo positivo en pantalla
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Código de Excel intacto
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;

            xla.Visible = true;

            ws.Cells[1, 1] = "Concepto";
            ws.Cells[1, 2] = "Monto Total";
            ws.Cells[1, 3] = "Tipo Pago";
            ws.Cells[1, 4] = "Fecha de Corte: " + lblFecha.Text;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                ws.Cells[i + 2, 1] = dataGridView1[1, i].Value.ToString();
                ws.Cells[i + 2, 2] = dataGridView1[2, i].Value.ToString();
                ws.Cells[i + 2, 3] = dataGridView1[5, i].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Preparamos la lista de productos (en este caso, los conceptos del corte)
            List<Producto> conceptos = new List<Producto>();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].IsNewRow) continue;

                conceptos.Add(new Producto
                {
                    Cantidad = 1,
                    Nombre = dataGridView1[1, i].Value.ToString(),
                    PrecioUnitario = Convert.ToDouble(dataGridView1[2, i].Value),
                    Total = Convert.ToDouble(dataGridView1[2, i].Value)
                });
            }

            // 2. Encabezados especiales del corte
            string[] encabezados = new string[] {
                "******** REIMPRESION CORTE  ********",
                "               Fecha de corte:",
                lblFecha.Text
            };

            // 3. Diccionario de totales usando el nuevo TicketPrinter (igual que frmCorte)
            Dictionary<string, double> totalesTicket = new Dictionary<string, double>();
            totalesTicket.Add("Efectivo en Caja", Convert.ToDouble(mas));
            totalesTicket.Add("Tarjetas", Convert.ToDouble(tarjeta));
            totalesTicket.Add("Transferencias", Convert.ToDouble(trans));
            totalesTicket.Add("Salidas", Convert.ToDouble(menos * -1));
            totalesTicket.Add("TOTAL DEL CORTE", Convert.ToDouble(granTotal));

            try
            {
                // Instanciamos TicketPrinter igual que en tu corte normal
                TicketPrinter ticketPrinter = new TicketPrinter(
                    encabezados,
                    Conexion.pieDeTicket,
                    Conexion.logoPath,
                    conceptos,
                    "",
                    "",
                    "",
                    Convert.ToDouble(granTotal),
                    true,
                    totalesTicket);

                ticketPrinter.ImprimirTicket();
                MessageBox.Show("Corte reimpreso con éxito.", "Impresión", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir el ticket: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}