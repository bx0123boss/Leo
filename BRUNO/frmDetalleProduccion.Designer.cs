namespace JaegerSoft
{
    partial class frmDetalleProduccion
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblBadgeEstado = new System.Windows.Forms.Label();
            this.lblFolio = new System.Windows.Forms.Label();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.lblCancelacionInfo = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.lblProducto = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabInsumos = new System.Windows.Forms.TabPage();
            this.dgvInsumos = new System.Windows.Forms.DataGridView();
            this.tabKardex = new System.Windows.Forms.TabPage();
            this.dgvKardex = new System.Windows.Forms.DataGridView();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblCostoTotal = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();

            this.pnlTop.SuspendLayout();
            this.pnlInfo.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabInsumos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInsumos)).BeginInit();
            this.tabKardex.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKardex)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();

            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.pnlTop.Controls.Add(this.lblBadgeEstado);
            this.pnlTop.Controls.Add(this.lblFolio);
            this.pnlTop.Controls.Add(this.lblTitulo);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(920, 52);
            this.pnlTop.TabIndex = 0;

            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(14, 13);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(252, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "DETALLE DE PRODUCCIÓN";

            // 
            // lblFolio
            // 
            this.lblFolio.AutoSize = true;
            this.lblFolio.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblFolio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblFolio.Location = new System.Drawing.Point(275, 16);
            this.lblFolio.Name = "lblFolio";
            this.lblFolio.Size = new System.Drawing.Size(155, 20);
            this.lblFolio.TabIndex = 1;
            this.lblFolio.Text = "Folio: LOT-00000000";

            // 
            // lblBadgeEstado
            // 
            this.lblBadgeEstado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBadgeEstado.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.lblBadgeEstado.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblBadgeEstado.ForeColor = System.Drawing.Color.White;
            this.lblBadgeEstado.Location = new System.Drawing.Point(760, 11);
            this.lblBadgeEstado.Name = "lblBadgeEstado";
            this.lblBadgeEstado.Size = new System.Drawing.Size(145, 30);
            this.lblBadgeEstado.TabIndex = 2;
            this.lblBadgeEstado.Text = "ACTIVO";
            this.lblBadgeEstado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // pnlInfo
            // 
            this.pnlInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.pnlInfo.Controls.Add(this.lblCancelacionInfo);
            this.pnlInfo.Controls.Add(this.lblUsuario);
            this.pnlInfo.Controls.Add(this.lblFecha);
            this.pnlInfo.Controls.Add(this.lblCantidad);
            this.pnlInfo.Controls.Add(this.lblProducto);
            this.pnlInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlInfo.Location = new System.Drawing.Point(0, 52);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.pnlInfo.Size = new System.Drawing.Size(920, 95);
            this.pnlInfo.TabIndex = 1;

            // 
            // lblProducto
            // 
            this.lblProducto.AutoSize = true;
            this.lblProducto.Font = new System.Drawing.Font("Segoe UI", 11.5F, System.Drawing.FontStyle.Bold);
            this.lblProducto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.lblProducto.Location = new System.Drawing.Point(15, 10);
            this.lblProducto.Name = "lblProducto";
            this.lblProducto.Size = new System.Drawing.Size(200, 21);
            this.lblProducto.TabIndex = 0;
            this.lblProducto.Text = "Producto: [ID] Nombre";

            // 
            // lblCantidad
            // 
            this.lblCantidad.AutoSize = true;
            this.lblCantidad.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblCantidad.ForeColor = System.Drawing.Color.White;
            this.lblCantidad.Location = new System.Drawing.Point(15, 38);
            this.lblCantidad.Name = "lblCantidad";
            this.lblCantidad.Size = new System.Drawing.Size(185, 19);
            this.lblCantidad.TabIndex = 1;
            this.lblCantidad.Text = "Cantidad Producida: 0 pz";

            // 
            // lblFecha
            // 
            this.lblFecha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFecha.AutoSize = true;
            this.lblFecha.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblFecha.ForeColor = System.Drawing.Color.LightGray;
            this.lblFecha.Location = new System.Drawing.Point(600, 10);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(130, 19);
            this.lblFecha.TabIndex = 2;
            this.lblFecha.Text = "Fecha: 00/00/0000";

            // 
            // lblUsuario
            // 
            this.lblUsuario.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblUsuario.ForeColor = System.Drawing.Color.LightGray;
            this.lblUsuario.Location = new System.Drawing.Point(600, 38);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(170, 19);
            this.lblUsuario.TabIndex = 3;
            this.lblUsuario.Text = "Productor / Usuario: ---";

            // 
            // lblCancelacionInfo
            // 
            this.lblCancelacionInfo.AutoSize = true;
            this.lblCancelacionInfo.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblCancelacionInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblCancelacionInfo.Location = new System.Drawing.Point(15, 66);
            this.lblCancelacionInfo.Name = "lblCancelacionInfo";
            this.lblCancelacionInfo.Size = new System.Drawing.Size(260, 17);
            this.lblCancelacionInfo.TabIndex = 4;
            this.lblCancelacionInfo.Text = "Cancelado el: --/--/---- por: Usuario";
            this.lblCancelacionInfo.Visible = false;

            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabInsumos);
            this.tabControl.Controls.Add(this.tabKardex);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.tabControl.Location = new System.Drawing.Point(0, 147);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(920, 375);
            this.tabControl.TabIndex = 2;

            // 
            // tabInsumos
            // 
            this.tabInsumos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tabInsumos.Controls.Add(this.dgvInsumos);
            this.tabInsumos.Location = new System.Drawing.Point(4, 26);
            this.tabInsumos.Name = "tabInsumos";
            this.tabInsumos.Padding = new System.Windows.Forms.Padding(10);
            this.tabInsumos.Size = new System.Drawing.Size(912, 345);
            this.tabInsumos.TabIndex = 0;
            this.tabInsumos.Text = "  Insumos y Receta Consumida  ";

            // 
            // dgvInsumos
            // 
            this.dgvInsumos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInsumos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInsumos.Location = new System.Drawing.Point(10, 10);
            this.dgvInsumos.Name = "dgvInsumos";
            this.dgvInsumos.Size = new System.Drawing.Size(892, 325);
            this.dgvInsumos.TabIndex = 0;

            // 
            // tabKardex
            // 
            this.tabKardex.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tabKardex.Controls.Add(this.dgvKardex);
            this.tabKardex.Location = new System.Drawing.Point(4, 26);
            this.tabKardex.Name = "tabKardex";
            this.tabKardex.Padding = new System.Windows.Forms.Padding(10);
            this.tabKardex.Size = new System.Drawing.Size(912, 345);
            this.tabKardex.TabIndex = 1;
            this.tabKardex.Text = "  Movimientos en Kardex  ";

            // 
            // dgvKardex
            // 
            this.dgvKardex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKardex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvKardex.Location = new System.Drawing.Point(10, 10);
            this.dgvKardex.Name = "dgvKardex";
            this.dgvKardex.Size = new System.Drawing.Size(892, 325);
            this.dgvKardex.TabIndex = 0;

            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.pnlBottom.Controls.Add(this.lblCostoTotal);
            this.pnlBottom.Controls.Add(this.btnCancelar);
            this.pnlBottom.Controls.Add(this.btnCerrar);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 522);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(920, 58);
            this.pnlBottom.TabIndex = 3;

            // 
            // lblCostoTotal
            // 
            this.lblCostoTotal.AutoSize = true;
            this.lblCostoTotal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblCostoTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.lblCostoTotal.Location = new System.Drawing.Point(15, 18);
            this.lblCostoTotal.Name = "lblCostoTotal";
            this.lblCostoTotal.Size = new System.Drawing.Size(260, 20);
            this.lblCostoTotal.TabIndex = 0;
            this.lblCostoTotal.Text = "Costo Total Estimado Insumos: $0.00";

            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.Location = new System.Drawing.Point(460, 9);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(320, 40);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "❌ CANCELAR PRODUCCIÓN (REVERTIR)";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(795, 9);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(110, 40);
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);

            // 
            // frmDetalleProduccion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 580);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlInfo);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.MinimumSize = new System.Drawing.Size(850, 550);
            this.Name = "frmDetalleProduccion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Detalle de Producción / Lote";
            this.Load += new System.EventHandler(this.frmDetalleProduccion_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabInsumos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInsumos)).EndInit();
            this.tabKardex.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvKardex)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblFolio;
        private System.Windows.Forms.Label lblBadgeEstado;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.Label lblProducto;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Label lblCancelacionInfo;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabInsumos;
        private System.Windows.Forms.DataGridView dgvInsumos;
        private System.Windows.Forms.TabPage tabKardex;
        private System.Windows.Forms.DataGridView dgvKardex;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblCostoTotal;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnCerrar;
    }
}
