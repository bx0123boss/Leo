namespace BRUNO
{
    partial class frmConsigna
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
            this.lblCliente = new System.Windows.Forms.Label();
            this.dgvBalance = new System.Windows.Forms.DataGridView();
            this.dgvKardex = new System.Windows.Forms.DataGridView();
            this.lblTituloBalance = new System.Windows.Forms.Label();
            this.lblTituloKardex = new System.Windows.Forms.Label();
            this.btnEntregar = new System.Windows.Forms.Button();
            this.btnDevolver = new System.Windows.Forms.Button();
            this.btnCobrar = new System.Windows.Forms.Button();
            this.pnlBotones = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKardex)).BeginInit();
            this.pnlBotones.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCliente
            // 
            this.lblCliente.AutoSize = true;
            this.lblCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCliente.Location = new System.Drawing.Point(14, 24);
            this.lblCliente.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(280, 24);
            this.lblCliente.TabIndex = 0;
            this.lblCliente.Text = "Cliente: [Nombre del Cliente]";
            // 
            // dgvBalance
            // 
            this.dgvBalance.AllowUserToAddRows = false;
            this.dgvBalance.AllowUserToDeleteRows = false;
            this.dgvBalance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBalance.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBalance.BackgroundColor = System.Drawing.Color.White;
            this.dgvBalance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBalance.Location = new System.Drawing.Point(19, 111);
            this.dgvBalance.Margin = new System.Windows.Forms.Padding(4);
            this.dgvBalance.MultiSelect = false;
            this.dgvBalance.Name = "dgvBalance";
            this.dgvBalance.ReadOnly = true;
            this.dgvBalance.RowHeadersVisible = false;
            this.dgvBalance.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBalance.Size = new System.Drawing.Size(883, 262);
            this.dgvBalance.TabIndex = 1;
            // 
            // dgvKardex
            // 
            this.dgvKardex.AllowUserToAddRows = false;
            this.dgvKardex.AllowUserToDeleteRows = false;
            this.dgvKardex.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvKardex.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvKardex.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvKardex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKardex.Location = new System.Drawing.Point(19, 425);
            this.dgvKardex.Margin = new System.Windows.Forms.Padding(4);
            this.dgvKardex.MultiSelect = false;
            this.dgvKardex.Name = "dgvKardex";
            this.dgvKardex.ReadOnly = true;
            this.dgvKardex.RowHeadersVisible = false;
            this.dgvKardex.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvKardex.Size = new System.Drawing.Size(883, 301);
            this.dgvKardex.TabIndex = 2;
            // 
            // lblTituloBalance
            // 
            this.lblTituloBalance.AutoSize = true;
            this.lblTituloBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloBalance.Location = new System.Drawing.Point(15, 84);
            this.lblTituloBalance.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTituloBalance.Name = "lblTituloBalance";
            this.lblTituloBalance.Size = new System.Drawing.Size(260, 18);
            this.lblTituloBalance.TabIndex = 3;
            this.lblTituloBalance.Text = "Mercancía Actual en Consigna (Saldo)";
            // 
            // lblTituloKardex
            // 
            this.lblTituloKardex.AutoSize = true;
            this.lblTituloKardex.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloKardex.Location = new System.Drawing.Point(15, 398);
            this.lblTituloKardex.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTituloKardex.Name = "lblTituloKardex";
            this.lblTituloKardex.Size = new System.Drawing.Size(171, 18);
            this.lblTituloKardex.TabIndex = 4;
            this.lblTituloKardex.Text = "Historial de Movimientos";
            // 
            // btnEntregar
            // 
            this.btnEntregar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEntregar.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnEntregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntregar.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEntregar.ForeColor = System.Drawing.Color.White;
            this.btnEntregar.Location = new System.Drawing.Point(12, 20);
            this.btnEntregar.Margin = new System.Windows.Forms.Padding(4);
            this.btnEntregar.Name = "btnEntregar";
            this.btnEntregar.Size = new System.Drawing.Size(175, 59);
            this.btnEntregar.TabIndex = 5;
            this.btnEntregar.Text = "Entregar Mercancía";
            this.btnEntregar.UseVisualStyleBackColor = false;
            this.btnEntregar.Click += new System.EventHandler(this.btnEntregar_Click_1);
            // 
            // btnDevolver
            // 
            this.btnDevolver.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDevolver.BackColor = System.Drawing.Color.Goldenrod;
            this.btnDevolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDevolver.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDevolver.ForeColor = System.Drawing.Color.White;
            this.btnDevolver.Location = new System.Drawing.Point(12, 98);
            this.btnDevolver.Margin = new System.Windows.Forms.Padding(4);
            this.btnDevolver.Name = "btnDevolver";
            this.btnDevolver.Size = new System.Drawing.Size(175, 59);
            this.btnDevolver.TabIndex = 6;
            this.btnDevolver.Text = "Devolver a Tienda";
            this.btnDevolver.UseVisualStyleBackColor = false;
            // 
            // btnCobrar
            // 
            this.btnCobrar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCobrar.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnCobrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCobrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCobrar.ForeColor = System.Drawing.Color.White;
            this.btnCobrar.Location = new System.Drawing.Point(12, 177);
            this.btnCobrar.Margin = new System.Windows.Forms.Padding(4);
            this.btnCobrar.Name = "btnCobrar";
            this.btnCobrar.Size = new System.Drawing.Size(175, 59);
            this.btnCobrar.TabIndex = 7;
            this.btnCobrar.Text = "Cobrar (Vender)";
            this.btnCobrar.UseVisualStyleBackColor = false;
            // 
            // pnlBotones
            // 
            this.pnlBotones.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBotones.Controls.Add(this.btnCobrar);
            this.pnlBotones.Controls.Add(this.btnEntregar);
            this.pnlBotones.Controls.Add(this.btnDevolver);
            this.pnlBotones.Location = new System.Drawing.Point(912, 92);
            this.pnlBotones.Margin = new System.Windows.Forms.Padding(4);
            this.pnlBotones.Name = "pnlBotones";
            this.pnlBotones.Size = new System.Drawing.Size(198, 262);
            this.pnlBotones.TabIndex = 8;
            // 
            // frmConsigna
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1121, 747);
            this.Controls.Add(this.pnlBotones);
            this.Controls.Add(this.lblTituloKardex);
            this.Controls.Add(this.lblTituloBalance);
            this.Controls.Add(this.dgvKardex);
            this.Controls.Add(this.dgvBalance);
            this.Controls.Add(this.lblCliente);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(931, 642);
            this.Name = "frmConsigna";
            this.Text = "Estado de Cuenta en Consigna";
            this.Load += new System.EventHandler(this.frmConsigna_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKardex)).EndInit();
            this.pnlBotones.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.DataGridView dgvBalance;
        private System.Windows.Forms.DataGridView dgvKardex;
        private System.Windows.Forms.Label lblTituloBalance;
        private System.Windows.Forms.Label lblTituloKardex;
        private System.Windows.Forms.Button btnEntregar;
        private System.Windows.Forms.Button btnDevolver;
        private System.Windows.Forms.Button btnCobrar;
        private System.Windows.Forms.Panel pnlBotones;
    }
}