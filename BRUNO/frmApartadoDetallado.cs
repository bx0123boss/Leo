using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmApartadoDetallado : frmBase // Heredamos de frmBase para los estilos
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbDataAdapter da;
        OleDbCommand cmd;
        double existenciasTotales = 0;
        public String usuario = "";
        string nombreCliente = "PUBLICO EN GENERAL";

        public frmApartadoDetallado()
        {
            InitializeComponent();
            AplicarEstilos();
        }

        private void AplicarEstilos()
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPeligro(this.button1);  // Cancelar
            EstilizarBotonPrimario(this.button2); // Reimprimir Ticket
        }

        private void frmApartadoDetallado_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from VentasApartados where FolioVenta='" + lblFolio.Text + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[7].Visible = false;
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;

            lblMonto.Text = "" + (Convert.ToDouble(lblRestante.Text) + Convert.ToDouble(lblAbonado.Text));

            if (usuario == "Invitado")
            {
                button1.Hide();
            }

            // Buscamos el nombre del cliente para la impresión del ticket
            if (dataGridView1.RowCount > 0)
            {
                string idCliente = dataGridView1.Rows[0].Cells[6].Value.ToString();
                cmd = new OleDbCommand("select Nombre from Clientes where Id=" + idCliente, conectar);
                try
                {
                    object res = cmd.ExecuteScalar();
                    if (res != null) nombreCliente = res.ToString();
                }
                catch { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de cancelar el apartado?", "Alto!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1[8, i].Value.ToString() == "DESCONTADO DE INVENTARIO")
                    {
                        cmd = new OleDbCommand("select Existencia from Inventario where Id='" + dataGridView1[2, i].Value.ToString() + "';", conectar);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            existenciasTotales = Convert.ToDouble(dataGridView1[3, i].Value.ToString()) + Convert.ToDouble(Convert.ToString(reader[0].ToString()));
                            cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existenciasTotales + "' Where Id='" + dataGridView1[2, i].Value.ToString() + "';", conectar);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    cmd = new OleDbCommand("UPDATE VentasApartados set Estatus='CANCELADO' where Id=" + dataGridView1[0, i].Value.ToString(), conectar);
                    cmd.ExecuteNonQuery();
                }

                cmd = new OleDbCommand("delete from Apartados where Folio='" + lblFolio.Text + "';", conectar);
                cmd.ExecuteNonQuery();

                cmd = new OleDbCommand("insert into Corte(Concepto,Monto) Values('Cancelacion de la apartado folio " + lblFolio.Text + "',-" + lblAbonado.Text + ");", conectar);
                cmd.ExecuteNonQuery();

                MessageBox.Show("APARTADO CANCELADO CON EXITO", "CANCELADA!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                frmApartados apart = new frmApartados();
                apart.usuario = this.usuario;
                apart.Show();
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Producto> productos = new List<Producto>();
            double totalMonto = Convert.ToDouble(lblMonto.Text);
            double abonado = Convert.ToDouble(lblAbonado.Text);
            double restante = Convert.ToDouble(lblRestante.Text);

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
            totales.Add("Total", totalMonto);
            totales.Add("Abonado", abonado);
            totales.Add("Restante", restante);

            if (Conexion.impresionMediaCarta)
            {
                try
                {
                    string idCliente = dataGridView1.RowCount > 0 ? dataGridView1.Rows[0].Cells[6].Value.ToString() : "0";

                    TicketMediaCarta pdfTicket = new TicketMediaCarta(
                            productos,
                            lblFolio.Text,
                            0, // Sin descuento calculado aquí explícitamente
                            totalMonto,
                            nombreCliente,
                            idCliente,
                            "EFECTIVO", // Método de Pago
                            "", // Datos
                            "REIMPRESIÓN APARTADO", // Observaciones
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
                TicketPrinter ticketPrinter = new TicketPrinter(Conexion.datosTicket, Conexion.pieDeTicket, Conexion.logoPath, productos, lblFolio.Text, "", nombreCliente, totalMonto, false, totales, "EFECTIVO");
                ticketPrinter.ImprimirTicket();
            }
        }
    }
}