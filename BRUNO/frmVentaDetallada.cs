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
        public frmVentaDetallada(string folio)
        {
            InitializeComponent();
            esConsigna = true;
            lblFolio.Text = folio;
            lblFolio.Visible = true;
            label2.Visible = true;
            try
            {
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();
                    string query = "SELECT Monto, Fecha, Estatus, Descuento, Pago FROM Ventas WHERE Folio = ?";
                    using (OleDbCommand cmdVenta = new OleDbCommand(query, con))
                    {
                        cmdVenta.Parameters.AddWithValue("?", folio);
                        using (OleDbDataReader reader = cmdVenta.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal montoTotal = Convert.ToDecimal(reader["Monto"]);
                                decimal descuento = reader["Descuento"] != DBNull.Value ? Convert.ToDecimal(reader["Descuento"]) : 0;
                                string estatus = reader["Estatus"].ToString();

                                lblMonto.Text = $"{montoTotal:C}";
                                this.monto = montoTotal; // Tu variable pública

                                lblDescuento.Text = $"{descuento:C}";
                                lblPago.Text = reader["Pago"].ToString();
                                lblFecha.Text = reader["Fecha"].ToString();

                                if (estatus == "CANCELADO")
                                {
                                    button1.Visible = false; 
                                    button2.Visible = false;
                                }
                                else
                                {
                                    button1.Visible = true;
                                    button2.Visible = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("No se encontraron los datos generales de la venta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos de la venta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                reader.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de cancelar la venta?", "Alto!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                string folioCancelar = lblFolio.Text;

                // =========================================================================
                // 1. VERIFICAR SI ESTA VENTA FUE UNA LIQUIDACIÓN DE CONSIGNA
                // =========================================================================
                bool esVentaConsigna = false;
                cmd = new OleDbCommand("SELECT COUNT(*) FROM ConsignaMovimientos WHERE ReferenciaVentaId = '" + folioCancelar + "'", conectar);
                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    esVentaConsigna = true;
                }

                if (esVentaConsigna)
                {
                    // ---> LÓGICA DE CANCELACIÓN PARA CONSIGNA <---

                    // A) Leemos todos los movimientos que generó este folio
                    DataTable dtMovs = new DataTable();
                    cmd = new OleDbCommand("SELECT ClienteId, ProductoId, Cantidad, PrecioVigente FROM ConsignaMovimientos WHERE ReferenciaVentaId = '" + folioCancelar + "'", conectar);
                    OleDbDataAdapter daMovs = new OleDbDataAdapter(cmd);
                    daMovs.Fill(dtMovs);

                    foreach (DataRow row in dtMovs.Rows)
                    {
                        int cId = Convert.ToInt32(row["ClienteId"]);
                        string pId = row["ProductoId"].ToString();
                        int cant = Convert.ToInt32(row["Cantidad"]);
                        decimal precio = Convert.ToDecimal(row["PrecioVigente"]);

                        // B) Rebotamos los saldos al cliente (Regresan a EnConsigna, se restan de Vendidos)
                        cmd = new OleDbCommand("UPDATE ConsignaCliente SET EnConsigna = EnConsigna + ?, Vendidos = Vendidos - ? WHERE ClienteId = ? AND ProductoId = ? AND PrecioCongelado = ?", conectar);
                        cmd.Parameters.AddWithValue("?", cant);
                        cmd.Parameters.AddWithValue("?", cant);
                        cmd.Parameters.AddWithValue("?", cId);
                        cmd.Parameters.AddWithValue("?", pId);
                        cmd.Parameters.AddWithValue("?", precio);
                        cmd.ExecuteNonQuery();

                        // C) Dejamos rastro en el Kardex de la Consigna
                        cmd = new OleDbCommand("INSERT INTO ConsignaMovimientos (ClienteId, ProductoId, TipoMovimiento, Cantidad, PrecioVigente, Fecha, ReferenciaVentaId) VALUES (?, ?, 'CANCELACION', ?, ?, NOW(), ?)", conectar);
                        cmd.Parameters.AddWithValue("?", cId);
                        cmd.Parameters.AddWithValue("?", pId);
                        cmd.Parameters.AddWithValue("?", cant);
                        cmd.Parameters.AddWithValue("?", precio);
                        cmd.Parameters.AddWithValue("?", folioCancelar);
                        cmd.ExecuteNonQuery();
                    }

                    // D) Borramos los totales en VentasContado como se hace normalmente
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        cmd = new OleDbCommand("UPDATE VentasContado set MontoTotal='0', Utilidad='0' Where Id=" + dataGridView1[0, i].Value.ToString() + ";", conectar);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // ---> LÓGICA ORIGINAL PARA VENTA NORMAL (Rebota a inventario tienda) <---
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

                            cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[2, i].Value.ToString() + "','ENTRADA','CANCELACION DE VENTA FOLIO: " + lblFolio.Text + "'," + Convert.ToString(reader[4].ToString()) + ",'" + existenciasTotales + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                            cmd.ExecuteNonQuery();
                        }
                        reader.Close(); // ¡Muy importante mantenerlo cerrado!
                    }
                }

                cmd = new OleDbCommand("update Ventas set Estatus='CANCELADO' where Folio='" + lblFolio.Text + "';", conectar);
                cmd.ExecuteNonQuery();

                // =========================================================================
                // 2. ACTUALIZAR ESTADO A 'CANCELADO' Y REVERTIR CORTES (COMÚN PARA AMBOS)
                // =========================================================================
                cmd = new OleDbCommand("update Ventas set Estatus='CANCELADO' where Folio='" + folioCancelar + "';", conectar);
                cmd.ExecuteNonQuery();

                // Buscamos si existen los pagos originales de este ticket en el Corte de hoy
                cmd = new OleDbCommand("SELECT Monto, Pago FROM Corte WHERE Concepto = 'Venta a contado folio " + folioCancelar + "';", conectar);
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
                readerCorte.Close(); // Cerrar antes de hacer Inserts

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

                MessageBox.Show("VENTA CANCELADA CON EXITO", "CANCELADA!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void frmVentaDetallada_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (esConsigna) return;
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