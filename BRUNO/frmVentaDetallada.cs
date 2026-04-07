using LibPrintTicket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmVentaDetallada : frmBase
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        OleDbCommand cmd;
        double existenciasTotales = 0;
        public String usuario = "";
        public decimal monto;

        // --- NUEVA VARIABLE PARA EL FOLIO REAL (El de la Base de Datos) ---
        public string folioRealConsulta = "";

        public frmVentaDetallada()
        {
            InitializeComponent();
        }

        private void frmVentaDetallada_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPeligro(this.button1);
            EstilizarBotonPrimario(this.button2);
            dataGridView1.ReadOnly = true;
            ds = new DataSet();
            conectar.Open();

            // --- LÓGICA PARA ELEGIR QUÉ FOLIO USAR PARA BUSCAR ---
            // Si nos pasaron un Folio Real, usamos ese. Si no, usamos el que está en el Label (para retrocompatibilidad)
            string folioBaseDatos = string.IsNullOrEmpty(folioRealConsulta) ? lblFolio.Text : folioRealConsulta;

            da = new OleDbDataAdapter("select * from VentasContado where FolioVenta='" + folioBaseDatos + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];

            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[6].Visible = false;

            if (usuario == "Invitado")
            {
                button1.Hide();
            }

            if (dataGridView1.Rows.Count > 0)
            {
                cmd = new OleDbCommand("select * from Clientes where Id=" + dataGridView1[6, 0].Value.ToString() + ";", conectar);
                OleDbDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblCliente.Text = Convert.ToString(reader[1].ToString());
                    lblDireccion.Text = Convert.ToString(reader[3].ToString());
                }
                else
                {
                    lblCliente.Text = "PUBLICO EN GENERAL";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de cancelar la venta?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // El folio que el usuario ve
                string folioVisual = lblFolio.Text;
                // El folio real en la base de datos Ventas
                string folioBaseDatos = string.IsNullOrEmpty(folioRealConsulta) ? lblFolio.Text : folioRealConsulta;

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[2, i].Value.ToString() + "';", conectar);
                    OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        existenciasTotales = Convert.ToDouble(dataGridView1[3, i].Value.ToString()) + Convert.ToDouble(Convert.ToString(reader[4].ToString()));
                        cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existenciasTotales + "' Where Id='" + dataGridView1[2, i].Value.ToString() + "';", conectar);
                        cmd.ExecuteNonQuery();

                        cmd = new OleDbCommand("UPDATE VentasContado set MontoTotal='0', Utilidad='0' Where Id=" + dataGridView1[0, i].Value.ToString() + ";", conectar);
                        cmd.ExecuteNonQuery();

                        // En el kardex es bueno que el usuario lea el folio que reconozca visualmente
                        cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[2, i].Value.ToString() + "','ENTRADA','CANCELACION DE VENTA FOLIO: " + folioVisual + "'," + Convert.ToString(reader[4].ToString()) + ",'" + existenciasTotales + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                        cmd.ExecuteNonQuery();
                    }
                    reader.Close();
                }

                // Aquí usamos el folio Real para encontrar el registro en Ventas
                cmd = new OleDbCommand("update Ventas set Estatus='CANCELADO' where Folio='" + folioBaseDatos + "';", conectar);
                cmd.ExecuteNonQuery();

                // =========================================================================
                // NUEVA LÓGICA DE CANCELACIÓN EN CORTES (SOPORTE PARA PAGO MIXTO)
                // =========================================================================

                // Buscamos en el corte el folio de base de datos
                cmd = new OleDbCommand("SELECT Monto, Pago FROM Corte WHERE Concepto = 'Venta a contado folio " + folioBaseDatos + "';", conectar);
                OleDbDataReader readerCorte = cmd.ExecuteReader();

                bool encontroPagosEnCorte = false;
                List<Tuple<double, string>> pagosARevertir = new List<Tuple<double, string>>();

                while (readerCorte.Read())
                {
                    encontroPagosEnCorte = true;
                    double montoOriginal = Convert.ToDouble(readerCorte["Monto"]);
                    string pagoOriginal = readerCorte["Pago"].ToString();
                    pagosARevertir.Add(new Tuple<double, string>(montoOriginal, pagoOriginal));
                }
                readerCorte.Close(); // OBLIGATORIO: Cerrar lector en Access antes de hacer Inserts

                if (encontroPagosEnCorte)
                {
                    foreach (var pago in pagosARevertir)
                    {
                        cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Cancelacion de la venta a contado folio " + folioVisual + "'," + (pago.Item1 * -1) + ",'" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + pago.Item2 + "');", conectar);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    string metodoReembolso = lblPago.Text;
                    if (metodoReembolso == "MIXTO")
                    {
                        metodoReembolso = "01=EFECTIVO";
                    }

                    cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Cancelacion de la venta a contado folio " + folioVisual + "',-" + monto + ",'" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "' ,'" + metodoReembolso + "');", conectar);
                    cmd.ExecuteNonQuery();
                }
                // =========================================================================

                MessageBox.Show("VENTA CANCELADA CON EXITO", "CANCELADA!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void frmVentaDetallada_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmReporteVentas repor = new frmReporteVentas();
            repor.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Producto> productos = new List<Producto>();

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                productos.Add(new Producto
                {
                    Nombre = dataGridView1[4, i].Value.ToString(),
                    Cantidad = Convert.ToDouble(dataGridView1[3, i].Value.ToString()),
                    PrecioUnitario = Convert.ToDouble(dataGridView1[5, i].Value.ToString()) / Convert.ToDouble(dataGridView1[3, i].Value.ToString()),
                    Total = Convert.ToDouble(dataGridView1[5, i].Value.ToString()),
                });
            }

            string GetNumericValue(string input)
            {
                return Regex.Replace(input, @"[^\d.-]", "");
            }

            double total = Convert.ToDouble(GetNumericValue(lblMonto.Text));
            Dictionary<string, double> totales = new Dictionary<string, double>();
            totales.Add("Subtotal", total / 1.16);
            totales.Add("IVA", (total / 1.16) * 0.16);
            totales.Add("Total", total);

            if (Conexion.impresionMediaCarta)
            {
                try
                {
                    TicketMediaCarta pdfTicket = new TicketMediaCarta(
                         productos,
                         lblFolio.Text, // <-- En la impresión SÍ usamos el visual para el cliente
                         0,
                         total,
                         lblCliente.Text,
                         dataGridView1[6, 0].Value.ToString(),
                         lblPago.Text,
                         "",
                         "",
                         Conexion.lugar,
                         Conexion.logoPath,
                         Conexion.datosTicket,
                         Conexion.pieDeTicket
                     );

                    pdfTicket.ImprimirDirectamente(Conexion.impresora);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al imprimir PDF Media Carta: " + ex.Message);
                }
            }
            else
            {
                TicketPrinter ticketPrinter = new TicketPrinter(Conexion.datosTicket, Conexion.pieDeTicket, Conexion.logoPath, productos, lblFolio.Text, "", "", 0, false, totales, lblPago.Text);
                ticketPrinter.ImprimirTicket();
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "MontoTotal" || dataGridView1.Columns[e.ColumnIndex].Name == "Utilidad")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }
    }
}