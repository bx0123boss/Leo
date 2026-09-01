using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using LibPrintTicket;
using System.Globalization;

namespace JaegerSoft
{
    public partial class frmAbonoCliente : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);

        // OleDbConnection conectar = new OleDbConnection(
        //     @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");

        OleDbCommand cmd;

        public string folio = "";
        public double saldo = 0;
        public double adeudo = 0;
        public double adeudoFact = 0;

        public frmAbonoCliente()
        {
            InitializeComponent();
            conectar.Open();
        }

        private void frmAbonoCliente_Load(object sender, EventArgs e)
        {
            cmbPago.SelectedIndex = 0;

           

            if (saldo > 0)
                txtAdeudo.Text = saldo.ToString();
            else
                txtAdeudo.Text = adeudo.ToString();
        }

        // ============================================================
        // VALIDAR ABONO
        // ============================================================
        private bool ValidarAbono()
        {
            try
            {
                double montoAbono = Convert.ToDouble(txtAbono.Text);
                double montoAdeudo = Convert.ToDouble(txtAdeudo.Text);

                double restante = montoAdeudo - montoAbono;

                if (montoAbono <= 0)
                {
                    txtAbono.Focus();

                    MessageBox.Show(
                        "Ingrese una cantidad válida.",
                        "ERROR",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
                }

                if (restante < 0)
                {
                    txtAbono.Text = "";
                    txtAbono.Focus();

                    MessageBox.Show(
                        "El abono no puede ser mayor al adeudo.",
                        "ERROR",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
                }

                txtRestante.Text = restante.ToString();

                return true;
            }
            catch
            {
                txtAbono.Text = "";
                txtAbono.Focus();

                MessageBox.Show(
                    "No se ha introducido correctamente un dato, favor de verificar.",
                    "ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        // ============================================================
        // CUANDO SALE DEL TEXTBOX
        // ============================================================
        private void txtAbono_Leave(object sender, EventArgs e)
        {
            ValidarAbono();
        }

        // ============================================================
        // ENTER EN TXTABONO
        // ============================================================
        private void txtAbono_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;

            // Números y separador decimal
            if (char.IsNumber(e.KeyChar) ||
                e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator)
            {
                e.Handled = false;
            }
            // ENTER
            else if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ValidarAbono();
            }
            // Cualquier otro carácter
            else
            {
                e.Handled = true;
            }
        }

        // ============================================================
        // BOTÓN ABONAR
        // ============================================================
        private void button1_Click(object sender, EventArgs e)
        {
            // Validar antes de continuar
            if (!ValidarAbono())
                return;

            try
            {
                double adeudoAnterior = Convert.ToDouble(txtAdeudo.Text);
                double abonoMonto = Convert.ToDouble(txtAbono.Text);
                double saldoInsoluto = Convert.ToDouble(txtRestante.Text);

                // Actualizar adeudo
                adeudoFact = -abonoMonto;
                double nuevoAdeudo = adeudo + adeudoFact;

                // ====================================================
                // ACTUALIZAR CLIENTE
                // ====================================================
                cmd = new OleDbCommand(
                    "UPDATE Clientes SET Adeudo = " + nuevoAdeudo +
                    ", UltimoPago = '" +
                    DateTime.Now.ToShortDateString() + " " +
                    DateTime.Now.ToShortTimeString() +
                    "' WHERE Id = " + lblID.Text + ";",
                    conectar);

                cmd.ExecuteNonQuery();

                // ====================================================
                // ACTUALIZAR VENTA
                // ====================================================
                cmd = new OleDbCommand(
                    "UPDATE Ventas2 SET Saldo = '" +
                    txtRestante.Text +
                    "' WHERE Folio = '" +
                    folio +
                    "';",
                    conectar);

                cmd.ExecuteNonQuery();

                // ====================================================
                // INSERTAR EN CORTE
                // ====================================================
                cmd = new OleDbCommand(
                    "INSERT INTO Corte " +
                    "(Concepto, Monto, FechaHora, Pago) " +
                    "VALUES (" +
                    "'Abono a cuenta de cliente " + lblCliente.Text + "'," +
                    "'" + txtAbono.Text + "'," +
                    "'" + DateTime.Now.ToShortDateString() + " " +
                    DateTime.Now.ToShortTimeString() + "'," +
                    "'" + cmbPago.Text + "'" +
                    ");",
                    conectar);

                cmd.ExecuteNonQuery();

                // ====================================================
                // INSERTAR ABONO
                // ====================================================
                cmd = new OleDbCommand(
                    "INSERT INTO Abonos " +
                    "(Abono, idCliente, Fecha, Nombre, Folio, Estatus) " +
                    "VALUES (" +
                    "'" + txtAbono.Text + "'," +
                    "'" + lblID.Text + "'," +
                    "'" + DateTime.Now.ToShortDateString() + " " +
                    DateTime.Now.ToShortTimeString() + "'," +
                    "'" + lblCliente.Text + "'," +
                    "'" + folio + "'," +
                    "'PAGADO'" +
                    ");",
                    conectar);

                cmd.ExecuteNonQuery();

                // ====================================================
                // GENERAR RECIBO
                // ====================================================
                ReciboAbonoCarta reciboCarta = new ReciboAbonoCarta(
                    folio: folio,
                    idCliente: lblID.Text,
                    clienteNombre: lblCliente.Text,
                    adeudoAnterior: adeudoAnterior,
                    abono: abonoMonto,
                    saldoInsoluto: saldoInsoluto,
                    formaPago: cmbPago.Text,
                    nombreLugar: Conexion.lugar ?? "JOYERIA JS",
                    logoPath: Conexion.logoPath,
                    datosTicket: Conexion.datosTicket,
                    pieDeTicket: Conexion.pieDeTicket
                );

                reciboCarta.ImprimirDirectamente(Conexion.impresora);

                // ====================================================
                // MENSAJE
                // ====================================================
                MessageBox.Show(
                    "Se ha abonado con éxito",
                    "ÉXITO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // ====================================================
                // ABRIR HISTORIAL
                // ====================================================
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al registrar el abono:\n\n" + ex.Message,
                    "ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // AL CERRAR FORMULARIO
        // ============================================================
        private void frmAbonoCliente_FormClosing(
            object sender,
            FormClosingEventArgs e)
        {
        }
    }
}