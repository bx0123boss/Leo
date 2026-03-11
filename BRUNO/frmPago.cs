using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmPago : Form
    {
        public double cambio { get; set; }
        public double efectivo { get; set; }
        public double total { get; set; }

        // El Diccionario donde guardaremos cada pago separado
        public Dictionary<string, double> PagosRealizados { get; set; }

        public frmPago()
        {
            InitializeComponent();
            PagosRealizados = new Dictionary<string, double>();
        }

        private void frmPago_Load(object sender, EventArgs e)
        {
            txtTotal.Text = $"{total:C}";
            txtRestante.Text = $"{total:C}";
            textBox3.Text = "$0.00";

            string[] opcionesPago = {
                "01=EFECTIVO",
                "02=CHEQUE NOMINATIVO",
                "03=TRANFERENCIA ELECTRONICA DE FONDOS",
                "04=TARJETA DE CREDITO",
                "28=TARJETA DE DEBITO",
                "05=MONEDERO ELECTRONICO",
                "06=DINERO ELECTRONICO",
                "08=VALES DE DESPENSA",
                "99=POR DEFINIR"
            };
            cmbPago.Items.AddRange(opcionesPago);
            cmbPago.SelectedIndex = 0;

            // Forzar el diseño Normal al abrir
            chkMixto.Checked = false;
            ConfigurarDiseno();
        }

        private void chkMixto_CheckedChanged(object sender, EventArgs e)
        {
            ConfigurarDiseno();
        }

        // Esta función encoge o agranda el formulario
        private void ConfigurarDiseno()
        {
            bool mixto = chkMixto.Checked;

            lblMetodo.Visible = mixto;
            cmbPago.Visible = mixto;
            btnAgregar.Visible = mixto;
            dgvPagos.Visible = mixto;
            btnLimpiar.Visible = mixto;
            lblRestante.Visible = mixto;
            txtRestante.Visible = mixto;

            if (mixto)
            {
                // --- MODO MIXTO (Formulario Grande) ---
                this.Height = 480;

                label2.Top = 115;
                textBox2.Top = 109;
                label2.Text = "MONTO:";

                label3.Top = 365;
                textBox3.Top = 361;
                button1.Top = 415;
                btnAceptar.Top = 415;

                PagosRealizados.Clear();
                ActualizarPagos();
                cmbPago.SelectedIndex = 0;
                textBox2.Clear();
                textBox2.Focus();
            }
            else
            {
                // --- MODO ORIGINAL (Formulario Pequeño) ---
                this.Height = 230;

                label2.Top = 65;
                textBox2.Top = 59;
                label2.Text = "EFECTIVO:";

                label3.Top = 115;
                textBox3.Top = 109;
                button1.Top = 165;
                btnAceptar.Top = 165;

                PagosRealizados.Clear();
                textBox2.Clear();
                textBox3.Text = "$0.00";
                efectivo = 0;
                cambio = 0;
                textBox2.Focus();
            }
        }

        private void cmbPago_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox2.Focus();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo permitir números y punto decimal
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && e.KeyChar != '.')
            {
                e.Handled = true;
                return;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (chkMixto.Checked)
                {
                    AgregarMonto();
                }
                else
                {
                    // LÓGICA DE COBRO NORMAL (Original)
                    if (string.IsNullOrWhiteSpace(textBox2.Text))
                    {
                        MessageBox.Show("Ingresa una cantidad valida", "ALTO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    double capturado = Convert.ToDouble(textBox2.Text);

                    if (capturado < total)
                    {
                        textBox2.Clear();
                        MessageBox.Show("Ingresa una cantidad valida", "ALTO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        efectivo = capturado;
                        cambio = capturado - total;
                        textBox3.Text = $"{cambio:C}";
                        textBox2.Text = $"{capturado:C}";

                        btnAceptar.Focus(); // Pasa el foco al botón Aceptar
                    }
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AgregarMonto();
        }

        private void AgregarMonto()
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text)) return;

            double montoAgregado = Convert.ToDouble(textBox2.Text);
            string metodo = cmbPago.Text;

            double sumaActual = PagosRealizados.Values.Sum();
            double restante = total - sumaActual;

            if (montoAgregado > 0)
            {
                // Regla: Si no es Efectivo, no puede exceder el límite
                if (!metodo.Contains("EFECTIVO") && montoAgregado > restante)
                {
                    MessageBox.Show($"El pago con {metodo} no puede ser mayor al saldo restante ({restante:C}).\n\nSolo el Efectivo puede generar cambio.",
                                    "Monto Excedido", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    textBox2.SelectAll();
                    textBox2.Focus();
                    return;
                }

                if (PagosRealizados.ContainsKey(metodo))
                {
                    PagosRealizados[metodo] += montoAgregado;
                }
                else
                {
                    PagosRealizados.Add(metodo, montoAgregado);
                }
            }

            ActualizarPagos();
            textBox2.Clear();
        }

        private void ActualizarPagos()
        {
            dgvPagos.Rows.Clear();
            double suma = 0;
            efectivo = 0;

            foreach (var pago in PagosRealizados)
            {
                dgvPagos.Rows.Add(pago.Key, $"{pago.Value:C}");
                suma += pago.Value;

                if (pago.Key.Contains("EFECTIVO"))
                {
                    efectivo += pago.Value;
                }
            }

            double restante = total - suma;

            if (restante <= 0)
            {
                txtRestante.Text = "$0.00";
                cambio = Math.Abs(restante);
                textBox3.Text = $"{cambio:C}";

                btnAceptar.Focus();
            }
            else
            {
                txtRestante.Text = $"{restante:C}";
                cambio = 0;
                textBox3.Text = "$0.00";
                cmbPago.Focus();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            PagosRealizados.Clear();
            ActualizarPagos();
            textBox2.Focus();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!btnAceptar.Enabled) return; // Anti-Doble Clic

            if (chkMixto.Checked)
            {
                // FINALIZACIÓN MODO MIXTO
                double suma = PagosRealizados.Values.Sum();
                if (suma >= total)
                {
                    btnAceptar.Enabled = false;

                    if (cambio > 0)
                    {
                        string llaveEfectivo = PagosRealizados.Keys.FirstOrDefault(k => k.Contains("EFECTIVO"));
                        if (llaveEfectivo != null)
                        {
                            PagosRealizados[llaveEfectivo] -= cambio;
                            cambio = 0;
                        }
                    }
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Falta cubrir el total de la venta.", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                // FINALIZACIÓN MODO ORIGINAL
                if (efectivo >= total)
                {
                    btnAceptar.Enabled = false;
                    PagosRealizados.Clear();

                    // Emulamos el diccionario para que frmVentas lo procese igual de fácil
                    PagosRealizados.Add("01=EFECTIVO", total);

                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Aún no ha ingresado el efectivo correcto.", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Focus();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Botón: Seguir Cobrando / Cancelar
            this.DialogResult = DialogResult.Cancel;
        }
    }
}