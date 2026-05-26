namespace JaegerSoft
{
    partial class frmAlmacenes
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabStock = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupManage = new System.Windows.Forms.GroupBox();
            this.btnEliminarAlmacen = new System.Windows.Forms.Button();
            this.btnEditarAlmacen = new System.Windows.Forms.Button();
            this.btnCrearAlmacen = new System.Windows.Forms.Button();
            this.txtNombreAlmacen = new System.Windows.Forms.TextBox();
            this.lblNuevoAlmacen = new System.Windows.Forms.Label();
            this.dgvAlmacenes = new System.Windows.Forms.DataGridView();
            this.groupStock = new System.Windows.Forms.GroupBox();
            this.txtBuscarProducto = new System.Windows.Forms.TextBox();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.cmbAlmacenStock = new System.Windows.Forms.ComboBox();
            this.lblAlmacenStock = new System.Windows.Forms.Label();
            this.dgvStock = new System.Windows.Forms.DataGridView();
            this.tabTraspasos = new System.Windows.Forms.TabPage();
            this.groupOperacion = new System.Windows.Forms.GroupBox();
            this.btnTraspasar = new System.Windows.Forms.Button();
            this.txtCantidadTraspaso = new System.Windows.Forms.TextBox();
            this.lblCantidadTraspaso = new System.Windows.Forms.Label();
            this.groupInfoProducto = new System.Windows.Forms.GroupBox();
            this.lblStockDestinoVal = new System.Windows.Forms.Label();
            this.lblStockOrigenVal = new System.Windows.Forms.Label();
            this.lblNombreProductoVal = new System.Windows.Forms.Label();
            this.lblStockDestino = new System.Windows.Forms.Label();
            this.lblStockOrigen = new System.Windows.Forms.Label();
            this.lblNombreProducto = new System.Windows.Forms.Label();
            this.txtCodigoProducto = new System.Windows.Forms.TextBox();
            this.lblCodigoProducto = new System.Windows.Forms.Label();
            this.cmbDestino = new System.Windows.Forms.ComboBox();
            this.lblDestino = new System.Windows.Forms.Label();
            this.cmbOrigen = new System.Windows.Forms.ComboBox();
            this.lblOrigen = new System.Windows.Forms.Label();
            this.tabHistorial = new System.Windows.Forms.TabPage();
            this.groupHistorial = new System.Windows.Forms.GroupBox();
            this.btnCancelarMovimiento = new System.Windows.Forms.Button();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.tabControl1.SuspendLayout();
            this.tabStock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupManage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlmacenes)).BeginInit();
            this.groupStock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStock)).BeginInit();
            this.tabTraspasos.SuspendLayout();
            this.groupOperacion.SuspendLayout();
            this.groupInfoProducto.SuspendLayout();
            this.tabHistorial.SuspendLayout();
            this.groupHistorial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabStock);
            this.tabControl1.Controls.Add(this.tabTraspasos);
            this.tabControl1.Controls.Add(this.tabHistorial);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(10, 10);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1164, 641);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabStock
            // 
            this.tabStock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.tabStock.Controls.Add(this.splitContainer1);
            this.tabStock.Location = new System.Drawing.Point(4, 29);
            this.tabStock.Name = "tabStock";
            this.tabStock.Padding = new System.Windows.Forms.Padding(3);
            this.tabStock.Size = new System.Drawing.Size(1156, 608);
            this.tabStock.TabIndex = 0;
            this.tabStock.Text = "Almacenes y Stock";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupManage);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupStock);
            this.splitContainer1.Size = new System.Drawing.Size(1150, 602);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupManage
            // 
            this.groupManage.Controls.Add(this.btnEliminarAlmacen);
            this.groupManage.Controls.Add(this.btnEditarAlmacen);
            this.groupManage.Controls.Add(this.btnCrearAlmacen);
            this.groupManage.Controls.Add(this.txtNombreAlmacen);
            this.groupManage.Controls.Add(this.lblNuevoAlmacen);
            this.groupManage.Controls.Add(this.dgvAlmacenes);
            this.groupManage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupManage.ForeColor = System.Drawing.Color.White;
            this.groupManage.Location = new System.Drawing.Point(0, 0);
            this.groupManage.Name = "groupManage";
            this.groupManage.Size = new System.Drawing.Size(300, 602);
            this.groupManage.TabIndex = 0;
            this.groupManage.TabStop = false;
            this.groupManage.Text = "Administración de Almacenes";
            // 
            // btnEliminarAlmacen
            // 
            this.btnEliminarAlmacen.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnEliminarAlmacen.Location = new System.Drawing.Point(196, 545);
            this.btnEliminarAlmacen.Name = "btnEliminarAlmacen";
            this.btnEliminarAlmacen.Size = new System.Drawing.Size(89, 40);
            this.btnEliminarAlmacen.TabIndex = 5;
            this.btnEliminarAlmacen.Text = "Eliminar";
            this.btnEliminarAlmacen.UseVisualStyleBackColor = true;
            this.btnEliminarAlmacen.Click += new System.EventHandler(this.btnEliminarAlmacen_Click);
            // 
            // btnEditarAlmacen
            // 
            this.btnEditarAlmacen.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnEditarAlmacen.Location = new System.Drawing.Point(102, 545);
            this.btnEditarAlmacen.Name = "btnEditarAlmacen";
            this.btnEditarAlmacen.Size = new System.Drawing.Size(88, 40);
            this.btnEditarAlmacen.TabIndex = 4;
            this.btnEditarAlmacen.Text = "Modificar";
            this.btnEditarAlmacen.UseVisualStyleBackColor = true;
            this.btnEditarAlmacen.Click += new System.EventHandler(this.btnEditarAlmacen_Click);
            // 
            // btnCrearAlmacen
            // 
            this.btnCrearAlmacen.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnCrearAlmacen.Location = new System.Drawing.Point(13, 545);
            this.btnCrearAlmacen.Name = "btnCrearAlmacen";
            this.btnCrearAlmacen.Size = new System.Drawing.Size(83, 40);
            this.btnCrearAlmacen.TabIndex = 3;
            this.btnCrearAlmacen.Text = "Crear";
            this.btnCrearAlmacen.UseVisualStyleBackColor = true;
            this.btnCrearAlmacen.Click += new System.EventHandler(this.btnCrearAlmacen_Click);
            // 
            // txtNombreAlmacen
            // 
            this.txtNombreAlmacen.Location = new System.Drawing.Point(15, 62);
            this.txtNombreAlmacen.Name = "txtNombreAlmacen";
            this.txtNombreAlmacen.Size = new System.Drawing.Size(270, 27);
            this.txtNombreAlmacen.TabIndex = 2;
            // 
            // lblNuevoAlmacen
            // 
            this.lblNuevoAlmacen.AutoSize = true;
            this.lblNuevoAlmacen.Location = new System.Drawing.Point(12, 35);
            this.lblNuevoAlmacen.Name = "lblNuevoAlmacen";
            this.lblNuevoAlmacen.Size = new System.Drawing.Size(154, 20);
            this.lblNuevoAlmacen.TabIndex = 1;
            this.lblNuevoAlmacen.Text = "Nombre del Almacén:";
            // 
            // dgvAlmacenes
            // 
            this.dgvAlmacenes.AllowUserToAddRows = false;
            this.dgvAlmacenes.AllowUserToDeleteRows = false;
            this.dgvAlmacenes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAlmacenes.Location = new System.Drawing.Point(15, 110);
            this.dgvAlmacenes.Name = "dgvAlmacenes";
            this.dgvAlmacenes.ReadOnly = true;
            this.dgvAlmacenes.Size = new System.Drawing.Size(270, 415);
            this.dgvAlmacenes.TabIndex = 0;
            this.dgvAlmacenes.SelectionChanged += new System.EventHandler(this.dgvAlmacenes_SelectionChanged);
            // 
            // groupStock
            // 
            this.groupStock.Controls.Add(this.txtBuscarProducto);
            this.groupStock.Controls.Add(this.lblBuscar);
            this.groupStock.Controls.Add(this.cmbAlmacenStock);
            this.groupStock.Controls.Add(this.lblAlmacenStock);
            this.groupStock.Controls.Add(this.dgvStock);
            this.groupStock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupStock.ForeColor = System.Drawing.Color.White;
            this.groupStock.Location = new System.Drawing.Point(0, 0);
            this.groupStock.Name = "groupStock";
            this.groupStock.Size = new System.Drawing.Size(846, 602);
            this.groupStock.TabIndex = 0;
            this.groupStock.TabStop = false;
            this.groupStock.Text = "Existencias de Productos";
            // 
            // txtBuscarProducto
            // 
            this.txtBuscarProducto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBuscarProducto.Location = new System.Drawing.Point(571, 33);
            this.txtBuscarProducto.Name = "txtBuscarProducto";
            this.txtBuscarProducto.Size = new System.Drawing.Size(255, 27);
            this.txtBuscarProducto.TabIndex = 4;
            this.txtBuscarProducto.TextChanged += new System.EventHandler(this.txtBuscarProducto_TextChanged);
            // 
            // lblBuscar
            // 
            this.lblBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBuscar.AutoSize = true;
            this.lblBuscar.Location = new System.Drawing.Point(509, 36);
            this.lblBuscar.Name = "lblBuscar";
            this.lblBuscar.Size = new System.Drawing.Size(55, 20);
            this.lblBuscar.TabIndex = 3;
            this.lblBuscar.Text = "Buscar:";
            // 
            // cmbAlmacenStock
            // 
            this.cmbAlmacenStock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlmacenStock.FormattingEnabled = true;
            this.cmbAlmacenStock.Location = new System.Drawing.Point(92, 33);
            this.cmbAlmacenStock.Name = "cmbAlmacenStock";
            this.cmbAlmacenStock.Size = new System.Drawing.Size(280, 28);
            this.cmbAlmacenStock.TabIndex = 2;
            this.cmbAlmacenStock.SelectedIndexChanged += new System.EventHandler(this.cmbAlmacenStock_SelectedIndexChanged);
            // 
            // lblAlmacenStock
            // 
            this.lblAlmacenStock.AutoSize = true;
            this.lblAlmacenStock.Location = new System.Drawing.Point(16, 36);
            this.lblAlmacenStock.Name = "lblAlmacenStock";
            this.lblAlmacenStock.Size = new System.Drawing.Size(70, 20);
            this.lblAlmacenStock.TabIndex = 1;
            this.lblAlmacenStock.Text = "Almacén:";
            // 
            // dgvStock
            // 
            this.dgvStock.AllowUserToAddRows = false;
            this.dgvStock.AllowUserToDeleteRows = false;
            this.dgvStock.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStock.Location = new System.Drawing.Point(20, 80);
            this.dgvStock.Name = "dgvStock";
            this.dgvStock.ReadOnly = true;
            this.dgvStock.Size = new System.Drawing.Size(806, 505);
            this.dgvStock.TabIndex = 0;
            // 
            // tabTraspasos
            // 
            this.tabTraspasos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.tabTraspasos.Controls.Add(this.groupOperacion);
            this.tabTraspasos.Location = new System.Drawing.Point(4, 29);
            this.tabTraspasos.Name = "tabTraspasos";
            this.tabTraspasos.Padding = new System.Windows.Forms.Padding(3);
            this.tabTraspasos.Size = new System.Drawing.Size(1156, 608);
            this.tabTraspasos.TabIndex = 1;
            this.tabTraspasos.Text = "Realizar Traspasos";
            // 
            // groupOperacion
            // 
            this.groupOperacion.Controls.Add(this.btnTraspasar);
            this.groupOperacion.Controls.Add(this.txtCantidadTraspaso);
            this.groupOperacion.Controls.Add(this.lblCantidadTraspaso);
            this.groupOperacion.Controls.Add(this.groupInfoProducto);
            this.groupOperacion.Controls.Add(this.txtCodigoProducto);
            this.groupOperacion.Controls.Add(this.lblCodigoProducto);
            this.groupOperacion.Controls.Add(this.cmbDestino);
            this.groupOperacion.Controls.Add(this.lblDestino);
            this.groupOperacion.Controls.Add(this.cmbOrigen);
            this.groupOperacion.Controls.Add(this.lblOrigen);
            this.groupOperacion.ForeColor = System.Drawing.Color.White;
            this.groupOperacion.Location = new System.Drawing.Point(220, 30);
            this.groupOperacion.Name = "groupOperacion";
            this.groupOperacion.Size = new System.Drawing.Size(700, 540);
            this.groupOperacion.TabIndex = 0;
            this.groupOperacion.TabStop = false;
            this.groupOperacion.Text = "Movimiento de Mercancía";
            // 
            // btnTraspasar
            // 
            this.btnTraspasar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTraspasar.Location = new System.Drawing.Point(220, 460);
            this.btnTraspasar.Name = "btnTraspasar";
            this.btnTraspasar.Size = new System.Drawing.Size(260, 50);
            this.btnTraspasar.TabIndex = 9;
            this.btnTraspasar.Text = "Confirmar Traspaso";
            this.btnTraspasar.UseVisualStyleBackColor = true;
            this.btnTraspasar.Click += new System.EventHandler(this.btnTraspasar_Click);
            // 
            // txtCantidadTraspaso
            // 
            this.txtCantidadTraspaso.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCantidadTraspaso.Location = new System.Drawing.Point(320, 396);
            this.txtCantidadTraspaso.Name = "txtCantidadTraspaso";
            this.txtCantidadTraspaso.Size = new System.Drawing.Size(160, 29);
            this.txtCantidadTraspaso.TabIndex = 8;
            // 
            // lblCantidadTraspaso
            // 
            this.lblCantidadTraspaso.AutoSize = true;
            this.lblCantidadTraspaso.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCantidadTraspaso.Location = new System.Drawing.Point(140, 399);
            this.lblCantidadTraspaso.Name = "lblCantidadTraspaso";
            this.lblCantidadTraspaso.Size = new System.Drawing.Size(173, 21);
            this.lblCantidadTraspaso.TabIndex = 7;
            this.lblCantidadTraspaso.Text = "Cantidad a Transferir:";
            // 
            // groupInfoProducto
            // 
            this.groupInfoProducto.Controls.Add(this.lblStockDestinoVal);
            this.groupInfoProducto.Controls.Add(this.lblStockOrigenVal);
            this.groupInfoProducto.Controls.Add(this.lblNombreProductoVal);
            this.groupInfoProducto.Controls.Add(this.lblStockDestino);
            this.groupInfoProducto.Controls.Add(this.lblStockOrigen);
            this.groupInfoProducto.Controls.Add(this.lblNombreProducto);
            this.groupInfoProducto.ForeColor = System.Drawing.Color.LightGray;
            this.groupInfoProducto.Location = new System.Drawing.Point(60, 195);
            this.groupInfoProducto.Name = "groupInfoProducto";
            this.groupInfoProducto.Size = new System.Drawing.Size(580, 175);
            this.groupInfoProducto.TabIndex = 6;
            this.groupInfoProducto.TabStop = false;
            this.groupInfoProducto.Text = "Información del Producto";
            // 
            // lblStockDestinoVal
            // 
            this.lblStockDestinoVal.AutoSize = true;
            this.lblStockDestinoVal.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStockDestinoVal.ForeColor = System.Drawing.Color.White;
            this.lblStockDestinoVal.Location = new System.Drawing.Point(260, 130);
            this.lblStockDestinoVal.Name = "lblStockDestinoVal";
            this.lblStockDestinoVal.Size = new System.Drawing.Size(18, 20);
            this.lblStockDestinoVal.TabIndex = 5;
            this.lblStockDestinoVal.Text = "0";
            // 
            // lblStockOrigenVal
            // 
            this.lblStockOrigenVal.AutoSize = true;
            this.lblStockOrigenVal.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStockOrigenVal.ForeColor = System.Drawing.Color.White;
            this.lblStockOrigenVal.Location = new System.Drawing.Point(260, 85);
            this.lblStockOrigenVal.Name = "lblStockOrigenVal";
            this.lblStockOrigenVal.Size = new System.Drawing.Size(18, 20);
            this.lblStockOrigenVal.TabIndex = 4;
            this.lblStockOrigenVal.Text = "0";
            // 
            // lblNombreProductoVal
            // 
            this.lblNombreProductoVal.AutoSize = true;
            this.lblNombreProductoVal.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreProductoVal.ForeColor = System.Drawing.Color.White;
            this.lblNombreProductoVal.Location = new System.Drawing.Point(260, 40);
            this.lblNombreProductoVal.Name = "lblNombreProductoVal";
            this.lblNombreProductoVal.Size = new System.Drawing.Size(171, 20);
            this.lblNombreProductoVal.TabIndex = 3;
            this.lblNombreProductoVal.Text = "Seleccione un producto";
            // 
            // lblStockDestino
            // 
            this.lblStockDestino.AutoSize = true;
            this.lblStockDestino.Location = new System.Drawing.Point(30, 130);
            this.lblStockDestino.Name = "lblStockDestino";
            this.lblStockDestino.Size = new System.Drawing.Size(214, 20);
            this.lblStockDestino.TabIndex = 2;
            this.lblStockDestino.Text = "Existencia en Almacén Destino:";
            // 
            // lblStockOrigen
            // 
            this.lblStockOrigen.AutoSize = true;
            this.lblStockOrigen.Location = new System.Drawing.Point(30, 85);
            this.lblStockOrigen.Name = "lblStockOrigen";
            this.lblStockOrigen.Size = new System.Drawing.Size(208, 20);
            this.lblStockOrigen.TabIndex = 1;
            this.lblStockOrigen.Text = "Existencia en Almacén Origen:";
            // 
            // lblNombreProducto
            // 
            this.lblNombreProducto.AutoSize = true;
            this.lblNombreProducto.Location = new System.Drawing.Point(30, 40);
            this.lblNombreProducto.Name = "lblNombreProducto";
            this.lblNombreProducto.Size = new System.Drawing.Size(156, 20);
            this.lblNombreProducto.TabIndex = 0;
            this.lblNombreProducto.Text = "Nombre del Producto:";
            // 
            // txtCodigoProducto
            // 
            this.txtCodigoProducto.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigoProducto.Location = new System.Drawing.Point(240, 142);
            this.txtCodigoProducto.Name = "txtCodigoProducto";
            this.txtCodigoProducto.Size = new System.Drawing.Size(400, 27);
            this.txtCodigoProducto.TabIndex = 5;
            this.txtCodigoProducto.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCodigoProducto_KeyDown);
            // 
            // lblCodigoProducto
            // 
            this.lblCodigoProducto.AutoSize = true;
            this.lblCodigoProducto.Location = new System.Drawing.Point(56, 145);
            this.lblCodigoProducto.Name = "lblCodigoProducto";
            this.lblCodigoProducto.Size = new System.Drawing.Size(171, 20);
            this.lblCodigoProducto.TabIndex = 4;
            this.lblCodigoProducto.Text = "Código/ID del Producto:";
            // 
            // cmbDestino
            // 
            this.cmbDestino.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDestino.FormattingEnabled = true;
            this.cmbDestino.Location = new System.Drawing.Point(240, 90);
            this.cmbDestino.Name = "cmbDestino";
            this.cmbDestino.Size = new System.Drawing.Size(400, 28);
            this.cmbDestino.TabIndex = 3;
            this.cmbDestino.SelectedIndexChanged += new System.EventHandler(this.cmbDestino_SelectedIndexChanged);
            // 
            // lblDestino
            // 
            this.lblDestino.AutoSize = true;
            this.lblDestino.Location = new System.Drawing.Point(56, 93);
            this.lblDestino.Name = "lblDestino";
            this.lblDestino.Size = new System.Drawing.Size(125, 20);
            this.lblDestino.TabIndex = 2;
            this.lblDestino.Text = "Almacén Destino:";
            // 
            // cmbOrigen
            // 
            this.cmbOrigen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOrigen.FormattingEnabled = true;
            this.cmbOrigen.Location = new System.Drawing.Point(240, 40);
            this.cmbOrigen.Name = "cmbOrigen";
            this.cmbOrigen.Size = new System.Drawing.Size(400, 28);
            this.cmbOrigen.TabIndex = 1;
            this.cmbOrigen.SelectedIndexChanged += new System.EventHandler(this.cmbOrigen_SelectedIndexChanged);
            // 
            // lblOrigen
            // 
            this.lblOrigen.AutoSize = true;
            this.lblOrigen.Location = new System.Drawing.Point(56, 43);
            this.lblOrigen.Name = "lblOrigen";
            this.lblOrigen.Size = new System.Drawing.Size(119, 20);
            this.lblOrigen.TabIndex = 0;
            this.lblOrigen.Text = "Almacén Origen:";
            // 
            // tabHistorial
            // 
            this.tabHistorial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.tabHistorial.Controls.Add(this.groupHistorial);
            this.tabHistorial.Location = new System.Drawing.Point(4, 29);
            this.tabHistorial.Name = "tabHistorial";
            this.tabHistorial.Padding = new System.Windows.Forms.Padding(3);
            this.tabHistorial.Size = new System.Drawing.Size(1156, 608);
            this.tabHistorial.TabIndex = 2;
            this.tabHistorial.Text = "Historial de Movimientos";
            // 
            // groupHistorial
            // 
            this.groupHistorial.Controls.Add(this.btnCancelarMovimiento);
            this.groupHistorial.Controls.Add(this.dgvHistorial);
            this.groupHistorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupHistorial.ForeColor = System.Drawing.Color.White;
            this.groupHistorial.Location = new System.Drawing.Point(3, 3);
            this.groupHistorial.Name = "groupHistorial";
            this.groupHistorial.Size = new System.Drawing.Size(1150, 602);
            this.groupHistorial.TabIndex = 0;
            this.groupHistorial.TabStop = false;
            this.groupHistorial.Text = "Registro Histórico de Traspasos";
            // 
            // btnCancelarMovimiento
            // 
            this.btnCancelarMovimiento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelarMovimiento.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnCancelarMovimiento.Location = new System.Drawing.Point(15, 535);
            this.btnCancelarMovimiento.Name = "btnCancelarMovimiento";
            this.btnCancelarMovimiento.Size = new System.Drawing.Size(280, 45);
            this.btnCancelarMovimiento.TabIndex = 1;
            this.btnCancelarMovimiento.Text = "Cancelar Movimiento Seleccionado";
            this.btnCancelarMovimiento.UseVisualStyleBackColor = true;
            this.btnCancelarMovimiento.Click += new System.EventHandler(this.btnCancelarMovimiento_Click);
            // 
            // dgvHistorial
            // 
            this.dgvHistorial.AllowUserToAddRows = false;
            this.dgvHistorial.AllowUserToDeleteRows = false;
            this.dgvHistorial.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvHistorial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorial.Location = new System.Drawing.Point(15, 30);
            this.dgvHistorial.Name = "dgvHistorial";
            this.dgvHistorial.ReadOnly = true;
            this.dgvHistorial.Size = new System.Drawing.Size(1120, 490);
            this.dgvHistorial.TabIndex = 0;
            // 
            // frmAlmacenes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.tabControl1);
            this.Name = "frmAlmacenes";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Manejo de Almacenes";
            this.Load += new System.EventHandler(this.frmAlmacenes_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabStock.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupManage.ResumeLayout(false);
            this.groupManage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlmacenes)).EndInit();
            this.groupStock.ResumeLayout(false);
            this.groupStock.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStock)).EndInit();
            this.tabTraspasos.ResumeLayout(false);
            this.groupOperacion.ResumeLayout(false);
            this.groupOperacion.PerformLayout();
            this.groupInfoProducto.ResumeLayout(false);
            this.groupInfoProducto.PerformLayout();
            this.tabHistorial.ResumeLayout(false);
            this.groupHistorial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabStock;
        private System.Windows.Forms.TabPage tabTraspasos;
        private System.Windows.Forms.TabPage tabHistorial;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupManage;
        private System.Windows.Forms.DataGridView dgvAlmacenes;
        private System.Windows.Forms.GroupBox groupStock;
        private System.Windows.Forms.DataGridView dgvStock;
        private System.Windows.Forms.Button btnCrearAlmacen;
        private System.Windows.Forms.TextBox txtNombreAlmacen;
        private System.Windows.Forms.Label lblNuevoAlmacen;
        private System.Windows.Forms.Button btnEliminarAlmacen;
        private System.Windows.Forms.Button btnEditarAlmacen;
        private System.Windows.Forms.ComboBox cmbAlmacenStock;
        private System.Windows.Forms.Label lblAlmacenStock;
        private System.Windows.Forms.TextBox txtBuscarProducto;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.GroupBox groupOperacion;
        private System.Windows.Forms.ComboBox cmbOrigen;
        private System.Windows.Forms.Label lblOrigen;
        private System.Windows.Forms.ComboBox cmbDestino;
        private System.Windows.Forms.Label lblDestino;
        private System.Windows.Forms.TextBox txtCodigoProducto;
        private System.Windows.Forms.Label lblCodigoProducto;
        private System.Windows.Forms.GroupBox groupInfoProducto;
        private System.Windows.Forms.Label lblStockDestinoVal;
        private System.Windows.Forms.Label lblStockOrigenVal;
        private System.Windows.Forms.Label lblNombreProductoVal;
        private System.Windows.Forms.Label lblStockDestino;
        private System.Windows.Forms.Label lblStockOrigen;
        private System.Windows.Forms.Label lblNombreProducto;
        private System.Windows.Forms.TextBox txtCantidadTraspaso;
        private System.Windows.Forms.Label lblCantidadTraspaso;
        private System.Windows.Forms.Button btnTraspasar;
        private System.Windows.Forms.GroupBox groupHistorial;
        private System.Windows.Forms.DataGridView dgvHistorial;
        private System.Windows.Forms.Button btnCancelarMovimiento;
    }
}
