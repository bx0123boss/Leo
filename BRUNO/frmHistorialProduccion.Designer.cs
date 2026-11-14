namespace JaegerSoft
{
    partial class frmHistorialProduccion
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblEstado = new System.Windows.Forms.Label();
            this.cmbEstado = new System.Windows.Forms.ComboBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.cmbUsuario = new System.Windows.Forms.ComboBox();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.chkFiltrarFecha = new System.Windows.Forms.CheckBox();
            this.pnlResumen = new System.Windows.Forms.Panel();
            this.pnlCardCancelados = new System.Windows.Forms.Panel();
            this.lblLotesCancelados = new System.Windows.Forms.Label();
            this.lblTitCancelados = new System.Windows.Forms.Label();
            this.pnlCardActivos = new System.Windows.Forms.Panel();
            this.lblLotesActivos = new System.Windows.Forms.Label();
            this.lblTitActivos = new System.Windows.Forms.Label();
            this.pnlCardPiezas = new System.Windows.Forms.Panel();
            this.lblTotalPiezas = new System.Windows.Forms.Label();
            this.lblTitPiezas = new System.Windows.Forms.Label();
            this.pnlCardLotes = new System.Windows.Forms.Panel();
            this.lblTotalLotes = new System.Windows.Forms.Label();
            this.lblTitLotes = new System.Windows.Forms.Label();
            this.dgvLotes = new System.Windows.Forms.DataGridView();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblInstruccion = new System.Windows.Forms.Label();
            this.btnVerDetalle = new System.Windows.Forms.Button();
            this.btnCancelarLote = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();

            this.pnlHeader.SuspendLayout();
            this.pnlFiltros.SuspendLayout();
            this.pnlResumen.SuspendLayout();
            this.pnlCardCancelados.SuspendLayout();
            this.pnlCardActivos.SuspendLayout();
            this.pnlCardPiezas.SuspendLayout();
            this.pnlCardLotes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLotes)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();

            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.pnlHeader.Controls.Add(this.lblSubtitulo);
            this.pnlHeader.Controls.Add(this.lblTitulo);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.pnlHeader.Size = new System.Drawing.Size(1180, 58);
            this.pnlHeader.TabIndex = 0;

            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13.5F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(15, 6);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(370, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "HISTORIAL DE PRODUCCIÓN / LOTES";

            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.DarkGray;
            this.lblSubtitulo.Location = new System.Drawing.Point(16, 32);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(560, 15);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Consulta los lotes producidos, filtra por fechas, productos o usuarios, y revierte ajustes con cancelación.";

            // 
            // pnlFiltros
            // 
            this.pnlFiltros.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.pnlFiltros.Controls.Add(this.btnExportar);
            this.pnlFiltros.Controls.Add(this.btnLimpiar);
            this.pnlFiltros.Controls.Add(this.btnBuscar);
            this.pnlFiltros.Controls.Add(this.lblEstado);
            this.pnlFiltros.Controls.Add(this.cmbEstado);
            this.pnlFiltros.Controls.Add(this.lblUsuario);
            this.pnlFiltros.Controls.Add(this.cmbUsuario);
            this.pnlFiltros.Controls.Add(this.lblBuscar);
            this.pnlFiltros.Controls.Add(this.txtBuscar);
            this.pnlFiltros.Controls.Add(this.dtpHasta);
            this.pnlFiltros.Controls.Add(this.lblHasta);
            this.pnlFiltros.Controls.Add(this.dtpDesde);
            this.pnlFiltros.Controls.Add(this.chkFiltrarFecha);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Location = new System.Drawing.Point(0, 58);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(1180, 75);
            this.pnlFiltros.TabIndex = 1;

            // 
            // chkFiltrarFecha
            // 
            this.chkFiltrarFecha.AutoSize = true;
            this.chkFiltrarFecha.Checked = true;
            this.chkFiltrarFecha.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFiltrarFecha.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold);
            this.chkFiltrarFecha.ForeColor = System.Drawing.Color.White;
            this.chkFiltrarFecha.Location = new System.Drawing.Point(15, 12);
            this.chkFiltrarFecha.Name = "chkFiltrarFecha";
            this.chkFiltrarFecha.Size = new System.Drawing.Size(90, 19);
            this.chkFiltrarFecha.TabIndex = 0;
            this.chkFiltrarFecha.Text = "Desde Fecha:";
            this.chkFiltrarFecha.UseVisualStyleBackColor = true;
            this.chkFiltrarFecha.CheckedChanged += new System.EventHandler(this.chkFiltrarFecha_CheckedChanged);

            // 
            // dtpDesde
            // 
            this.dtpDesde.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(15, 36);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(100, 23);
            this.dtpDesde.TabIndex = 1;

            // 
            // lblHasta
            // 
            this.lblHasta.AutoSize = true;
            this.lblHasta.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblHasta.ForeColor = System.Drawing.Color.White;
            this.lblHasta.Location = new System.Drawing.Point(125, 13);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(39, 15);
            this.lblHasta.TabIndex = 2;
            this.lblHasta.Text = "Hasta:";

            // 
            // dtpHasta
            // 
            this.dtpHasta.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(125, 36);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(100, 23);
            this.dtpHasta.TabIndex = 3;

            // 
            // lblBuscar
            // 
            this.lblBuscar.AutoSize = true;
            this.lblBuscar.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblBuscar.ForeColor = System.Drawing.Color.White;
            this.lblBuscar.Location = new System.Drawing.Point(240, 13);
            this.lblBuscar.Name = "lblBuscar";
            this.lblBuscar.Size = new System.Drawing.Size(130, 15);
            this.lblBuscar.TabIndex = 4;
            this.lblBuscar.Text = "Buscar Producto/Folio:";

            // 
            // txtBuscar
            // 
            this.txtBuscar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBuscar.Location = new System.Drawing.Point(240, 36);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(200, 23);
            this.txtBuscar.TabIndex = 5;
            this.txtBuscar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBuscar_KeyDown);

            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblUsuario.ForeColor = System.Drawing.Color.White;
            this.lblUsuario.Location = new System.Drawing.Point(455, 13);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(60, 15);
            this.lblUsuario.TabIndex = 6;
            this.lblUsuario.Text = "Productor:";

            // 
            // cmbUsuario
            // 
            this.cmbUsuario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsuario.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbUsuario.FormattingEnabled = true;
            this.cmbUsuario.Location = new System.Drawing.Point(455, 36);
            this.cmbUsuario.Name = "cmbUsuario";
            this.cmbUsuario.Size = new System.Drawing.Size(140, 23);
            this.cmbUsuario.TabIndex = 7;

            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblEstado.ForeColor = System.Drawing.Color.White;
            this.lblEstado.Location = new System.Drawing.Point(610, 13);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(46, 15);
            this.lblEstado.TabIndex = 8;
            this.lblEstado.Text = "Estado:";

            // 
            // cmbEstado
            // 
            this.cmbEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstado.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbEstado.FormattingEnabled = true;
            this.cmbEstado.Items.AddRange(new object[] {
            "Todos",
            "Solo Activos",
            "Solo Cancelados"});
            this.cmbEstado.Location = new System.Drawing.Point(610, 36);
            this.cmbEstado.Name = "cmbEstado";
            this.cmbEstado.Size = new System.Drawing.Size(125, 23);
            this.cmbEstado.TabIndex = 9;

            // 
            // btnBuscar
            // 
            this.btnBuscar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBuscar.Location = new System.Drawing.Point(750, 32);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(110, 31);
            this.btnBuscar.TabIndex = 10;
            this.btnBuscar.Text = "🔍 FILTRAR";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);

            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.btnLimpiar.FlatAppearance.BorderSize = 0;
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.Location = new System.Drawing.Point(868, 32);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(100, 31);
            this.btnLimpiar.TabIndex = 11;
            this.btnLimpiar.Text = "🔄 LIMPIAR";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);

            // 
            // btnExportar
            // 
            this.btnExportar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnExportar.FlatAppearance.BorderSize = 0;
            this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnExportar.ForeColor = System.Drawing.Color.White;
            this.btnExportar.Location = new System.Drawing.Point(1035, 32);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(130, 31);
            this.btnExportar.TabIndex = 12;
            this.btnExportar.Text = "📊 EXCEL";
            this.btnExportar.UseVisualStyleBackColor = false;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);

            // 
            // pnlResumen
            // 
            this.pnlResumen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlResumen.Controls.Add(this.pnlCardCancelados);
            this.pnlResumen.Controls.Add(this.pnlCardActivos);
            this.pnlResumen.Controls.Add(this.pnlCardPiezas);
            this.pnlResumen.Controls.Add(this.pnlCardLotes);
            this.pnlResumen.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlResumen.Location = new System.Drawing.Point(0, 133);
            this.pnlResumen.Name = "pnlResumen";
            this.pnlResumen.Padding = new System.Windows.Forms.Padding(15, 6, 15, 6);
            this.pnlResumen.Size = new System.Drawing.Size(1180, 60);
            this.pnlResumen.TabIndex = 2;

            // 
            // pnlCardLotes
            // 
            this.pnlCardLotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlCardLotes.Controls.Add(this.lblTotalLotes);
            this.pnlCardLotes.Controls.Add(this.lblTitLotes);
            this.pnlCardLotes.Location = new System.Drawing.Point(15, 8);
            this.pnlCardLotes.Name = "pnlCardLotes";
            this.pnlCardLotes.Size = new System.Drawing.Size(200, 44);
            this.pnlCardLotes.TabIndex = 0;

            // 
            // lblTitLotes
            // 
            this.lblTitLotes.AutoSize = true;
            this.lblTitLotes.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblTitLotes.ForeColor = System.Drawing.Color.LightGray;
            this.lblTitLotes.Location = new System.Drawing.Point(8, 5);
            this.lblTitLotes.Name = "lblTitLotes";
            this.lblTitLotes.Size = new System.Drawing.Size(73, 12);
            this.lblTitLotes.TabIndex = 0;
            this.lblTitLotes.Text = "TOTAL LOTES";

            // 
            // lblTotalLotes
            // 
            this.lblTotalLotes.AutoSize = true;
            this.lblTotalLotes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalLotes.ForeColor = System.Drawing.Color.White;
            this.lblTotalLotes.Location = new System.Drawing.Point(8, 19);
            this.lblTotalLotes.Name = "lblTotalLotes";
            this.lblTotalLotes.Size = new System.Drawing.Size(19, 21);
            this.lblTotalLotes.TabIndex = 1;
            this.lblTotalLotes.Text = "0";

            // 
            // pnlCardPiezas
            // 
            this.pnlCardPiezas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlCardPiezas.Controls.Add(this.lblTotalPiezas);
            this.pnlCardPiezas.Controls.Add(this.lblTitPiezas);
            this.pnlCardPiezas.Location = new System.Drawing.Point(230, 8);
            this.pnlCardPiezas.Name = "pnlCardPiezas";
            this.pnlCardPiezas.Size = new System.Drawing.Size(220, 44);
            this.pnlCardPiezas.TabIndex = 1;

            // 
            // lblTitPiezas
            // 
            this.lblTitPiezas.AutoSize = true;
            this.lblTitPiezas.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblTitPiezas.ForeColor = System.Drawing.Color.LightGray;
            this.lblTitPiezas.Location = new System.Drawing.Point(8, 5);
            this.lblTitPiezas.Name = "lblTitPiezas";
            this.lblTitPiezas.Size = new System.Drawing.Size(117, 12);
            this.lblTitPiezas.TabIndex = 0;
            this.lblTitPiezas.Text = "TOTAL PIEZAS PRODUCIDAS";

            // 
            // lblTotalPiezas
            // 
            this.lblTotalPiezas.AutoSize = true;
            this.lblTotalPiezas.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalPiezas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.lblTotalPiezas.Location = new System.Drawing.Point(8, 19);
            this.lblTotalPiezas.Name = "lblTotalPiezas";
            this.lblTotalPiezas.Size = new System.Drawing.Size(39, 21);
            this.lblTotalPiezas.TabIndex = 1;
            this.lblTotalPiezas.Text = "0 pz";

            // 
            // pnlCardActivos
            // 
            this.pnlCardActivos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlCardActivos.Controls.Add(this.lblLotesActivos);
            this.pnlCardActivos.Controls.Add(this.lblTitActivos);
            this.pnlCardActivos.Location = new System.Drawing.Point(465, 8);
            this.pnlCardActivos.Name = "pnlCardActivos";
            this.pnlCardActivos.Size = new System.Drawing.Size(190, 44);
            this.pnlCardActivos.TabIndex = 2;

            // 
            // lblTitActivos
            // 
            this.lblTitActivos.AutoSize = true;
            this.lblTitActivos.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblTitActivos.ForeColor = System.Drawing.Color.LightGray;
            this.lblTitActivos.Location = new System.Drawing.Point(8, 5);
            this.lblTitActivos.Name = "lblTitActivos";
            this.lblTitActivos.Size = new System.Drawing.Size(84, 12);
            this.lblTitActivos.TabIndex = 0;
            this.lblTitActivos.Text = "LOTES ACTIVOS";

            // 
            // lblLotesActivos
            // 
            this.lblLotesActivos.AutoSize = true;
            this.lblLotesActivos.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblLotesActivos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.lblLotesActivos.Location = new System.Drawing.Point(8, 19);
            this.lblLotesActivos.Name = "lblLotesActivos";
            this.lblLotesActivos.Size = new System.Drawing.Size(19, 21);
            this.lblLotesActivos.TabIndex = 1;
            this.lblLotesActivos.Text = "0";

            // 
            // pnlCardCancelados
            // 
            this.pnlCardCancelados.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlCardCancelados.Controls.Add(this.lblLotesCancelados);
            this.pnlCardCancelados.Controls.Add(this.lblTitCancelados);
            this.pnlCardCancelados.Location = new System.Drawing.Point(670, 8);
            this.pnlCardCancelados.Name = "pnlCardCancelados";
            this.pnlCardCancelados.Size = new System.Drawing.Size(190, 44);
            this.pnlCardCancelados.TabIndex = 3;

            // 
            // lblTitCancelados
            // 
            this.lblTitCancelados.AutoSize = true;
            this.lblTitCancelados.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblTitCancelados.ForeColor = System.Drawing.Color.LightGray;
            this.lblTitCancelados.Location = new System.Drawing.Point(8, 5);
            this.lblTitCancelados.Name = "lblTitCancelados";
            this.lblTitCancelados.Size = new System.Drawing.Size(107, 12);
            this.lblTitCancelados.TabIndex = 0;
            this.lblTitCancelados.Text = "LOTES CANCELADOS";

            // 
            // lblLotesCancelados
            // 
            this.lblLotesCancelados.AutoSize = true;
            this.lblLotesCancelados.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblLotesCancelados.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblLotesCancelados.Location = new System.Drawing.Point(8, 19);
            this.lblLotesCancelados.Name = "lblLotesCancelados";
            this.lblLotesCancelados.Size = new System.Drawing.Size(19, 21);
            this.lblLotesCancelados.TabIndex = 1;
            this.lblLotesCancelados.Text = "0";

            // 
            // dgvLotes
            // 
            this.dgvLotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLotes.Location = new System.Drawing.Point(0, 193);
            this.dgvLotes.Name = "dgvLotes";
            this.dgvLotes.Size = new System.Drawing.Size(1180, 420);
            this.dgvLotes.TabIndex = 3;
            this.dgvLotes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLotes_CellDoubleClick);
            this.dgvLotes.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvLotes_CellFormatting);

            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.pnlBottom.Controls.Add(this.lblInstruccion);
            this.pnlBottom.Controls.Add(this.btnVerDetalle);
            this.pnlBottom.Controls.Add(this.btnCancelarLote);
            this.pnlBottom.Controls.Add(this.btnCerrar);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 613);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1180, 57);
            this.pnlBottom.TabIndex = 4;

            // 
            // lblInstruccion
            // 
            this.lblInstruccion.AutoSize = true;
            this.lblInstruccion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblInstruccion.ForeColor = System.Drawing.Color.LightGray;
            this.lblInstruccion.Location = new System.Drawing.Point(15, 20);
            this.lblInstruccion.Name = "lblInstruccion";
            this.lblInstruccion.Size = new System.Drawing.Size(430, 15);
            this.lblInstruccion.TabIndex = 0;
            this.lblInstruccion.Text = "💡 Haz doble clic en una fila o presiona 'Ver Detalle' para consultar insumos y Kardex.";

            // 
            // btnVerDetalle
            // 
            this.btnVerDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVerDetalle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnVerDetalle.Location = new System.Drawing.Point(630, 10);
            this.btnVerDetalle.Name = "btnVerDetalle";
            this.btnVerDetalle.Size = new System.Drawing.Size(200, 37);
            this.btnVerDetalle.TabIndex = 1;
            this.btnVerDetalle.Text = "🔍 VER DETALLE DEL LOTE";
            this.btnVerDetalle.UseVisualStyleBackColor = true;
            this.btnVerDetalle.Click += new System.EventHandler(this.btnVerDetalle_Click);

            // 
            // btnCancelarLote
            // 
            this.btnCancelarLote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelarLote.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCancelarLote.Location = new System.Drawing.Point(845, 10);
            this.btnCancelarLote.Name = "btnCancelarLote";
            this.btnCancelarLote.Size = new System.Drawing.Size(210, 37);
            this.btnCancelarLote.TabIndex = 2;
            this.btnCancelarLote.Text = "❌ CANCELAR LOTE";
            this.btnCancelarLote.UseVisualStyleBackColor = true;
            this.btnCancelarLote.Click += new System.EventHandler(this.btnCancelarLote_Click);

            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(1070, 10);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(95, 37);
            this.btnCerrar.TabIndex = 3;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);

            // 
            // frmHistorialProduccion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 670);
            this.Controls.Add(this.dgvLotes);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlResumen);
            this.Controls.Add(this.pnlFiltros);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "frmHistorialProduccion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Historial de Producción y Lotes Elaborados";
            this.Load += new System.EventHandler(this.frmHistorialProduccion_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            this.pnlResumen.ResumeLayout(false);
            this.pnlCardCancelados.ResumeLayout(false);
            this.pnlCardCancelados.PerformLayout();
            this.pnlCardActivos.ResumeLayout(false);
            this.pnlCardActivos.PerformLayout();
            this.pnlCardPiezas.ResumeLayout(false);
            this.pnlCardPiezas.PerformLayout();
            this.pnlCardLotes.ResumeLayout(false);
            this.pnlCardLotes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLotes)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.CheckBox chkFiltrarFecha;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.ComboBox cmbUsuario;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cmbEstado;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Panel pnlResumen;
        private System.Windows.Forms.Panel pnlCardLotes;
        private System.Windows.Forms.Label lblTitLotes;
        private System.Windows.Forms.Label lblTotalLotes;
        private System.Windows.Forms.Panel pnlCardPiezas;
        private System.Windows.Forms.Label lblTitPiezas;
        private System.Windows.Forms.Label lblTotalPiezas;
        private System.Windows.Forms.Panel pnlCardActivos;
        private System.Windows.Forms.Label lblTitActivos;
        private System.Windows.Forms.Label lblLotesActivos;
        private System.Windows.Forms.Panel pnlCardCancelados;
        private System.Windows.Forms.Label lblTitCancelados;
        private System.Windows.Forms.Label lblLotesCancelados;
        private System.Windows.Forms.DataGridView dgvLotes;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblInstruccion;
        private System.Windows.Forms.Button btnVerDetalle;
        private System.Windows.Forms.Button btnCancelarLote;
        private System.Windows.Forms.Button btnCerrar;
    }
}
