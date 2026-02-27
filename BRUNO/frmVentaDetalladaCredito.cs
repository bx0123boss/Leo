using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmVentaDetalladaCredito : frmBase
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public string id;
        double existenciasTotales = 0;
        public String usuario = "";
        public double adeudo = 0;
        string idCliente = "0";

        public frmVentaDetalladaCredito()
        {
            InitializeComponent();
            AplicarEstilos();
        }

        private void AplicarEstilos()
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPrimario(this.button2); // Reimprimir
            EstilizarBotonPeligro(this.button1);  // Cancelar
        }

        private void frmVentaDetalladaCredito_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from VentasCredito where FolioVenta='" + lblFolio.Text + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;

            if (usuario == "Invitado" || usuario == "NO")
            {
                button1.Hide();
            }

            if (dataGridView1.RowCount > 0)
            {
                idCliente = dataGridView1[6, 0].Value.ToString();
                cmd = new OleDbCommand("select * from Clientes where Id=" + idCliente + ";", conectar);
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

        private void button2_Click(object sender, EventArgs e)
        {
            // -- ÁREA DE REIMPRESIÓN --
            List<Producto> productos = new List<Producto>();
            double totalVenta = Convert.ToDouble(lblMonto.Text);

            // Extraer productos del grid
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                double cantidad = Convert.ToDouble(dataGridView1[3, i].Value);
                double totalProd = Convert.ToDouble(dataGridView1[5, i].Value);
                double precioUnitario = cantidad > 0 ? totalProd / cantidad : totalProd;

                productos.Add(new Producto
                {
                    Cantidad = cantidad,
                    Nombre = dataGridView1[4, i].Value.ToString(),
                    PrecioUnitario = precioUnitario,
                    Total = totalProd
                });
            }

            Dictionary<string, double> totales = new Dictionary<string, double>();
            if (Conexion.ConIva)
            {
                totales.Add("Subtotal", totalVenta / 1.16);
                totales.Add("IVA", (totalVenta / 1.16) * 0.16);
            }
            totales.Add("Total", totalVenta);

            if (Conexion.impresionMediaCarta)
            {
                try
                {
                    TicketMediaCarta pdfTicket = new TicketMediaCarta(
                         productos,
                         lblFolio.Text,
                         0, // Descuento
                         totalVenta,
                         lblCliente.Text,
                         idCliente,
                         "CRÉDITO", // Método de Pago
                         "", // Datos extra
                         "REIMPRESIÓN", // Observaciones
                         Conexion.lugar,
                         Conexion.logoPath,    // Logo
                         Conexion.datosTicket, // Encabezado
                         Conexion.pieDeTicket  // Pie de página
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
                TicketPrinter ticketPrinter = new TicketPrinter(Conexion.datosTicket, Conexion.pieDeTicket, Conexion.logoPath, productos, lblFolio.Text, "", lblCliente.Text, totalVenta, false, totales, "CRÉDITO");
                ticketPrinter.ImprimirTicket();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de cancelar la venta a crédito?", "Alto!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[2, i].Value.ToString() + "';", conectar);
                    OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        existenciasTotales = Convert.ToDouble(dataGridView1[3, i].Value.ToString()) + Convert.ToDouble(Convert.ToString(reader[4].ToString()));
                        cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existenciasTotales + "' Where Id='" + dataGridView1[2, i].Value.ToString() + "';", conectar);
                        cmd.ExecuteNonQuery();
                    }
                }

                cmd = new OleDbCommand("select * from Clientes where Id=" + idCliente + ";", conectar);
                OleDbDataReader reader2 = cmd.ExecuteReader();
                double adeu2 = 0;
                if (reader2.Read())
                {
                    adeu2 = Convert.ToDouble(reader2[7].ToString());
                }
                adeu2 -= adeudo; // Se asume que 'adeudo' contiene el monto exacto de esta venta que se está cancelando

                cmd = new OleDbCommand("update Ventas2 set Estatus='CANCELADO', Saldo='0' where Folio='" + lblFolio.Text + "';", conectar);
                cmd.ExecuteNonQuery();

                cmd = new OleDbCommand("update Clientes set Adeudo='" + adeu2 + "' where Id=" + idCliente + ";", conectar);
                cmd.ExecuteNonQuery();

                cmd = new OleDbCommand("insert into Corte(Concepto,Monto) Values('Cancelacion de la venta a credito folio " + lblFolio.Text + "',-" + lblMonto.Text + ");", conectar);
                cmd.ExecuteNonQuery();

                MessageBox.Show("VENTA A CRÉDITO CANCELADA CON ÉXITO", "CANCELADA!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }
    }
}