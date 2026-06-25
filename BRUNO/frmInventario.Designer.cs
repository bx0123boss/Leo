namespace JaegerSoft
{
    partial class frmInventario
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInventario));

            // ── Declaraciones ──────────────────────────────────────────────
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.BtnApartados = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btnAlmacenes = new System.Windows.Forms.Button();
            this.lblOperaciones = new System.Windows.Forms.Label();
            this.lblCatalogos = new System.Windows.Forms.Label();
            this.button11 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.lblReportes = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button19 = new System.Windows.Forms.Button();
            this.lblEstado = new System.Windows.Forms.Label();
            this.btnPrimero = new System.Windows.Forms.Button();
            this.btnAnterior = new System.Windows.Forms.Button();
            this.btnUltimo = new System.Windows.Forms.Button();
            this.btnSiguiente = new System.Windows.Forms.Button();

            // Header
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelFiltros = new System.Windows.Forms.TableLayoutPanel();

            // KPI container → FlowLayoutPanel para que las tarjetas fluyan
            this.panelKpiContainer = new System.Windows.Forms.FlowLayoutPanel();

            // Tarjetas KPI
            this.cardCategorias = new System.Windows.Forms.Panel();
            this.lblKpiCategoriasTitulo = new System.Windows.Forms.Label();
            this.lblKpiCategorias = new System.Windows.Forms.Label();
            this.cardArticulos = new System.Windows.Forms.Panel();
            this.lblKpiArticulosTitulo = new System.Windows.Forms.Label();
            this.lblKpiArticulos = new System.Windows.Forms.Label();
            this.cardStock = new System.Windows.Forms.Panel();
            this.lblKpiStockTitulo = new System.Windows.Forms.Label();
            this.lblKpiStock = new System.Windows.Forms.Label();
            this.cardInversion = new System.Windows.Forms.Panel();
            this.lblKpiInversionTitulo = new System.Windows.Forms.Label();
            this.lblKpiInversion = new System.Windows.Forms.Label();
            this.cardVentaPotencial = new System.Windows.Forms.Panel();
            this.lblKpiVentaPotencialTitulo = new System.Windows.Forms.Label();
            this.lblKpiVentaPotencial = new System.Windows.Forms.Label();
            this.cardUtilidad = new System.Windows.Forms.Panel();
            this.lblKpiUtilidadTitulo = new System.Windows.Forms.Label();
            this.lblKpiUtilidad = new System.Windows.Forms.Label();
            this.cardMargen = new System.Windows.Forms.Panel();
            this.lblKpiMargenTitulo = new System.Windows.Forms.Label();
            this.lblKpiMargen = new System.Windows.Forms.Label();
            this.cardStockCritico = new System.Windows.Forms.Panel();
            this.lblKpiStockCriticoTitulo = new System.Windows.Forms.Label();
            this.lblKpiStockCritico = new System.Windows.Forms.Label();
            this.cardCapitalInmovilizado = new System.Windows.Forms.Panel();
            this.lblKpiCapitalInmovilizadoTitulo = new System.Windows.Forms.Label();
            this.lblKpiCapitalInmovilizado = new System.Windows.Forms.Label();

            // Panel analítico → Panel estándar con contenedor para scroll
            this.panelAnaliticoWrapper = new System.Windows.Forms.Panel();
            this.panelAnalitico = new System.Windows.Forms.Panel();
            this.lblFinancieroTitulo = new System.Windows.Forms.Label();
            this.lblFinancieroInversion = new System.Windows.Forms.Label();
            this.lblFinancieroVenta = new System.Windows.Forms.Label();
            this.lblFinancieroUtilidad = new System.Windows.Forms.Label();
            this.lblFinancieroMargen = new System.Windows.Forms.Label();
            this.lblFinancieroInmovilizado = new System.Windows.Forms.Label();
            this.lblRiesgoTitulo = new System.Windows.Forms.Label();
            this.lblRiesgoCriticos = new System.Windows.Forms.Label();
            this.lblRiesgoSobrestock = new System.Windows.Forms.Label();
            this.lblRiesgoRotacion = new System.Windows.Forms.Label();
            this.lblRiesgoReposicion = new System.Windows.Forms.Label();

            this.panelPaginador = new System.Windows.Forms.Panel();

            // ── Begin init ─────────────────────────────────────────────────
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.panel6.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelFiltros.SuspendLayout();
            this.panelKpiContainer.SuspendLayout();
            this.cardArticulos.SuspendLayout();
            this.cardStock.SuspendLayout();
            this.cardCategorias.SuspendLayout();
            this.cardInversion.SuspendLayout();
            this.cardVentaPotencial.SuspendLayout();
            this.cardUtilidad.SuspendLayout();
            this.cardMargen.SuspendLayout();
            this.cardStockCritico.SuspendLayout();
            this.cardCapitalInmovilizado.SuspendLayout();
            this.panelPaginador.SuspendLayout();
            this.panelAnaliticoWrapper.SuspendLayout();
            this.panelAnalitico.SuspendLayout();
            this.SuspendLayout();

            // ═══════════════════════════════════════════════════════════════
            // dataGridView2
            // ═══════════════════════════════════════════════════════════════
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToOrderColumns = true;
            this.dataGridView2.Anchor = System.Windows.Forms.AnchorStyles.Top
                                     | System.Windows.Forms.AnchorStyles.Bottom
                                     | System.Windows.Forms.AnchorStyles.Left
                                     | System.Windows.Forms.AnchorStyles.Right;
            this.dataGridView2.BackgroundColor = System.Drawing.Color.FromArgb(20, 20, 20);
            this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.EnableHeadersVisualStyles = false;
            this.dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(35, 35, 35);
            this.dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.dataGridView2.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.dataGridView2.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(28, 28, 28);
            this.dataGridView2.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.dataGridView2.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dataGridView2.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.dataGridView2.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridView2.GridColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.dataGridView2.Location = new System.Drawing.Point(10, 290);
            this.dataGridView2.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.RowTemplate.Height = 24;
            this.dataGridView2.Size = new System.Drawing.Size(1293, 640);
            this.dataGridView2.TabIndex = 26;
            this.dataGridView2.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView2_CellFormatting);

            // ═══════════════════════════════════════════════════════════════
            // PANEL HEADER (toda la barra superior)
            // ═══════════════════════════════════════════════════════════════
            this.panelHeader.Anchor = System.Windows.Forms.AnchorStyles.Top
                                       | System.Windows.Forms.AnchorStyles.Left
                                       | System.Windows.Forms.AnchorStyles.Right;
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(28, 28, 28);
            this.panelHeader.Controls.Add(this.panelFiltros);
            this.panelHeader.Controls.Add(this.panelKpiContainer);
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Padding = new System.Windows.Forms.Padding(0);
            this.panelHeader.Size = new System.Drawing.Size(1530, 200);
            this.panelHeader.TabIndex = 0;

            // ───────────────────────────────────────────────────────────────
            // panelFiltros  (TableLayoutPanel — lado izquierdo fijo ~510 px)
            // ───────────────────────────────────────────────────────────────
            this.panelFiltros.Anchor = System.Windows.Forms.AnchorStyles.Top
                                          | System.Windows.Forms.AnchorStyles.Bottom
                                          | System.Windows.Forms.AnchorStyles.Left;
            this.panelFiltros.BackColor = System.Drawing.Color.Transparent;
            this.panelFiltros.ColumnCount = 2;
            this.panelFiltros.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 165F));
            this.panelFiltros.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 235F));
            this.panelFiltros.RowCount = 4;
            this.panelFiltros.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.panelFiltros.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.panelFiltros.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.panelFiltros.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelFiltros.Location = new System.Drawing.Point(0, 0);
            this.panelFiltros.Name = "panelFiltros";
            this.panelFiltros.Padding = new System.Windows.Forms.Padding(12, 8, 8, 4);
            this.panelFiltros.Size = new System.Drawing.Size(400, 200);
            this.panelFiltros.TabIndex = 0;

            // label3 "Buscar por Nombre:"
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.label3.Name = "label3";
            this.label3.Text = "Buscar por Nombre:";
            this.panelFiltros.Controls.Add(this.label3, 0, 0);

            // textBox1
            this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.TabIndex = 1;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            this.panelFiltros.Controls.Add(this.textBox1, 1, 0);

            // label14 "Buscar por ID:"
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.label14.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.label14.Name = "label14";
            this.label14.Text = "Buscar por ID:";
            this.panelFiltros.Controls.Add(this.label14, 0, 1);

            // textBox2
            this.textBox2.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.textBox2.ForeColor = System.Drawing.Color.White;
            this.textBox2.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.textBox2.Name = "textBox2";
            this.textBox2.TabIndex = 2;
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress);
            this.panelFiltros.Controls.Add(this.textBox2, 1, 1);

            // label5 "Categorías:"
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.label5.Name = "label5";
            this.label5.Text = "Categorías:";
            this.panelFiltros.Controls.Add(this.label5, 0, 2);

            // comboBox2
            this.comboBox2.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            this.comboBox2.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.comboBox2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox2.ForeColor = System.Drawing.Color.White;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.ItemHeight = 21;
            this.comboBox2.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.TabIndex = 3;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            this.panelFiltros.Controls.Add(this.comboBox2, 1, 2);

            // checkBox1
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.checkBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Ver Categoría";
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            this.panelFiltros.Controls.Add(this.checkBox1, 1, 3);

            // ═══════════════════════════════════════════════════════════════
            // panelKpiContainer  → FlowLayoutPanel, ocupa el resto del header
            // ═══════════════════════════════════════════════════════════════
            this.panelKpiContainer.Anchor = System.Windows.Forms.AnchorStyles.Top
                                                | System.Windows.Forms.AnchorStyles.Bottom
                                                | System.Windows.Forms.AnchorStyles.Left
                                                | System.Windows.Forms.AnchorStyles.Right;
            this.panelKpiContainer.AutoScroll = true;
            this.panelKpiContainer.BackColor = System.Drawing.Color.Transparent;
            this.panelKpiContainer.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.panelKpiContainer.WrapContents = true;
            this.panelKpiContainer.Padding = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.panelKpiContainer.Location = new System.Drawing.Point(400, 0);
            this.panelKpiContainer.Name = "panelKpiContainer";
            this.panelKpiContainer.Size = new System.Drawing.Size(1130, 200);
            this.panelKpiContainer.TabIndex = 57;

            // ───────────────────────────────────────────────────────────────
            // Helper local: aplica estilo uniforme a cada tarjeta KPI
            // ───────────────────────────────────────────────────────────────
            System.Windows.Forms.Padding cardMarginVal = new System.Windows.Forms.Padding(4, 4, 4, 4);
            System.Drawing.Size cardSz = new System.Drawing.Size(175, 95);
            System.Drawing.Color cardBg = System.Drawing.Color.FromArgb(40, 40, 40);

            void ConfigCard(System.Windows.Forms.Panel card,
                            System.Windows.Forms.Label titulo,
                            System.Windows.Forms.Label valor,
                            string tituloText, string valorText,
                            bool isMoney = false)
            {
                card.AutoScroll = true;
                card.BackColor = cardBg;
                card.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                card.Margin = cardMarginVal;
                card.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
                card.Size = cardSz;

                titulo.AutoSize = true;
                titulo.Location = new System.Drawing.Point(8, 6);
                titulo.Font = new System.Drawing.Font("Segoe UI Semibold", 7.5F, System.Drawing.FontStyle.Bold);
                titulo.ForeColor = System.Drawing.Color.FromArgb(130, 130, 130);
                titulo.Text = tituloText;

                valor.AutoSize = true;
                valor.Location = new System.Drawing.Point(8, 28);
                valor.Font = isMoney
                    ? new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold)
                    : new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
                valor.ForeColor = System.Drawing.Color.White;
                valor.Text = valorText;
                valor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                card.Controls.Add(valor);
                card.Controls.Add(titulo);
            }

            // Tarjetas conteo (fila 1)
            ConfigCard(cardCategorias, lblKpiCategoriasTitulo, lblKpiCategorias, "CATEGORÍAS ACTIVAS", "0");
            ConfigCard(cardArticulos, lblKpiArticulosTitulo, lblKpiArticulos, "ARTÍCULOS CATALOGADOS", "0");
            ConfigCard(cardStock, lblKpiStockTitulo, lblKpiStock, "EXISTENCIA GLOBAL", "0");
            ConfigCard(cardMargen, lblKpiMargenTitulo, lblKpiMargen, "MARGEN", "0 %");

            // Tarjetas monetarias (fila 2 / wrap)
            ConfigCard(cardInversion, lblKpiInversionTitulo, lblKpiInversion, "INVERSIÓN ACTUAL", "$0.00", true);
            ConfigCard(cardVentaPotencial, lblKpiVentaPotencialTitulo, lblKpiVentaPotencial, "VENTA POTENCIAL", "$0.00", true);
            ConfigCard(cardUtilidad, lblKpiUtilidadTitulo, lblKpiUtilidad, "UTILIDAD ESPERADA", "$0.00", true);
            ConfigCard(cardStockCritico, lblKpiStockCriticoTitulo, lblKpiStockCritico, "STOCK CRÍTICO", "0 PROD.", true);
            ConfigCard(cardCapitalInmovilizado, lblKpiCapitalInmovilizadoTitulo, lblKpiCapitalInmovilizado,
                       "CAPITAL INMOVILIZADO", "$0.00", true);

            // Agregar tarjetas al FlowLayout (orden visible)
            this.panelKpiContainer.Controls.AddRange(new System.Windows.Forms.Control[] {
                cardCategorias, cardArticulos, cardStock, cardMargen,
                cardInversion, cardVentaPotencial, cardUtilidad, cardStockCritico, cardCapitalInmovilizado
            });

            // ═══════════════════════════════════════════════════════════════
            // panelAnaliticoWrapper  → Contenedor con Scroll
            // ═══════════════════════════════════════════════════════════════
            this.panelAnaliticoWrapper.Anchor = System.Windows.Forms.AnchorStyles.Top
                                            | System.Windows.Forms.AnchorStyles.Left
                                            | System.Windows.Forms.AnchorStyles.Right;
            this.panelAnaliticoWrapper.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
            this.panelAnaliticoWrapper.Location = new System.Drawing.Point(0, 200);
            this.panelAnaliticoWrapper.Name = "panelAnaliticoWrapper";
            this.panelAnaliticoWrapper.Size = new System.Drawing.Size(1530, 90);
            this.panelAnaliticoWrapper.TabIndex = 10;
            this.panelAnaliticoWrapper.AutoScroll = true;
            this.panelAnaliticoWrapper.Controls.Add(this.panelAnalitico);

            // ═══════════════════════════════════════════════════════════════
            // panelAnalitico  → Panel interno de contenido fijo/mínimo
            // ═══════════════════════════════════════════════════════════════
            this.panelAnalitico.Anchor = System.Windows.Forms.AnchorStyles.Top
                                            | System.Windows.Forms.AnchorStyles.Left
                                            | System.Windows.Forms.AnchorStyles.Right;
            this.panelAnalitico.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
            this.panelAnalitico.Location = new System.Drawing.Point(0, 0);
            this.panelAnalitico.Name = "panelAnalitico";
            this.panelAnalitico.Size = new System.Drawing.Size(1530, 72);
            this.panelAnalitico.MinimumSize = new System.Drawing.Size(1300, 72);
            this.panelAnalitico.Padding = new System.Windows.Forms.Padding(14, 6, 14, 4);
            this.panelAnalitico.TabIndex = 0;
            
            // Vincular visibilidad de panelAnalitico a panelAnaliticoWrapper
            this.panelAnalitico.VisibleChanged += (sender, e) => {
                this.panelAnaliticoWrapper.Visible = this.panelAnalitico.Visible;
            };

            System.Drawing.Font fAnalitico = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);

            void AnLabel(System.Windows.Forms.Label lbl, string txt,
                         bool isTitle, int col, int row, int colSpan = 1)
            {
                lbl.AutoSize = true;
                lbl.Font = isTitle
                    ? new System.Drawing.Font("Segoe UI Semibold", 7.5F, System.Drawing.FontStyle.Bold)
                    : fAnalitico;
                lbl.ForeColor = isTitle
                    ? System.Drawing.Color.FromArgb(120, 120, 120)
                    : System.Drawing.Color.FromArgb(210, 210, 210);
                lbl.Text = txt;
                lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                
                int x = 14 + (col * 230);
                int y = 6 + (row * 24);
                lbl.Location = new System.Drawing.Point(x, y);

                this.panelAnalitico.Controls.Add(lbl);
            }

            // Fila 0: títulos de sección
            AnLabel(lblFinancieroTitulo, "▸ INVENTARIO FINANCIERO", true, 0, 0, 3);
            AnLabel(lblRiesgoTitulo, "▸ RIESGO OPERATIVO", true, 3, 0, 3);

            // Fila 1: datos financieros
            AnLabel(lblFinancieroInversion, "Inversión: $0.00", false, 0, 1);
            AnLabel(lblFinancieroVenta, "Venta Est.: $0.00", false, 1, 1);
            AnLabel(lblFinancieroUtilidad, "Ganancia: $0.00", false, 2, 1);

            // Fila 2: datos financieros continuación + riesgo
            AnLabel(lblFinancieroMargen, "Margen: 0 %", false, 0, 2);
            AnLabel(lblFinancieroInmovilizado, "Inmovilizado: $0.00", false, 1, 2);
            AnLabel(lblRiesgoCriticos, "Críticos: 0 Prod.", false, 3, 1);
            AnLabel(lblRiesgoSobrestock, "Sobrestock: 0 Prod.", false, 4, 1);
            AnLabel(lblRiesgoRotacion, "Baja Rotación: 0 Prod.", false, 3, 2);
            AnLabel(lblRiesgoReposicion, "Reposición: 0 Prod.", false, 4, 2);

            // ═══════════════════════════════════════════════════════════════
            // panel6 — barra lateral derecha (acciones / reportes)
            // ═══════════════════════════════════════════════════════════════
            this.panel6.Anchor = System.Windows.Forms.AnchorStyles.Top
                                   | System.Windows.Forms.AnchorStyles.Bottom
                                   | System.Windows.Forms.AnchorStyles.Right;
            this.panel6.AutoScroll = true;
            this.panel6.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
            this.panel6.Location = new System.Drawing.Point(1311, 290);
            this.panel6.Margin = new System.Windows.Forms.Padding(4);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.panel6.Size = new System.Drawing.Size(219, 695);
            this.panel6.TabIndex = 50;

            // Botones de la barra lateral: helper
            int btnY = 0;
            int btnW = 199;
            int btnH = 36;
            int sep = 4;

            System.Windows.Forms.Button SideBtn(System.Windows.Forms.Button b,
                                                string name, string text,
                                                System.Drawing.Color bg, System.Drawing.Color hover,
                                                bool hasBorder = false)
            {
                b.BackColor = bg;
                b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                b.FlatAppearance.BorderSize = hasBorder ? 1 : 0;
                b.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(60, 60, 60);
                b.FlatAppearance.MouseOverBackColor = hover;
                b.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
                b.ForeColor = System.Drawing.Color.White;
                b.Location = new System.Drawing.Point(10, btnY);
                b.Name = name;
                b.Size = new System.Drawing.Size(btnW, btnH);
                b.Text = text;
                b.Cursor = System.Windows.Forms.Cursors.Hand;
                b.UseVisualStyleBackColor = false;
                btnY += btnH + sep;
                return b;
            }

            System.Drawing.Color cAccion = System.Drawing.Color.FromArgb(52, 73, 94);
            System.Drawing.Color cHovA = System.Drawing.Color.FromArgb(44, 62, 80);
            System.Drawing.Color cAzul = System.Drawing.Color.FromArgb(41, 128, 185);
            System.Drawing.Color cHovAzul = System.Drawing.Color.FromArgb(52, 152, 219);
            System.Drawing.Color cDark = System.Drawing.Color.FromArgb(40, 40, 40);
            System.Drawing.Color cHovD = System.Drawing.Color.FromArgb(55, 55, 55);

            // — ACCIONES DE PRODUCTO —
            lblOperaciones.AutoSize = true;
            lblOperaciones.Font = new System.Drawing.Font("Segoe UI Semibold", 7F, System.Drawing.FontStyle.Bold);
            lblOperaciones.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            lblOperaciones.Location = new System.Drawing.Point(10, btnY);
            lblOperaciones.Name = "lblOperaciones";
            lblOperaciones.Text = "ACCIONES DE PRODUCTO";
            btnY += 18;

            SideBtn(BtnApartados, "BtnApartados", "➕  AGREGAR",
                    System.Drawing.Color.FromArgb(39, 174, 96),
                    System.Drawing.Color.FromArgb(30, 150, 80));
            BtnApartados.Click += new System.EventHandler(this.BtnApartados_Click);

            SideBtn(button2, "button2", "✏  EDITAR",
                    System.Drawing.Color.FromArgb(211, 84, 0),
                    System.Drawing.Color.FromArgb(230, 126, 34));
            button2.Click += new System.EventHandler(this.button2_Click);

            SideBtn(button1, "button1", "🗑  ELIMINAR",
                    System.Drawing.Color.FromArgb(192, 57, 43),
                    System.Drawing.Color.FromArgb(231, 76, 60));
            button1.Click += new System.EventHandler(this.button1_Click);

            btnY += 6; // separador visual
            // — PARÁMETROS / CATÁLOGOS —
            lblCatalogos.AutoSize = true;
            lblCatalogos.Font = new System.Drawing.Font("Segoe UI Semibold", 7F, System.Drawing.FontStyle.Bold);
            lblCatalogos.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            lblCatalogos.Location = new System.Drawing.Point(10, btnY);
            lblCatalogos.Name = "lblCatalogos";
            lblCatalogos.Text = "PARÁMETROS / CATÁLOGOS";
            btnY += 18;

            SideBtn(button11, "button11", "CATEGORÍAS", cDark, cHovD, true);
            button11.Click += new System.EventHandler(this.button11_Click);
            SideBtn(button10, "button10", "UNIDADES", cDark, cHovD, true);
            button10.Click += new System.EventHandler(this.button10_Click);
            SideBtn(button13, "button13", "AJUSTE FÍSICO", cDark, cHovD, true);
            button13.Click += new System.EventHandler(this.button13_Click);
            SideBtn(button12, "button12", "SUSPENDIDOS", cDark, cHovD, true);
            button12.Click += new System.EventHandler(this.button12_Click);
            SideBtn(btnAlmacenes, "btnAlmacenes", "ALMACENES", cDark, cHovD, true);
            btnAlmacenes.Click += new System.EventHandler(this.btnAlmacenes_Click);

            btnY += 6;
            // — REPORTES / CONSULTAS —
            lblReportes.AutoSize = true;
            lblReportes.Font = new System.Drawing.Font("Segoe UI Semibold", 7F, System.Drawing.FontStyle.Bold);
            lblReportes.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            lblReportes.Location = new System.Drawing.Point(10, btnY);
            lblReportes.Name = "lblReportes";
            lblReportes.Text = "REPORTES / CONSULTAS";
            btnY += 18;

            SideBtn(button7, "button7", "HISTORIAL KARDEX", cAzul, cHovAzul);
            button7.Click += new System.EventHandler(this.button7_Click);
            SideBtn(button8, "button8", "PÓLIZAS COMPRAS", cAzul, cHovAzul);
            button8.Click += new System.EventHandler(this.button8_Click);
            SideBtn(button4, "button4", "GASTOS EXTRA", cAzul, cHovAzul);
            button4.Click += new System.EventHandler(this.button4_Click_1);
            SideBtn(button14, "button14", "EXPORTAR EXCEL", cAzul, cHovAzul);
            button14.Click += new System.EventHandler(this.ExportarInventarioExcel);
            SideBtn(button6, "button6", "TOTALES", cAzul, cHovAzul);
            button6.Click += new System.EventHandler(this.button6_Click);
            SideBtn(button18, "button18", "AJUSTES TARJETA", cDark, cHovD, true);
            button18.Click += new System.EventHandler(this.button18_Click);

            // Botones ocultos (sin cambios funcionales)
            button16.Location = new System.Drawing.Point(10, btnY + 40); button16.Size = new System.Drawing.Size(btnW, btnH);
            button16.Name = "button16"; button16.Text = "Captura"; button16.Visible = false;
            button16.Click += new System.EventHandler(this.button16_Click);

            button17.Location = new System.Drawing.Point(10, btnY + 80); button17.Size = new System.Drawing.Size(btnW, btnH);
            button17.Name = "button17"; button17.Text = "AGREGAR EXISTENCIAS"; button17.Visible = false;
            button17.Click += new System.EventHandler(this.button17_Click);

            button15.Location = new System.Drawing.Point(10, btnY + 120); button15.Size = new System.Drawing.Size(btnW, btnH);
            button15.Name = "button15"; button15.Text = "Revisar límites"; button15.Visible = false;
            button15.Click += new System.EventHandler(this.button15_Click);

            button19.Location = new System.Drawing.Point(10, btnY + 160); button19.Size = new System.Drawing.Size(btnW, btnH);
            button19.Name = "button19"; button19.Text = "RESUMEN FINANCIERO";
            button19.BackColor = cAzul; button19.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button19.FlatAppearance.BorderSize = 0; button19.ForeColor = System.Drawing.Color.White;
            button19.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            button19.Visible = false;

            // Agregar todos al panel6
            this.panel6.Controls.Add(lblOperaciones);
            this.panel6.Controls.Add(BtnApartados);
            this.panel6.Controls.Add(button2);
            this.panel6.Controls.Add(button1);
            this.panel6.Controls.Add(lblCatalogos);
            this.panel6.Controls.Add(button11);
            this.panel6.Controls.Add(button10);
            this.panel6.Controls.Add(button13);
            this.panel6.Controls.Add(button12);
            this.panel6.Controls.Add(btnAlmacenes);
            this.panel6.Controls.Add(lblReportes);
            this.panel6.Controls.Add(button7);
            this.panel6.Controls.Add(button8);
            this.panel6.Controls.Add(button4);
            this.panel6.Controls.Add(button14);
            this.panel6.Controls.Add(button6);
            this.panel6.Controls.Add(button18);
            this.panel6.Controls.Add(button16);
            this.panel6.Controls.Add(button17);
            this.panel6.Controls.Add(button15);
            this.panel6.Controls.Add(button19);

            // ═══════════════════════════════════════════════════════════════
            // panelPaginador
            // ═══════════════════════════════════════════════════════════════
            this.panelPaginador.Anchor = System.Windows.Forms.AnchorStyles.Bottom
                                          | System.Windows.Forms.AnchorStyles.Left
                                          | System.Windows.Forms.AnchorStyles.Right;
            this.panelPaginador.BackColor = System.Drawing.Color.FromArgb(28, 28, 28);
            this.panelPaginador.Location = new System.Drawing.Point(10, 934);
            this.panelPaginador.Name = "panelPaginador";
            this.panelPaginador.Size = new System.Drawing.Size(1293, 58);
            this.panelPaginador.TabIndex = 60;
            this.panelPaginador.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);

            // lblEstado
            this.lblEstado.AutoSize = true;
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.lblEstado.Location = new System.Drawing.Point(14, 16);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Text = "Paginador";

            System.Windows.Forms.Button PagBtn(System.Windows.Forms.Button b,
                                               string name, string text, int x)
            {
                b.Cursor = System.Windows.Forms.Cursors.Hand;
                b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                b.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(60, 60, 60);
                b.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(41, 128, 185);
                b.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
                b.ForeColor = System.Drawing.Color.White;
                b.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
                b.Location = new System.Drawing.Point(x, 10);
                b.Name = name;
                b.Size = new System.Drawing.Size(110, 36);
                b.Text = text;
                b.UseVisualStyleBackColor = false;
                return b;
            }

            PagBtn(btnPrimero, "btnPrimero", "« Primero", 300);
            btnPrimero.Click += new System.EventHandler(this.btnPrimero_Click);
            PagBtn(btnAnterior, "btnAnterior", "‹ Anterior", 420);
            btnAnterior.Click += new System.EventHandler(this.btnAnterior_Click);
            PagBtn(btnSiguiente, "btnSiguiente", "Siguiente ›", 540);
            btnSiguiente.Click += new System.EventHandler(this.btnSiguiente_Click);
            PagBtn(btnUltimo, "btnUltimo", "Último »", 660);
            btnUltimo.Click += new System.EventHandler(this.btnUltimo_Click);

            this.panelPaginador.Controls.Add(lblEstado);
            this.panelPaginador.Controls.Add(btnPrimero);
            this.panelPaginador.Controls.Add(btnAnterior);
            this.panelPaginador.Controls.Add(btnSiguiente);
            this.panelPaginador.Controls.Add(btnUltimo);

            // ═══════════════════════════════════════════════════════════════
            // FORM
            // ═══════════════════════════════════════════════════════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = false;
            this.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            this.ClientSize = new System.Drawing.Size(1530, 1000);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmInventario";
            this.Text = "Inventario";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmInventario_Load);
            this.Shown += new System.EventHandler(this.frmInventario_Shown);

            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelAnaliticoWrapper);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panelPaginador);

            // ── End init ───────────────────────────────────────────────────
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelFiltros.ResumeLayout(false);
            this.panelFiltros.PerformLayout();
            this.panelKpiContainer.ResumeLayout(false);
            this.panelPaginador.ResumeLayout(false);
            this.panelPaginador.PerformLayout();
            this.panelAnaliticoWrapper.ResumeLayout(false);
            this.panelAnaliticoWrapper.PerformLayout();
            this.panelAnalitico.ResumeLayout(false);
            this.panelAnalitico.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        // ── Declaraciones de campos ────────────────────────────────────────
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Button BtnApartados;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.CheckBox checkBox1;
        public System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.Button btnAlmacenes;
        private System.Windows.Forms.Label lblOperaciones;
        private System.Windows.Forms.Label lblCatalogos;
        private System.Windows.Forms.Label lblReportes;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Button btnPrimero;
        private System.Windows.Forms.Button btnAnterior;
        private System.Windows.Forms.Button btnUltimo;
        private System.Windows.Forms.Button btnSiguiente;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.TableLayoutPanel panelFiltros;
        private System.Windows.Forms.FlowLayoutPanel panelKpiContainer;
        private System.Windows.Forms.Panel panelPaginador;
        private System.Windows.Forms.Panel cardArticulos;
        private System.Windows.Forms.Panel cardStock;
        private System.Windows.Forms.Panel cardCategorias;
        private System.Windows.Forms.Panel cardInversion;
        private System.Windows.Forms.Panel cardVentaPotencial;
        private System.Windows.Forms.Panel cardUtilidad;
        private System.Windows.Forms.Panel cardMargen;
        private System.Windows.Forms.Panel cardStockCritico;
        private System.Windows.Forms.Panel cardCapitalInmovilizado;
        private System.Windows.Forms.Label lblKpiArticulosTitulo;
        private System.Windows.Forms.Label lblKpiArticulos;
        private System.Windows.Forms.Label lblKpiStockTitulo;
        private System.Windows.Forms.Label lblKpiStock;
        private System.Windows.Forms.Label lblKpiCategoriasTitulo;
        private System.Windows.Forms.Label lblKpiCategorias;
        private System.Windows.Forms.Label lblKpiInversionTitulo;
        private System.Windows.Forms.Label lblKpiInversion;
        private System.Windows.Forms.Label lblKpiVentaPotencialTitulo;
        private System.Windows.Forms.Label lblKpiVentaPotencial;
        private System.Windows.Forms.Label lblKpiUtilidadTitulo;
        private System.Windows.Forms.Label lblKpiUtilidad;
        private System.Windows.Forms.Label lblKpiMargenTitulo;
        private System.Windows.Forms.Label lblKpiMargen;
        private System.Windows.Forms.Label lblKpiStockCriticoTitulo;
        private System.Windows.Forms.Label lblKpiStockCritico;
        private System.Windows.Forms.Label lblKpiCapitalInmovilizadoTitulo;
        private System.Windows.Forms.Label lblKpiCapitalInmovilizado;
        private System.Windows.Forms.Panel panelAnalitico;
        private System.Windows.Forms.Panel panelAnaliticoWrapper;
        private System.Windows.Forms.Label lblFinancieroTitulo;
        private System.Windows.Forms.Label lblFinancieroInversion;
        private System.Windows.Forms.Label lblFinancieroVenta;
        private System.Windows.Forms.Label lblFinancieroUtilidad;
        private System.Windows.Forms.Label lblFinancieroMargen;
        private System.Windows.Forms.Label lblFinancieroInmovilizado;
        private System.Windows.Forms.Label lblRiesgoTitulo;
        private System.Windows.Forms.Label lblRiesgoCriticos;
        private System.Windows.Forms.Label lblRiesgoSobrestock;
        private System.Windows.Forms.Label lblRiesgoRotacion;
        private System.Windows.Forms.Label lblRiesgoReposicion;
    }
}