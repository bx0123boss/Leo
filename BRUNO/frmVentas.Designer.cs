namespace BRUNO
{
    partial class frmVentas
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVentas));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Cant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Producto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Precio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Monto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Existencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Origin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Impuesto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Compra = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comentario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button3 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFolio = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.txtDescuento = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCliente = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.cmbPago = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblCajero = new System.Windows.Forms.Label();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.GhostWhite;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Aquamarine;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Cant,
            this.Producto,
            this.Precio,
            this.Monto,
            this.Existencia,
            this.ID,
            this.Origin,
            this.Impuesto,
            this.Compra,
            this.Comentario});
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView1.Location = new System.Drawing.Point(6, 68);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.Size = new System.Drawing.Size(1041, 405);
            this.dataGridView1.TabIndex = 9;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            // 
            // Cant
            // 
            this.Cant.HeaderText = "Cant";
            this.Cant.Name = "Cant";
            this.Cant.Width = 80;
            // 
            // Producto
            // 
            this.Producto.FillWeight = 50F;
            this.Producto.HeaderText = "Producto";
            this.Producto.Name = "Producto";
            this.Producto.ReadOnly = true;
            this.Producto.Width = 480;
            // 
            // Precio
            // 
            this.Precio.HeaderText = "Precio";
            this.Precio.Name = "Precio";
            this.Precio.ReadOnly = true;
            this.Precio.Width = 160;
            // 
            // Monto
            // 
            this.Monto.HeaderText = "Monto";
            this.Monto.Name = "Monto";
            this.Monto.ReadOnly = true;
            this.Monto.Width = 160;
            // 
            // Existencia
            // 
            this.Existencia.HeaderText = "Existencia";
            this.Existencia.Name = "Existencia";
            this.Existencia.Visible = false;
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.Visible = false;
            // 
            // Origin
            // 
            this.Origin.HeaderText = "Origin";
            this.Origin.Name = "Origin";
            this.Origin.Visible = false;
            // 
            // Impuesto
            // 
            this.Impuesto.HeaderText = "IVA";
            this.Impuesto.Name = "Impuesto";
            this.Impuesto.Visible = false;
            // 
            // Compra
            // 
            this.Compra.HeaderText = "Compra";
            this.Compra.Name = "Compra";
            this.Compra.Visible = false;
            // 
            // Comentario
            // 
            this.Comentario.HeaderText = "Comentario";
            this.Comentario.Name = "Comentario";
            this.Comentario.Width = 120;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(25, 531);
            this.button3.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(174, 60);
            this.button3.TabIndex = 17;
            this.button3.Text = "BUSCAR PRODUCTO";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(10, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(175, 25);
            this.label2.TabIndex = 15;
            this.label2.Text = " FOLIO VENTA:";
            this.label2.Visible = false;
            // 
            // lblFolio
            // 
            this.lblFolio.AutoSize = true;
            this.lblFolio.BackColor = System.Drawing.Color.Transparent;
            this.lblFolio.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFolio.ForeColor = System.Drawing.Color.White;
            this.lblFolio.Location = new System.Drawing.Point(184, 8);
            this.lblFolio.Name = "lblFolio";
            this.lblFolio.Size = new System.Drawing.Size(25, 25);
            this.lblFolio.TabIndex = 16;
            this.lblFolio.Text = "0";
            this.lblFolio.Visible = false;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.White;
            this.lblTotal.Location = new System.Drawing.Point(572, 555);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(88, 39);
            this.lblTotal.TabIndex = 14;
            this.lblTotal.Text = "0.00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(405, 555);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 39);
            this.label1.TabIndex = 13;
            this.label1.Text = "TOTAL: $";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(751, 560);
            this.button2.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(174, 65);
            this.button2.TabIndex = 12;
            this.button2.Text = "COBRAR";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(211, 531);
            this.button1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(174, 60);
            this.button1.TabIndex = 11;
            this.button1.Text = "ELIMINAR";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(25, 489);
            this.textBox1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(360, 29);
            this.textBox1.TabIndex = 0;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.ForeColor = System.Drawing.Color.White;
            this.radioButton1.Location = new System.Drawing.Point(728, 498);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(128, 28);
            this.radioButton1.TabIndex = 18;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Porcentaje";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton2.ForeColor = System.Drawing.Color.White;
            this.radioButton2.Location = new System.Drawing.Point(862, 498);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(102, 28);
            this.radioButton2.TabIndex = 19;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Efectivo";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // txtDescuento
            // 
            this.txtDescuento.Location = new System.Drawing.Point(728, 532);
            this.txtDescuento.Name = "txtDescuento";
            this.txtDescuento.Size = new System.Drawing.Size(241, 20);
            this.txtDescuento.TabIndex = 20;
            this.txtDescuento.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDescuento_KeyPress);
            this.txtDescuento.Leave += new System.EventHandler(this.txtDescuento_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(746, 478);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(172, 25);
            this.label3.TabIndex = 21;
            this.label3.Text = "Descuento por:";
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(412, 8);
            this.button4.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(167, 24);
            this.button4.TabIndex = 26;
            this.button4.Text = "BUSCAR CLIENTE";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(20, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 25);
            this.label4.TabIndex = 24;
            this.label4.Text = "CLIENTE:";
            // 
            // lblCliente
            // 
            this.lblCliente.AutoSize = true;
            this.lblCliente.BackColor = System.Drawing.Color.Transparent;
            this.lblCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCliente.ForeColor = System.Drawing.Color.White;
            this.lblCliente.Location = new System.Drawing.Point(140, 33);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(234, 24);
            this.lblCliente.TabIndex = 25;
            this.lblCliente.Text = "PUBLICO EN GENERAL";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.ForeColor = System.Drawing.Color.White;
            this.checkBox1.Location = new System.Drawing.Point(427, 603);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(193, 22);
            this.checkBox1.TabIndex = 27;
            this.checkBox1.Text = "PAGO CON TARJETA";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // cmbPago
            // 
            this.cmbPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPago.FormattingEnabled = true;
            this.cmbPago.ItemHeight = 13;
            this.cmbPago.Items.AddRange(new object[] {
            "01=EFECTIVO",
            "02=CHEQUE NOMINATIVO",
            "03=TRANFERENCIA ELECTRONICA DE FONDOS",
            "05=MONEDERO ELECTRONICO",
            "06=DINERO ELECTRONICO",
            "08=VALES DE DESPENSA",
            "12=DACION EN PAGO",
            "13=PAGO POR SUBROGACION",
            "14=PAGO POR CONSIGNACION",
            "15=CONDONACION",
            "17=COMPENSACION",
            "23=NOVACION",
            "24=CONFUSION",
            "25=REMISION DE DEUDA",
            "26=PRESCRIPCION O CADUCIDAD",
            "27=A SATISFACCION DEL ACREEDOR",
            "29=TARJETA DE SERVICIOS",
            "30=APLICACION DE ANTICIPOS",
            "31=INTERMEDIARIO PAGOS",
            "99=POR DEFINIR"});
            this.cmbPago.Location = new System.Drawing.Point(412, 531);
            this.cmbPago.Name = "cmbPago";
            this.cmbPago.Size = new System.Drawing.Size(239, 21);
            this.cmbPago.TabIndex = 28;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(443, 503);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(177, 25);
            this.label5.TabIndex = 29;
            this.label5.Text = "Forma de pago:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.75F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(590, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 24);
            this.label6.TabIndex = 30;
            this.label6.Text = "Vendedor:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.BackColor = System.Drawing.Color.Transparent;
            this.lblUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.75F, System.Drawing.FontStyle.Bold);
            this.lblUsuario.ForeColor = System.Drawing.Color.White;
            this.lblUsuario.Location = new System.Drawing.Point(705, 36);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(79, 24);
            this.lblUsuario.TabIndex = 31;
            this.lblUsuario.Text = "usuario";
            this.lblUsuario.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lblUsuario_MouseDoubleClick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.75F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(622, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 24);
            this.label7.TabIndex = 32;
            this.label7.Text = "Cajero:";
            // 
            // lblCajero
            // 
            this.lblCajero.AutoSize = true;
            this.lblCajero.BackColor = System.Drawing.Color.Transparent;
            this.lblCajero.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.75F, System.Drawing.FontStyle.Bold);
            this.lblCajero.ForeColor = System.Drawing.Color.White;
            this.lblCajero.Location = new System.Drawing.Point(705, 9);
            this.lblCajero.Name = "lblCajero";
            this.lblCajero.Size = new System.Drawing.Size(79, 24);
            this.lblCajero.TabIndex = 33;
            this.lblCajero.Text = "usuario";
            // 
            // frmVentas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1230, 651);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblCajero);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbPago);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblCliente);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDescuento);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblFolio);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmVentas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ventas";
            this.Load += new System.EventHandler(this.frmVentas_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmVentas_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFolio;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.TextBox txtDescuento;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox cmbPago;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label lblCajero;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cant;
        private System.Windows.Forms.DataGridViewTextBoxColumn Producto;
        private System.Windows.Forms.DataGridViewTextBoxColumn Precio;
        private System.Windows.Forms.DataGridViewTextBoxColumn Monto;
        private System.Windows.Forms.DataGridViewTextBoxColumn Existencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Origin;
        private System.Windows.Forms.DataGridViewTextBoxColumn Impuesto;
        private System.Windows.Forms.DataGridViewTextBoxColumn Compra;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comentario;
        private System.Drawing.Printing.PrintDocument printDocument1;
    }
}