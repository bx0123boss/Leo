namespace JaegerSoft
{
    partial class frmRecetas
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
            this.lblProducto = new System.Windows.Forms.Label();
            this.pnlAgregar = new System.Windows.Forms.Panel();
            this.lblInsumo = new System.Windows.Forms.Label();
            this.cmbInsumos = new System.Windows.Forms.ComboBox();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.txtCantidad = new System.Windows.Forms.TextBox();
            this.btnAgregarInsumo = new System.Windows.Forms.Button();
            this.btnEliminarInsumo = new System.Windows.Forms.Button();
            this.dgvReceta = new System.Windows.Forms.DataGridView();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblCostoTotal = new System.Windows.Forms.Label();

            this.pnlTop.SuspendLayout();
            this.pnlAgregar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReceta)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlTop.Controls.Add(this.lblProducto);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(784, 50);
            this.pnlTop.TabIndex = 0;
            // 
            // lblProducto
            // 
            this.lblProducto.AutoSize = true;
            this.lblProducto.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblProducto.ForeColor = System.Drawing.Color.White;
            this.lblProducto.Location = new System.Drawing.Point(15, 13);
            this.lblProducto.Name = "lblProducto";
            this.lblProducto.Size = new System.Drawing.Size(200, 21);
            this.lblProducto.Text = "Receta para: [Producto]";
            // 
            // pnlAgregar
            // 
            this.pnlAgregar.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlAgregar.Controls.Add(this.btnEliminarInsumo);
            this.pnlAgregar.Controls.Add(this.btnAgregarInsumo);
            this.pnlAgregar.Controls.Add(this.txtCantidad);
            this.pnlAgregar.Controls.Add(this.lblCantidad);
            this.pnlAgregar.Controls.Add(this.cmbInsumos);
            this.pnlAgregar.Controls.Add(this.lblInsumo);
            this.pnlAgregar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAgregar.Location = new System.Drawing.Point(0, 50);
            this.pnlAgregar.Name = "pnlAgregar";
            this.pnlAgregar.Size = new System.Drawing.Size(784, 70);
            this.pnlAgregar.TabIndex = 1;
            // 
            // lblInsumo
            // 
            this.lblInsumo.AutoSize = true;
            this.lblInsumo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblInsumo.Location = new System.Drawing.Point(16, 12);
            this.lblInsumo.Name = "lblInsumo";
            this.lblInsumo.Size = new System.Drawing.Size(126, 15);
            this.lblInsumo.Text = "Seleccionar Insumo:";
            // 
            // cmbInsumos
            // 
            this.cmbInsumos.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbInsumos.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbInsumos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInsumos.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cmbInsumos.FormattingEnabled = true;
            this.cmbInsumos.Location = new System.Drawing.Point(19, 30);
            this.cmbInsumos.Name = "cmbInsumos";
            this.cmbInsumos.Size = new System.Drawing.Size(280, 25);
            this.cmbInsumos.TabIndex = 0;
            // 
            // lblCantidad
            // 
            this.lblCantidad.AutoSize = true;
            this.lblCantidad.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCantidad.Location = new System.Drawing.Point(315, 12);
            this.lblCantidad.Name = "lblCantidad";
            this.lblCantidad.Size = new System.Drawing.Size(124, 15);
            this.lblCantidad.Text = "Cantidad por Pieza:";
            // 
            // txtCantidad
            // 
            this.txtCantidad.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtCantidad.Location = new System.Drawing.Point(318, 30);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.Size = new System.Drawing.Size(120, 24);
            this.txtCantidad.TabIndex = 1;
            // 
            // btnAgregarInsumo
            // 
            this.btnAgregarInsumo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnAgregarInsumo.FlatAppearance.BorderSize = 0;
            this.btnAgregarInsumo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgregarInsumo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAgregarInsumo.ForeColor = System.Drawing.Color.White;
            this.btnAgregarInsumo.Location = new System.Drawing.Point(460, 28);
            this.btnAgregarInsumo.Name = "btnAgregarInsumo";
            this.btnAgregarInsumo.Size = new System.Drawing.Size(130, 28);
            this.btnAgregarInsumo.TabIndex = 2;
            this.btnAgregarInsumo.Text = "+ Agregar Insumo";
            this.btnAgregarInsumo.UseVisualStyleBackColor = false;
            this.btnAgregarInsumo.Click += new System.EventHandler(this.btnAgregarInsumo_Click);
            // 
            // btnEliminarInsumo
            // 
            this.btnEliminarInsumo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnEliminarInsumo.FlatAppearance.BorderSize = 0;
            this.btnEliminarInsumo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminarInsumo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEliminarInsumo.ForeColor = System.Drawing.Color.White;
            this.btnEliminarInsumo.Location = new System.Drawing.Point(600, 28);
            this.btnEliminarInsumo.Name = "btnEliminarInsumo";
            this.btnEliminarInsumo.Size = new System.Drawing.Size(120, 28);
            this.btnEliminarInsumo.TabIndex = 3;
            this.btnEliminarInsumo.Text = "- Quitar Seleccionado";
            this.btnEliminarInsumo.UseVisualStyleBackColor = false;
            this.btnEliminarInsumo.Click += new System.EventHandler(this.btnEliminarInsumo_Click);
            // 
            // dgvReceta
            // 
            this.dgvReceta.AllowUserToAddRows = false;
            this.dgvReceta.AllowUserToDeleteRows = false;
            this.dgvReceta.BackgroundColor = System.Drawing.Color.White;
            this.dgvReceta.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReceta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReceta.Location = new System.Drawing.Point(0, 120);
            this.dgvReceta.MultiSelect = false;
            this.dgvReceta.Name = "dgvReceta";
            this.dgvReceta.ReadOnly = true;
            this.dgvReceta.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReceta.Size = new System.Drawing.Size(784, 291);
            this.dgvReceta.TabIndex = 2;
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlBottom.Controls.Add(this.lblCostoTotal);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 411);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(784, 50);
            this.pnlBottom.TabIndex = 3;
            // 
            // lblCostoTotal
            // 
            this.lblCostoTotal.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblCostoTotal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblCostoTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblCostoTotal.Location = new System.Drawing.Point(384, 0);
            this.lblCostoTotal.Name = "lblCostoTotal";
            this.lblCostoTotal.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.lblCostoTotal.Size = new System.Drawing.Size(400, 50);
            this.lblCostoTotal.TabIndex = 0;
            this.lblCostoTotal.Text = "Costo de Insumos por Pieza: $0.00";
            this.lblCostoTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmRecetas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.dgvReceta);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlAgregar);
            this.Controls.Add(this.pnlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRecetas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ficha Técnica y Receta de Producto";
            this.Load += new System.EventHandler(this.frmRecetas_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlAgregar.ResumeLayout(false);
            this.pnlAgregar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReceta)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblProducto;
        private System.Windows.Forms.Panel pnlAgregar;
        private System.Windows.Forms.Label lblInsumo;
        private System.Windows.Forms.ComboBox cmbInsumos;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.TextBox txtCantidad;
        private System.Windows.Forms.Button btnAgregarInsumo;
        private System.Windows.Forms.Button btnEliminarInsumo;
        private System.Windows.Forms.DataGridView dgvReceta;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblCostoTotal;
    }
}