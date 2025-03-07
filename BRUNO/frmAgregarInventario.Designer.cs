namespace BRUNO
{
    partial class frmAgregarInventario
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAgregarInventario));
            this.button2 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtLimite = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCantidad = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtVenta = new System.Windows.Forms.TextBox();
            this.txtTotales = new System.Windows.Forms.TextBox();
            this.txtActuales = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lblOrigen = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCompra = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProducto = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label24 = new System.Windows.Forms.Label();
            this.cmbUnidad = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.txtEspecial = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.cmbProveedor = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtMaquila = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtKilataje = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtPeso = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.cmbSub = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cmbCategoria = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblLimite = new System.Windows.Forms.Label();
            this.lblExistencias = new System.Windows.Forms.Label();
            this.lblPrecioVenta = new System.Windows.Forms.Label();
            this.lblPrecioCompra = new System.Windows.Forms.Label();
            this.lblNombre = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.lblOroFino = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(113, 208);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(127, 43);
            this.button2.TabIndex = 18;
            this.button2.Text = "Agregar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(26, 160);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(155, 18);
            this.label6.TabIndex = 17;
            this.label6.Text = "Existencias totales:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(181, 98);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(153, 26);
            this.textBox1.TabIndex = 14;
            this.textBox1.Leave += new System.EventHandler(this.textBox1_Leave);
            // 
            // txtLimite
            // 
            this.txtLimite.Location = new System.Drawing.Point(163, 95);
            this.txtLimite.Name = "txtLimite";
            this.txtLimite.Size = new System.Drawing.Size(379, 26);
            this.txtLimite.TabIndex = 3;
            this.txtLimite.Text = "0";
            this.txtLimite.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLimite_KeyPress);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(98, 99);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 18);
            this.label11.TabIndex = 38;
            this.label11.Text = "Limite:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(16, 107);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(165, 18);
            this.label7.TabIndex = 15;
            this.label7.Text = "Existencias a Añadir:";
            // 
            // txtCantidad
            // 
            this.txtCantidad.Location = new System.Drawing.Point(163, 63);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.Size = new System.Drawing.Size(379, 26);
            this.txtCantidad.TabIndex = 2;
            this.txtCantidad.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCantidad_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(65, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 18);
            this.label5.TabIndex = 31;
            this.label5.Text = "Existencias:";
            // 
            // txtVenta
            // 
            this.txtVenta.Location = new System.Drawing.Point(163, 194);
            this.txtVenta.Name = "txtVenta";
            this.txtVenta.Size = new System.Drawing.Size(379, 26);
            this.txtVenta.TabIndex = 6;
            this.txtVenta.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtVenta_KeyPress);
            // 
            // txtTotales
            // 
            this.txtTotales.Enabled = false;
            this.txtTotales.Location = new System.Drawing.Point(179, 151);
            this.txtTotales.Name = "txtTotales";
            this.txtTotales.Size = new System.Drawing.Size(153, 26);
            this.txtTotales.TabIndex = 16;
            // 
            // txtActuales
            // 
            this.txtActuales.Enabled = false;
            this.txtActuales.Location = new System.Drawing.Point(181, 48);
            this.txtActuales.Name = "txtActuales";
            this.txtActuales.Size = new System.Drawing.Size(153, 26);
            this.txtActuales.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(16, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(168, 18);
            this.label8.TabIndex = 13;
            this.label8.Text = "Existencias Actuales:";
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(245, 321);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 30);
            this.button1.TabIndex = 10;
            this.button1.Text = "Agregar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblOrigen
            // 
            this.lblOrigen.AutoSize = true;
            this.lblOrigen.Location = new System.Drawing.Point(674, 461);
            this.lblOrigen.Name = "lblOrigen";
            this.lblOrigen.Size = new System.Drawing.Size(54, 13);
            this.lblOrigen.TabIndex = 33;
            this.lblOrigen.Text = "Inventario";
            this.lblOrigen.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(2, 198);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(159, 18);
            this.label4.TabIndex = 30;
            this.label4.Text = "Precio de Menudeo:";
            // 
            // txtCompra
            // 
            this.txtCompra.Location = new System.Drawing.Point(162, 227);
            this.txtCompra.Name = "txtCompra";
            this.txtCompra.Size = new System.Drawing.Size(381, 26);
            this.txtCompra.TabIndex = 7;
            this.txtCompra.Text = "0";
            this.txtCompra.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCompra_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(2, 231);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 18);
            this.label3.TabIndex = 29;
            this.label3.Text = "Precio de Mayoreo:";
            // 
            // txtProducto
            // 
            this.txtProducto.Location = new System.Drawing.Point(162, 31);
            this.txtProducto.Name = "txtProducto";
            this.txtProducto.Size = new System.Drawing.Size(380, 26);
            this.txtProducto.TabIndex = 1;
            this.txtProducto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProducto_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 18);
            this.label1.TabIndex = 28;
            this.label1.Text = "Nombre Producto:";
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(47, 12);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(526, 20);
            this.txtID.TabIndex = 0;
            this.txtID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtID_KeyPress);
            this.txtID.Leave += new System.EventHandler(this.txtID_Leave);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(93, 12);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(10, 20);
            this.textBox2.TabIndex = 32;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.txtTotales);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtActuales);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Enabled = false;
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(579, 57);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(353, 272);
            this.groupBox2.TabIndex = 31;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Agregar Existencias";
            this.groupBox2.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.cmbUnidad);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.txtEspecial);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.cmbProveedor);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.txtMaquila);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.txtKilataje);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.txtPeso);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.cmbSub);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.cmbCategoria);
            this.groupBox1.Controls.Add(this.txtLimite);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.txtCantidad);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtVenta);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtCompra);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtProducto);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Enabled = false;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(25, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(548, 362);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Agregar Producto Nuevo";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ForeColor = System.Drawing.Color.White;
            this.label24.Location = new System.Drawing.Point(91, 295);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(65, 18);
            this.label24.TabIndex = 57;
            this.label24.Text = "Unidad:";
            // 
            // cmbUnidad
            // 
            this.cmbUnidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUnidad.FormattingEnabled = true;
            this.cmbUnidad.ItemHeight = 20;
            this.cmbUnidad.Items.AddRange(new object[] {
            "Ferreteria",
            "Materiales"});
            this.cmbUnidad.Location = new System.Drawing.Point(162, 291);
            this.cmbUnidad.Name = "cmbUnidad";
            this.cmbUnidad.Size = new System.Drawing.Size(379, 28);
            this.cmbUnidad.TabIndex = 9;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ForeColor = System.Drawing.Color.White;
            this.label23.Location = new System.Drawing.Point(79, 130);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(82, 18);
            this.label23.TabIndex = 55;
            this.label23.Text = "Impuesto:";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.ItemHeight = 20;
            this.comboBox1.Items.AddRange(new object[] {
            "IVA(16)",
            "NO INCLUYE IVA"});
            this.comboBox1.Location = new System.Drawing.Point(163, 126);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(379, 28);
            this.comboBox1.TabIndex = 4;
            // 
            // txtEspecial
            // 
            this.txtEspecial.Location = new System.Drawing.Point(163, 259);
            this.txtEspecial.Name = "txtEspecial";
            this.txtEspecial.Size = new System.Drawing.Size(380, 26);
            this.txtEspecial.TabIndex = 8;
            this.txtEspecial.Text = "0";
            this.txtEspecial.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEspecial_KeyPress);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.White;
            this.label20.Location = new System.Drawing.Point(6, 262);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(150, 18);
            this.label20.TabIndex = 53;
            this.label20.Text = "Precio de Compra:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.Color.White;
            this.label21.Location = new System.Drawing.Point(47, 361);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(91, 18);
            this.label21.TabIndex = 52;
            this.label21.Text = "Proveedor:";
            this.label21.Visible = false;
            this.label21.Click += new System.EventHandler(this.label21_Click);
            // 
            // cmbProveedor
            // 
            this.cmbProveedor.FormattingEnabled = true;
            this.cmbProveedor.ItemHeight = 20;
            this.cmbProveedor.Location = new System.Drawing.Point(142, 367);
            this.cmbProveedor.Name = "cmbProveedor";
            this.cmbProveedor.Size = new System.Drawing.Size(381, 28);
            this.cmbProveedor.TabIndex = 29;
            this.cmbProveedor.Visible = false;
            this.cmbProveedor.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(402, 394);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 49;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // txtMaquila
            // 
            this.txtMaquila.Location = new System.Drawing.Point(146, 417);
            this.txtMaquila.Name = "txtMaquila";
            this.txtMaquila.Size = new System.Drawing.Size(315, 26);
            this.txtMaquila.TabIndex = 33;
            this.txtMaquila.Visible = false;
            this.txtMaquila.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaquila_KeyPress);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.White;
            this.label19.Location = new System.Drawing.Point(62, 421);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(71, 18);
            this.label19.TabIndex = 48;
            this.label19.Text = "Maquila:";
            this.label19.Visible = false;
            // 
            // txtKilataje
            // 
            this.txtKilataje.Location = new System.Drawing.Point(146, 385);
            this.txtKilataje.Name = "txtKilataje";
            this.txtKilataje.Size = new System.Drawing.Size(379, 26);
            this.txtKilataje.TabIndex = 32;
            this.txtKilataje.Visible = false;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.Color.White;
            this.label18.Location = new System.Drawing.Point(72, 389);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(68, 18);
            this.label18.TabIndex = 46;
            this.label18.Text = "Kilataje:";
            this.label18.Visible = false;
            // 
            // txtPeso
            // 
            this.txtPeso.Location = new System.Drawing.Point(144, 363);
            this.txtPeso.Name = "txtPeso";
            this.txtPeso.Size = new System.Drawing.Size(381, 26);
            this.txtPeso.TabIndex = 31;
            this.txtPeso.Visible = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.White;
            this.label17.Location = new System.Drawing.Point(86, 357);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(52, 18);
            this.label17.TabIndex = 44;
            this.label17.Text = "Peso:";
            this.label17.Visible = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.White;
            this.label16.Location = new System.Drawing.Point(23, 371);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(115, 18);
            this.label16.TabIndex = 42;
            this.label16.Text = "SubCategoria:";
            this.label16.Visible = false;
            // 
            // cmbSub
            // 
            this.cmbSub.FormattingEnabled = true;
            this.cmbSub.ItemHeight = 20;
            this.cmbSub.Location = new System.Drawing.Point(150, 357);
            this.cmbSub.Name = "cmbSub";
            this.cmbSub.Size = new System.Drawing.Size(381, 28);
            this.cmbSub.TabIndex = 28;
            this.cmbSub.Visible = false;
            this.cmbSub.SelectedIndexChanged += new System.EventHandler(this.cmbSub_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.White;
            this.label15.Location = new System.Drawing.Point(71, 164);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(86, 18);
            this.label15.TabIndex = 40;
            this.label15.Text = "Categoria:";
            // 
            // cmbCategoria
            // 
            this.cmbCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoria.FormattingEnabled = true;
            this.cmbCategoria.ItemHeight = 20;
            this.cmbCategoria.Items.AddRange(new object[] {
            "Ferreteria",
            "Materiales"});
            this.cmbCategoria.Location = new System.Drawing.Point(163, 160);
            this.cmbCategoria.Name = "cmbCategoria";
            this.cmbCategoria.Size = new System.Drawing.Size(379, 28);
            this.cmbCategoria.TabIndex = 5;
            this.cmbCategoria.SelectedIndexChanged += new System.EventHandler(this.cmbCategoria_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(11, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 18);
            this.label2.TabIndex = 29;
            this.label2.Text = "ID:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblLimite);
            this.groupBox3.Controls.Add(this.lblExistencias);
            this.groupBox3.Controls.Add(this.lblPrecioVenta);
            this.groupBox3.Controls.Add(this.lblPrecioCompra);
            this.groupBox3.Controls.Add(this.lblNombre);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(25, 51);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(484, 265);
            this.groupBox3.TabIndex = 32;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Datos del producto";
            this.groupBox3.Visible = false;
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // lblLimite
            // 
            this.lblLimite.AutoSize = true;
            this.lblLimite.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLimite.ForeColor = System.Drawing.Color.White;
            this.lblLimite.Location = new System.Drawing.Point(57, 281);
            this.lblLimite.Name = "lblLimite";
            this.lblLimite.Size = new System.Drawing.Size(58, 18);
            this.lblLimite.TabIndex = 44;
            this.lblLimite.Text = "Limite:";
            // 
            // lblExistencias
            // 
            this.lblExistencias.AutoSize = true;
            this.lblExistencias.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExistencias.ForeColor = System.Drawing.Color.White;
            this.lblExistencias.Location = new System.Drawing.Point(49, 226);
            this.lblExistencias.Name = "lblExistencias";
            this.lblExistencias.Size = new System.Drawing.Size(99, 18);
            this.lblExistencias.TabIndex = 43;
            this.lblExistencias.Text = "Existencias:";
            // 
            // lblPrecioVenta
            // 
            this.lblPrecioVenta.AutoSize = true;
            this.lblPrecioVenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrecioVenta.ForeColor = System.Drawing.Color.White;
            this.lblPrecioVenta.Location = new System.Drawing.Point(42, 175);
            this.lblPrecioVenta.Name = "lblPrecioVenta";
            this.lblPrecioVenta.Size = new System.Drawing.Size(99, 18);
            this.lblPrecioVenta.TabIndex = 42;
            this.lblPrecioVenta.Text = "PrecioVenta";
            // 
            // lblPrecioCompra
            // 
            this.lblPrecioCompra.AutoSize = true;
            this.lblPrecioCompra.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrecioCompra.ForeColor = System.Drawing.Color.White;
            this.lblPrecioCompra.Location = new System.Drawing.Point(42, 114);
            this.lblPrecioCompra.Name = "lblPrecioCompra";
            this.lblPrecioCompra.Size = new System.Drawing.Size(150, 18);
            this.lblPrecioCompra.TabIndex = 41;
            this.lblPrecioCompra.Text = "Precio de Compra:";
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombre.ForeColor = System.Drawing.Color.White;
            this.lblNombre.Location = new System.Drawing.Point(42, 63);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(73, 18);
            this.lblNombre.TabIndex = 40;
            this.lblNombre.Text = "Nombre:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(16, 255);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(58, 18);
            this.label14.TabIndex = 39;
            this.label14.Text = "Limite:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.White;
            this.label13.Location = new System.Drawing.Point(16, 200);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(99, 18);
            this.label13.TabIndex = 19;
            this.label13.Text = "Existencias:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(16, 142);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(132, 18);
            this.label9.TabIndex = 17;
            this.label9.Text = "Precio de Venta:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(16, 89);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(150, 18);
            this.label10.TabIndex = 15;
            this.label10.Text = "Precio de Compra:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(16, 35);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 18);
            this.label12.TabIndex = 13;
            this.label12.Text = "Nombre:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.White;
            this.label22.Location = new System.Drawing.Point(541, 401);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(80, 18);
            this.label22.TabIndex = 34;
            this.label22.Text = "Oro Fino:";
            this.label22.Visible = false;
            // 
            // lblOroFino
            // 
            this.lblOroFino.AutoSize = true;
            this.lblOroFino.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOroFino.ForeColor = System.Drawing.Color.White;
            this.lblOroFino.Location = new System.Drawing.Point(616, 401);
            this.lblOroFino.Name = "lblOroFino";
            this.lblOroFino.Size = new System.Drawing.Size(80, 18);
            this.lblOroFino.TabIndex = 35;
            this.lblOroFino.Text = "Oro Fino:";
            this.lblOroFino.Visible = false;
            // 
            // frmAgregarInventario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(590, 424);
            this.Controls.Add(this.lblOroFino);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblOrigen);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAgregarInventario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Agregar Inventario";
            this.Load += new System.EventHandler(this.frmAgregarInventario_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtLimite;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCantidad;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtVenta;
        private System.Windows.Forms.TextBox txtTotales;
        public System.Windows.Forms.TextBox txtActuales;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.Label lblOrigen;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCompra;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtProducto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblLimite;
        private System.Windows.Forms.Label lblExistencias;
        private System.Windows.Forms.Label lblPrecioVenta;
        private System.Windows.Forms.Label lblPrecioCompra;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmbCategoria;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cmbSub;
        private System.Windows.Forms.TextBox txtPeso;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtMaquila;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtKilataje;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ComboBox cmbProveedor;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label lblOroFino;
        private System.Windows.Forms.TextBox txtEspecial;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.ComboBox cmbUnidad;
    }
}