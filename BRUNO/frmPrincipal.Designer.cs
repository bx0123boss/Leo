namespace BRUNO
{
    partial class frmPrincipal
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrincipal));
            this.BtnCobrar = new System.Windows.Forms.Button();
            this.BtnApartados = new System.Windows.Forms.Button();
            this.BtnDevoluciones = new System.Windows.Forms.Button();
            this.BtnTipodecambio = new System.Windows.Forms.Button();
            this.BtnRetiro = new System.Windows.Forms.Button();
            this.BtnDeposito = new System.Windows.Forms.Button();
            this.BtnInventario = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.BtnClientes = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BtnCotizar = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnCobrar
            // 
            this.BtnCobrar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnCobrar.BackgroundImage")));
            this.BtnCobrar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnCobrar.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnCobrar.ForeColor = System.Drawing.Color.White;
            this.BtnCobrar.Location = new System.Drawing.Point(186, 95);
            this.BtnCobrar.Name = "BtnCobrar";
            this.BtnCobrar.Size = new System.Drawing.Size(215, 84);
            this.BtnCobrar.TabIndex = 0;
            this.BtnCobrar.Text = "VENTAS";
            this.BtnCobrar.UseVisualStyleBackColor = true;
            this.BtnCobrar.Click += new System.EventHandler(this.BtnCobrar_Click);
            // 
            // BtnApartados
            // 
            this.BtnApartados.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnApartados.BackgroundImage")));
            this.BtnApartados.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnApartados.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnApartados.ForeColor = System.Drawing.Color.White;
            this.BtnApartados.Location = new System.Drawing.Point(432, 95);
            this.BtnApartados.Name = "BtnApartados";
            this.BtnApartados.Size = new System.Drawing.Size(215, 84);
            this.BtnApartados.TabIndex = 1;
            this.BtnApartados.Text = "APARTADOS";
            this.BtnApartados.UseVisualStyleBackColor = true;
            this.BtnApartados.Click += new System.EventHandler(this.BtnApartados_Click);
            // 
            // BtnDevoluciones
            // 
            this.BtnDevoluciones.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnDevoluciones.BackgroundImage")));
            this.BtnDevoluciones.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnDevoluciones.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnDevoluciones.ForeColor = System.Drawing.Color.White;
            this.BtnDevoluciones.Location = new System.Drawing.Point(186, 225);
            this.BtnDevoluciones.Name = "BtnDevoluciones";
            this.BtnDevoluciones.Size = new System.Drawing.Size(215, 84);
            this.BtnDevoluciones.TabIndex = 2;
            this.BtnDevoluciones.Text = "REPORTE DE VENTAS";
            this.BtnDevoluciones.UseVisualStyleBackColor = true;
            this.BtnDevoluciones.Click += new System.EventHandler(this.BtnDevoluciones_Click);
            // 
            // BtnTipodecambio
            // 
            this.BtnTipodecambio.BackgroundImage = global::BRUNO.Properties.Resources.button;
            this.BtnTipodecambio.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnTipodecambio.Font = new System.Drawing.Font("Modern No. 20", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnTipodecambio.ForeColor = System.Drawing.Color.White;
            this.BtnTipodecambio.Location = new System.Drawing.Point(-73, 567);
            this.BtnTipodecambio.Name = "BtnTipodecambio";
            this.BtnTipodecambio.Size = new System.Drawing.Size(308, 86);
            this.BtnTipodecambio.TabIndex = 3;
            this.BtnTipodecambio.Text = "TIPO DE CAMBIO";
            this.BtnTipodecambio.UseVisualStyleBackColor = true;
            this.BtnTipodecambio.Visible = false;
            this.BtnTipodecambio.Click += new System.EventHandler(this.BtnTipodecambio_Click);
            // 
            // BtnRetiro
            // 
            this.BtnRetiro.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnRetiro.BackgroundImage")));
            this.BtnRetiro.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnRetiro.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnRetiro.ForeColor = System.Drawing.Color.White;
            this.BtnRetiro.Location = new System.Drawing.Point(186, 356);
            this.BtnRetiro.Name = "BtnRetiro";
            this.BtnRetiro.Size = new System.Drawing.Size(215, 84);
            this.BtnRetiro.TabIndex = 4;
            this.BtnRetiro.Text = "RETIRO DE EFECTIVO";
            this.BtnRetiro.UseVisualStyleBackColor = true;
            this.BtnRetiro.Click += new System.EventHandler(this.BtnRetiro_Click);
            // 
            // BtnDeposito
            // 
            this.BtnDeposito.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnDeposito.BackgroundImage")));
            this.BtnDeposito.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnDeposito.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnDeposito.ForeColor = System.Drawing.Color.White;
            this.BtnDeposito.Location = new System.Drawing.Point(432, 356);
            this.BtnDeposito.Name = "BtnDeposito";
            this.BtnDeposito.Size = new System.Drawing.Size(215, 84);
            this.BtnDeposito.TabIndex = 5;
            this.BtnDeposito.Text = "DEPOSITO DE EFECTIVO";
            this.BtnDeposito.UseVisualStyleBackColor = true;
            this.BtnDeposito.Click += new System.EventHandler(this.BtnDeposito_Click);
            // 
            // BtnInventario
            // 
            this.BtnInventario.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnInventario.BackgroundImage")));
            this.BtnInventario.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnInventario.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnInventario.ForeColor = System.Drawing.Color.White;
            this.BtnInventario.Location = new System.Drawing.Point(186, 477);
            this.BtnInventario.Name = "BtnInventario";
            this.BtnInventario.Size = new System.Drawing.Size(215, 84);
            this.BtnInventario.TabIndex = 6;
            this.BtnInventario.Text = "INVENTARIO";
            this.BtnInventario.UseVisualStyleBackColor = true;
            this.BtnInventario.Click += new System.EventHandler(this.BtnInventario_Click);
            // 
            // button1
            // 
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(432, 477);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(215, 84);
            this.button1.TabIndex = 7;
            this.button1.Text = "CORTE";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(-25, 185);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(215, 84);
            this.button2.TabIndex = 8;
            this.button2.Text = "SERVICIOS";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // BtnClientes
            // 
            this.BtnClientes.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnClientes.BackgroundImage")));
            this.BtnClientes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnClientes.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnClientes.ForeColor = System.Drawing.Color.White;
            this.BtnClientes.Location = new System.Drawing.Point(432, 225);
            this.BtnClientes.Name = "BtnClientes";
            this.BtnClientes.Size = new System.Drawing.Size(215, 84);
            this.BtnClientes.TabIndex = 9;
            this.BtnClientes.Text = "CLIENTES";
            this.BtnClientes.UseVisualStyleBackColor = true;
            this.BtnClientes.Click += new System.EventHandler(this.BtnClientes_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Location = new System.Drawing.Point(681, 341);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(399, 220);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // BtnCotizar
            // 
            this.BtnCotizar.BackColor = System.Drawing.Color.Black;
            this.BtnCotizar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnCotizar.BackgroundImage")));
            this.BtnCotizar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnCotizar.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnCotizar.ForeColor = System.Drawing.Color.White;
            this.BtnCotizar.Location = new System.Drawing.Point(760, 122);
            this.BtnCotizar.Name = "BtnCotizar";
            this.BtnCotizar.Size = new System.Drawing.Size(215, 84);
            this.BtnCotizar.TabIndex = 10;
            this.BtnCotizar.Text = "PROVEEDORES";
            this.BtnCotizar.UseVisualStyleBackColor = false;
            this.BtnCotizar.Click += new System.EventHandler(this.BtnCotizar_Click);
            // 
            // button3
            // 
            this.button3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button3.BackgroundImage")));
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button3.Font = new System.Drawing.Font("Modern No. 20", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(288, 587);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(308, 99);
            this.button3.TabIndex = 12;
            this.button3.Text = "PRODUCTO MAS VENDIDO";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button5
            // 
            this.button5.BackgroundImage = global::BRUNO.Properties.Resources.button;
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button5.Font = new System.Drawing.Font("Modern No. 20", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Location = new System.Drawing.Point(-3, -1);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(223, 53);
            this.button5.TabIndex = 14;
            this.button5.Text = "RESPALDO";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button4.BackgroundImage")));
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button4.Font = new System.Drawing.Font("Modern No. 20", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(760, 225);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(215, 84);
            this.button4.TabIndex = 15;
            this.button4.Text = "USUARIOS";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // button7
            // 
            this.button7.BackgroundImage = global::BRUNO.Properties.Resources.compaq;
            this.button7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold);
            this.button7.Location = new System.Drawing.Point(728, 572);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(291, 81);
            this.button7.TabIndex = 19;
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Visible = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.MidnightBlue;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1174, 733);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.BtnCotizar);
            this.Controls.Add(this.BtnClientes);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.BtnInventario);
            this.Controls.Add(this.BtnDeposito);
            this.Controls.Add(this.BtnRetiro);
            this.Controls.Add(this.BtnTipodecambio);
            this.Controls.Add(this.BtnDevoluciones);
            this.Controls.Add(this.BtnApartados);
            this.Controls.Add(this.BtnCobrar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bienvenido";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPrincipal_FormClosing);
            this.Load += new System.EventHandler(this.frmPrincipal_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnCobrar;
        private System.Windows.Forms.Button BtnApartados;
        private System.Windows.Forms.Button BtnDevoluciones;
        private System.Windows.Forms.Button BtnTipodecambio;
        private System.Windows.Forms.Button BtnRetiro;
        private System.Windows.Forms.Button BtnDeposito;
        private System.Windows.Forms.Button BtnInventario;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button BtnClientes;
        private System.Windows.Forms.Button BtnCotizar;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button7;
    }
}

