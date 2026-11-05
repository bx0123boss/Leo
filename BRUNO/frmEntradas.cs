using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmEntradas : Form
    {
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL); 
        //SqlConnection conectar = new SqlConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        SqlCommand cmd;
        public string usuario;
        public frmEntradas()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIngreso.Text) || string.IsNullOrWhiteSpace(txtConcepto.Text))
            {
                MessageBox.Show("Por favor complete todos los campos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                double monto = 0;
                double.TryParse(txtIngreso.Text, out monto);
                conectar.Open();
                cmd = new SqlCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Entrada en caja por concepto: " + txtConcepto.Text + "'," + txtIngreso.Text + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','01=EFECTIVO');", conectar);
                cmd.ExecuteNonQuery();

                // =========================================================
                // IMPRESIÓN DEL COMPROBANTE DE RETIRO CON LA CLASE GENÉRICA
                // =========================================================
                DialogResult printResult = MessageBox.Show("¿Desea imprimir el comprobante de ingreso?", "Imprimir", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (printResult == DialogResult.Yes)
                {
                    GenericTicketPrinter ticket = new GenericTicketPrinter();

                    ticket.AddTitle("COMPROBANTE DE INGRESO")
                          .AddText("ENTRADA A CAJA CHICA", true, System.Drawing.StringAlignment.Center)
                          .AddLine()
                          .AddRow("FECHA:", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"))
                          .AddRow("USUARIO:", !string.IsNullOrEmpty(usuario) ? usuario : "CAJERO")
                          .AddLine()
                          .AddText("CONCEPTO:", true)
                          .AddText(txtConcepto.Text.Trim())
                          .AddSpace(5)
                          .AddRow("MONTO INGRESADO:", $"${monto:N2}", true)
                          .AddLine()
                          .AddSpace(10)
                          // CAMPOS PARA LLENAR A LAPICERO
                          .AddFillField("ENTREGADO POR (NOMBRE):")
                          .AddSpace(15)
                          .AddFillField("FIRMA DE CONFORMIDAD:")
                          .AddSpace(10)
                          .AddText("Conserve este comprobante para su arqueo.", false, System.Drawing.StringAlignment.Center);

                    // Reemplaza por tu variable global de impresora si aplica
                    ticket.Print();
                }

                MessageBox.Show("Se ha depositado con exito", "DEPOSITADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                conectar.Close();
                this.Close();
            }
        }

        private void frmEntradas_Load(object sender, EventArgs e)
        {
            AbrirTeclado();
        }
        private void AbrirTeclado()
        {
            string RutaTeclado = @"C:\Jaeger Soft\FreeVK.exe";
            try
            {
                if (File.Exists(RutaTeclado))
                {
                    // Verifica si el proceso YA está corriendo para evitar abrir múltiples instancias
                    if (Process.GetProcessesByName("FreeVK").Length == 0)
                    {
                        Process.Start(RutaTeclado);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al abrir el teclado: " + ex.Message);
            }
        }

        private void txtIngreso_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
                                                     