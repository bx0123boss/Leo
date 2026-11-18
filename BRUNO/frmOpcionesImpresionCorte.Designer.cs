namespace JaegerSoft
{
    partial class frmOpcionesImpresionCorte
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.chkResumen = new System.Windows.Forms.CheckBox();
            this.chkFoliosDetallados = new System.Windows.Forms.CheckBox();
            this.chkCategorias = new System.Windows.Forms.CheckBox();
            this.chkGrupos = new System.Windows.Forms.CheckBox();
            this.chkSalidas = new System.Windows.Forms.CheckBox();
            this.btnImprimir = new System.Windows.Forms.Button();
            this.btnNoImprimir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(22, 18);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(268, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Opciones de Impresión de Corte";
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.Silver;
            this.lblSubtitulo.Location = new System.Drawing.Point(24, 48);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(345, 17);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Cada sección marcada se imprimirá en un ticket separado:";
            // 
            // chkResumen
            // 
            this.chkResumen.AutoSize = true;
            this.chkResumen.Checked = true;
            this.chkResumen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkResumen.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.chkResumen.ForeColor = System.Drawing.Color.White;
            this.chkResumen.Location = new System.Drawing.Point(27, 82);
            this.chkResumen.Name = "chkResumen";
            this.chkResumen.Size = new System.Drawing.Size(261, 24);
            this.chkResumen.TabIndex = 2;
            this.chkResumen.Text = "Ticket 1: Resumen y Arqueo de Caja";
            this.chkResumen.UseVisualStyleBackColor = true;
            // 
            // chkFoliosDetallados
            // 
            this.chkFoliosDetallados.AutoSize = true;
            this.chkFoliosDetallados.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkFoliosDetallados.ForeColor = System.Drawing.Color.LightGray;
            this.chkFoliosDetallados.Location = new System.Drawing.Point(48, 112);
            this.chkFoliosDetallados.Name = "chkFoliosDetallados";
            this.chkFoliosDetallados.Size = new System.Drawing.Size(264, 21);
            this.chkFoliosDetallados.TabIndex = 3;
            this.chkFoliosDetallados.Text = "Incluir listado detallado de folios cobrados";
            this.chkFoliosDetallados.UseVisualStyleBackColor = true;
            // 
            // chkGrupos
            // 
            this.chkGrupos.AutoSize = true;
            this.chkGrupos.Checked = true;
            this.chkGrupos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGrupos.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.chkGrupos.ForeColor = System.Drawing.Color.White;
            this.chkGrupos.Location = new System.Drawing.Point(27, 146);
            this.chkGrupos.Name = "chkGrupos";
            this.chkGrupos.Size = new System.Drawing.Size(277, 24);
            this.chkGrupos.TabIndex = 4;
            this.chkGrupos.Text = "Ticket 2A: Totales por Grupos de Cat.";
            this.chkGrupos.UseVisualStyleBackColor = true;
            // 
            // chkCategorias
            // 
            this.chkCategorias.AutoSize = true;
            this.chkCategorias.Checked = true;
            this.chkCategorias.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCategorias.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.chkCategorias.ForeColor = System.Drawing.Color.White;
            this.chkCategorias.Location = new System.Drawing.Point(27, 178);
            this.chkCategorias.Name = "chkCategorias";
            this.chkCategorias.Size = new System.Drawing.Size(284, 24);
            this.chkCategorias.TabIndex = 5;
            this.chkCategorias.Text = "Ticket 2B: Totales por Categoría (Pzs/$)";
            this.chkCategorias.UseVisualStyleBackColor = true;
            // 
            // chkSalidas
            // 
            this.chkSalidas.AutoSize = true;
            this.chkSalidas.Checked = true;
            this.chkSalidas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSalidas.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.chkSalidas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.chkSalidas.Location = new System.Drawing.Point(27, 210);
            this.chkSalidas.Name = "chkSalidas";
            this.chkSalidas.Size = new System.Drawing.Size(235, 24);
            this.chkSalidas.TabIndex = 6;
            this.chkSalidas.Text = "Ticket 3: Salidas Detalladas";
            this.chkSalidas.UseVisualStyleBackColor = true;
            // 
            // btnImprimir
            // 
            this.btnImprimir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnImprimir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImprimir.FlatAppearance.BorderSize = 0;
            this.btnImprimir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImprimir.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnImprimir.ForeColor = System.Drawing.Color.White;
            this.btnImprimir.Location = new System.Drawing.Point(205, 255);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(175, 42);
            this.btnImprimir.TabIndex = 7;
            this.btnImprimir.Text = "IMPRIMIR SELECCIÓN";
            this.btnImprimir.UseVisualStyleBackColor = false;
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // btnNoImprimir
            // 
            this.btnNoImprimir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(75)))));
            this.btnNoImprimir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNoImprimir.FlatAppearance.BorderSize = 0;
            this.btnNoImprimir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNoImprimir.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNoImprimir.ForeColor = System.Drawing.Color.White;
            this.btnNoImprimir.Location = new System.Drawing.Point(27, 255);
            this.btnNoImprimir.Name = "btnNoImprimir";
            this.btnNoImprimir.Size = new System.Drawing.Size(165, 42);
            this.btnNoImprimir.TabIndex = 8;
            this.btnNoImprimir.Text = "CERRAR SIN IMPRIMIR";
            this.btnNoImprimir.UseVisualStyleBackColor = false;
            this.btnNoImprimir.Click += new System.EventHandler(this.btnNoImprimir_Click);
            // 
            // frmOpcionesImpresionCorte
            // 
            this.AcceptButton = this.btnImprimir;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(32)))));
            this.CancelButton = this.btnNoImprimir;
            this.ClientSize = new System.Drawing.Size(408, 318);
            this.Controls.Add(this.btnNoImprimir);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.chkSalidas);
            this.Controls.Add(this.chkCategorias);
            this.Controls.Add(this.chkGrupos);
            this.Controls.Add(this.chkFoliosDetallados);
            this.Controls.Add(this.chkResumen);
            this.Controls.Add(this.lblSubtitulo);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOpcionesImpresionCorte";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Impresión de Corte de Caja";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.CheckBox chkResumen;
        private System.Windows.Forms.CheckBox chkFoliosDetallados;
        private System.Windows.Forms.CheckBox chkCategorias;
        private System.Windows.Forms.CheckBox chkGrupos;
        private System.Windows.Forms.CheckBox chkSalidas;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.Button btnNoImprimir;
    }
}