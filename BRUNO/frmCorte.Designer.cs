namespace BRUNO
{
    partial class frmCorte
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCorte));

            // Paneles y Contenedores Principales
            this.panel2 = new System.Windows.Forms.Panel(); // Dashboard Izquierdo
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageEfectivo = new System.Windows.Forms.TabPage();
            this.tabPageTarjeta = new System.Windows.Forms.TabPage();
            this.tabPageTrans = new System.Windows.Forms.TabPage();
            this.tabPageAbonos = new System.Windows.Forms.TabPage();
            this.tabPageFolios = new System.Windows.Forms.TabPage();
            this.tabPageCategorias = new System.Windows.Forms.TabPage();
            this.panelOculto = new System.Windows.Forms.Panel(); // Para grids y labels que no se ven

            // Grids
            this.dgvCorte = new System.Windows.Forms.DataGridView();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            this.dataGridView5 = new System.Windows.Forms.DataGridView();
            this.dgvFolios = new System.Windows.Forms.DataGridView();
            this.dataGridView6 = new System.Windows.Forms.DataGridView();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();

            // Botones
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();

            // Labels Valores
            this.lblECaja = new System.Windows.Forms.Label();
            this.lblCorte = new System.Windows.Forms.Label();
            this.lblEntrada = new System.Windows.Forms.Label();
            this.lblSalida = new System.Windows.Forms.Label();
            this.lblCredito = new System.Windows.Forms.Label();
            this.lbl5por = new System.Windows.Forms.Label();
            this.lblTrans = new System.Windows.Forms.Label();
            this.lblNoIva = new System.Windows.Forms.Label();
            this.lblIVA = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblInversion = new System.Windows.Forms.Label();
            this.lblBruta = new System.Windows.Forms.Label();
            this.lblGastos = new System.Windows.Forms.Label();
            this.lblUtilidad = new System.Windows.Forms.Label();

            // Labels Titulos
            this.label17 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblComision = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();

            // Labels de cabeceras viejas (Se ocultan)
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();

            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageEfectivo.SuspendLayout();
            this.tabPageTarjeta.SuspendLayout();
            this.tabPageTrans.SuspendLayout();
            this.tabPageAbonos.SuspendLayout();
            this.tabPageFolios.SuspendLayout();
            this.tabPageCategorias.SuspendLayout();
            this.panelOculto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCorte)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolios)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();

            // =========================================================
            // TAB CONTROL (Sustituye la lista infinita hacia abajo)
            // =========================================================
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.tabControl1.Controls.Add(this.tabPageEfectivo);
            this.tabControl1.Controls.Add(this.tabPageTarjeta);
            this.tabControl1.Controls.Add(this.tabPageTrans);
            this.tabControl1.Controls.Add(this.tabPageAbonos);
            this.tabControl1.Controls.Add(this.tabPageFolios);
            this.tabControl1.Controls.Add(this.tabPageCategorias);
            this.tabControl1.ItemSize = new System.Drawing.Size(120, 40);

            // tabPageEfectivo
            this.tabPageEfectivo.Controls.Add(this.dgvCorte);
            this.tabPageEfectivo.Text = "Efectivo";
            this.tabPageEfectivo.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);

            // tabPageTarjeta
            this.tabPageTarjeta.Controls.Add(this.dataGridView3);
            this.tabPageTarjeta.Text = "Tarjetas";
            this.tabPageTarjeta.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);

            // tabPageTrans
            this.tabPageTrans.Controls.Add(this.dataGridView4);
            this.tabPageTrans.Text = "Transferencias";
            this.tabPageTrans.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);

            // tabPageAbonos
            this.tabPageAbonos.Controls.Add(this.dataGridView5);
            this.tabPageAbonos.Text = "Abonos";
            this.tabPageAbonos.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);

            // tabPageFolios
            this.tabPageFolios.Controls.Add(this.dgvFolios);
            this.tabPageFolios.Text = "Folios";
            this.tabPageFolios.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);

            // tabPageCategorias
            this.tabPageCategorias.Controls.Add(this.dataGridView6);
            this.tabPageCategorias.Text = "Por Categoría";
            this.tabPageCategorias.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);

            // =========================================================
            // GRIDS COMUNES (Estilo Moderno)
            // =========================================================
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 11F);

            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F);

            // dgvCorte
            this.dgvCorte.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCorte.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCorte.BackgroundColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.dgvCorte.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCorte.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCorte.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCorte.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvCorte_CellFormatting);

            // dataGridView3
            this.dataGridView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView3.BackgroundColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.dataGridView3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView3.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView3.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView3.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView3_CellFormatting);

            // dataGridView4
            this.dataGridView4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView4.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView4.BackgroundColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.dataGridView4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView4.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView4.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView4.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView4_CellFormatting);

            // dataGridView5
            this.dataGridView5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView5.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView5.BackgroundColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.dataGridView5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView5.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView5.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;

            // dgvFolios
            this.dgvFolios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFolios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFolios.BackgroundColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.dgvFolios.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFolios.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFolios.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFolios.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvFolios_CellFormatting);

            // dataGridView6
            this.dataGridView6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView6.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView6.BackgroundColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.dataGridView6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView6.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView6.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView6.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView6_CellFormatting);


            // =========================================================
            // PANEL IZQUIERDO (DASHBOARD)
            // =========================================================
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Width = 380;
            this.panel2.BackColor = System.Drawing.Color.FromArgb(20, 20, 20); // Gris muy oscuro
            this.panel2.Padding = new System.Windows.Forms.Padding(15);
            this.panel2.AutoScroll = true;

            // --- SECCIÓN EFECTIVO ---
            this.label17.Text = "Efectivo en Caja:";
            this.label17.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label17.ForeColor = System.Drawing.Color.Gray;
            this.label17.Location = new System.Drawing.Point(15, 20);
            this.label17.AutoSize = true;

            this.lblECaja.Text = "$0.00";
            this.lblECaja.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblECaja.ForeColor = System.Drawing.Color.White;
            this.lblECaja.Location = new System.Drawing.Point(200, 20);
            this.lblECaja.AutoSize = true;

            this.label3.Text = "Ventas Efectivo:";
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(15, 55);
            this.label3.AutoSize = true;

            this.lblCorte.Text = "$0.00";
            this.lblCorte.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblCorte.ForeColor = System.Drawing.Color.White;
            this.lblCorte.Location = new System.Drawing.Point(200, 55);
            this.lblCorte.AutoSize = true;

            this.label1.Text = "Entradas:";
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(15, 90);
            this.label1.AutoSize = true;

            this.lblEntrada.Text = "$0.00";
            this.lblEntrada.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblEntrada.ForeColor = System.Drawing.Color.FromArgb(46, 204, 113); // Verde
            this.lblEntrada.Location = new System.Drawing.Point(200, 90);
            this.lblEntrada.AutoSize = true;

            this.label2.Text = "Salidas:";
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(15, 125);
            this.label2.AutoSize = true;

            this.lblSalida.Text = "$0.00";
            this.lblSalida.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblSalida.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60); // Rojo
            this.lblSalida.Location = new System.Drawing.Point(200, 125);
            this.lblSalida.AutoSize = true;

            // --- LÍNEA DIVISORIA ---
            System.Windows.Forms.Label line1 = new System.Windows.Forms.Label();
            line1.AutoSize = false;
            line1.Height = 2;
            line1.Width = 350;
            line1.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            line1.Location = new System.Drawing.Point(15, 165);

            // --- SECCIÓN ELECTRÓNICOS ---
            this.label5.Text = "Tarjeta:";
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label5.ForeColor = System.Drawing.Color.Gray;
            this.label5.Location = new System.Drawing.Point(15, 180);
            this.label5.AutoSize = true;

            this.lblCredito.Text = "$0.00";
            this.lblCredito.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblCredito.ForeColor = System.Drawing.Color.White;
            this.lblCredito.Location = new System.Drawing.Point(200, 180);
            this.lblCredito.AutoSize = true;

            this.lblComision.Text = "2.7% Comisión:";
            this.lblComision.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblComision.ForeColor = System.Drawing.Color.Gray;
            this.lblComision.Location = new System.Drawing.Point(15, 215);
            this.lblComision.AutoSize = true;

            this.lbl5por.Text = "$0.00";
            this.lbl5por.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lbl5por.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.lbl5por.Location = new System.Drawing.Point(200, 215);
            this.lbl5por.AutoSize = true;

            this.label13.Text = "Transferencias:";
            this.label13.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label13.ForeColor = System.Drawing.Color.Gray;
            this.label13.Location = new System.Drawing.Point(15, 250);
            this.label13.AutoSize = true;

            this.lblTrans.Text = "$0.00";
            this.lblTrans.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTrans.ForeColor = System.Drawing.Color.White;
            this.lblTrans.Location = new System.Drawing.Point(200, 250);
            this.lblTrans.AutoSize = true;

            // --- LÍNEA DIVISORIA 2 ---
            System.Windows.Forms.Label line2 = new System.Windows.Forms.Label();
            line2.AutoSize = false;
            line2.Height = 2;
            line2.Width = 350;
            line2.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            line2.Location = new System.Drawing.Point(15, 290);

            // --- SECCIÓN TOTALES ---
            this.label19.Text = "Subtotal (Sin IVA):";
            this.label19.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label19.ForeColor = System.Drawing.Color.Gray;
            this.label19.Location = new System.Drawing.Point(15, 305);
            this.label19.AutoSize = true;

            this.lblNoIva.Text = "$0.00";
            this.lblNoIva.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblNoIva.ForeColor = System.Drawing.Color.White;
            this.lblNoIva.Location = new System.Drawing.Point(200, 305);
            this.lblNoIva.AutoSize = true;

            this.label21.Text = "IVA:";
            this.label21.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label21.ForeColor = System.Drawing.Color.Gray;
            this.label21.Location = new System.Drawing.Point(15, 340);
            this.label21.AutoSize = true;

            this.lblIVA.Text = "$0.00";
            this.lblIVA.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblIVA.ForeColor = System.Drawing.Color.White;
            this.lblIVA.Location = new System.Drawing.Point(200, 340);
            this.lblIVA.AutoSize = true;

            this.label8.Text = "TOTAL NETO:";
            this.label8.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(15, 385);
            this.label8.AutoSize = true;

            this.lblTotal.Text = "$0.00";
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(52, 152, 219); // Azul Fuerte
            this.lblTotal.Location = new System.Drawing.Point(200, 380);
            this.lblTotal.AutoSize = true;

            // --- LÍNEA DIVISORIA 3 ---
            System.Windows.Forms.Label line3 = new System.Windows.Forms.Label();
            line3.AutoSize = false;
            line3.Height = 2;
            line3.Width = 350;
            line3.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            line3.Location = new System.Drawing.Point(15, 430);

            // --- SECCIÓN RENDIMIENTO ---
            this.label9.Text = "Inversión:";
            this.label9.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label9.ForeColor = System.Drawing.Color.Gray;
            this.label9.Location = new System.Drawing.Point(15, 445);
            this.label9.AutoSize = true;

            this.lblInversion.Text = "$0.00";
            this.lblInversion.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblInversion.ForeColor = System.Drawing.Color.White;
            this.lblInversion.Location = new System.Drawing.Point(200, 445);
            this.lblInversion.AutoSize = true;

            this.label14.Text = "Utilidad Bruta:";
            this.label14.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label14.ForeColor = System.Drawing.Color.Gray;
            this.label14.Location = new System.Drawing.Point(15, 480);
            this.label14.AutoSize = true;

            this.lblBruta.Text = "$0.00";
            this.lblBruta.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblBruta.ForeColor = System.Drawing.Color.White;
            this.lblBruta.Location = new System.Drawing.Point(200, 480);
            this.lblBruta.AutoSize = true;

            this.label10.Text = "Gastos:";
            this.label10.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label10.ForeColor = System.Drawing.Color.Gray;
            this.label10.Location = new System.Drawing.Point(15, 515);
            this.label10.AutoSize = true;

            this.lblGastos.Text = "$0.00";
            this.lblGastos.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblGastos.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.lblGastos.Location = new System.Drawing.Point(200, 515);
            this.lblGastos.AutoSize = true;

            this.label11.Text = "UTILIDAD NETA:";
            this.label11.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(15, 560);
            this.label11.AutoSize = true;

            this.lblUtilidad.Text = "$0.00";
            this.lblUtilidad.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblUtilidad.ForeColor = System.Drawing.Color.FromArgb(46, 204, 113); // Verde
            this.lblUtilidad.Location = new System.Drawing.Point(200, 555);
            this.lblUtilidad.AutoSize = true;

            // --- BOTON REALIZAR CORTE ---
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Height = 70;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.button1.BackColor = System.Drawing.Color.FromArgb(46, 204, 113); // Verde Atractivo
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.Text = "CERRAR E IMPRIMIR CORTE";
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.Click += new System.EventHandler(this.button1_Click);

            // Agregar al panel 2
            this.panel2.Controls.Add(this.label17);
            this.panel2.Controls.Add(this.lblECaja);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.lblCorte);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.lblEntrada);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.lblSalida);
            this.panel2.Controls.Add(line1);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.lblCredito);
            this.panel2.Controls.Add(this.lblComision);
            this.panel2.Controls.Add(this.lbl5por);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.lblTrans);
            this.panel2.Controls.Add(line2);
            this.panel2.Controls.Add(this.label19);
            this.panel2.Controls.Add(this.lblNoIva);
            this.panel2.Controls.Add(this.label21);
            this.panel2.Controls.Add(this.lblIVA);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.lblTotal);
            this.panel2.Controls.Add(line3);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.lblInversion);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.lblBruta);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.lblGastos);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.lblUtilidad);
            this.panel2.Controls.Add(this.button1);

            // =========================================================
            // CONTENEDOR OCULTO PARA CABECERAS VIEJAS Y GRIDS NO USADOS
            // =========================================================
            this.panelOculto.Visible = false;
            this.panelOculto.Controls.Add(this.dataGridView1);
            this.panelOculto.Controls.Add(this.dataGridView2);
            this.panelOculto.Controls.Add(this.button2); // Arqueo
            this.panelOculto.Controls.Add(this.label4);
            this.panelOculto.Controls.Add(this.label6);
            this.panelOculto.Controls.Add(this.label7);
            this.panelOculto.Controls.Add(this.label12);
            this.panelOculto.Controls.Add(this.label15);
            this.panelOculto.Controls.Add(this.label16);
            this.panelOculto.Controls.Add(this.panel1);

            // =========================================================
            // FORMULARIO PRINCIPAL
            // =========================================================
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(45, 45, 48); // Fondo moderno
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.Controls.Add(this.tabControl1); // Ocupa la derecha
            this.Controls.Add(this.panel2);      // Ocupa la izquierda
            this.Controls.Add(this.panelOculto); // Basura vieja oculta
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCorte";
            this.Text = "Corte de Caja";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmCorte_Load);

            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageEfectivo.ResumeLayout(false);
            this.tabPageTarjeta.ResumeLayout(false);
            this.tabPageTrans.ResumeLayout(false);
            this.tabPageAbonos.ResumeLayout(false);
            this.tabPageFolios.ResumeLayout(false);
            this.tabPageCategorias.ResumeLayout(false);
            this.panelOculto.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCorte)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolios)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        // Elementos UI Nuevos
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageEfectivo;
        private System.Windows.Forms.TabPage tabPageTarjeta;
        private System.Windows.Forms.TabPage tabPageTrans;
        private System.Windows.Forms.TabPage tabPageAbonos;
        private System.Windows.Forms.TabPage tabPageFolios;
        private System.Windows.Forms.TabPage tabPageCategorias;
        private System.Windows.Forms.Panel panelOculto;

        // Elementos Mantenidos (Para no romper el código trasero)
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.Label lblCredito;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCorte;
        private System.Windows.Forms.Label lblSalida;
        private System.Windows.Forms.Label lblEntrada;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dgvCorte;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lblInversion;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblUtilidad;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Label lblGastos;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblBruta;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dataGridView4;
        private System.Windows.Forms.Label lblTrans;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DataGridView dataGridView5;
        private System.Windows.Forms.Label lbl5por;
        private System.Windows.Forms.Label lblComision;
        private System.Windows.Forms.Label lblECaja;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridView dgvFolios;
        private System.Windows.Forms.Label lblNoIva;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblIVA;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DataGridView dataGridView6;
    }
}