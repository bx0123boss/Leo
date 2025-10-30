namespace BRUNO
{
    partial class frmConfiguracionTicket
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox grpEncabezado;
        private System.Windows.Forms.GroupBox grpPie;
        private System.Windows.Forms.TextBox txtEncabezado;
        private System.Windows.Forms.TextBox txtPie;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.TextBox txtLogoPath;
        private System.Windows.Forms.Button btnBuscarLogo;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.PictureBox picLogo;

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
            this.grpEncabezado = new System.Windows.Forms.GroupBox();
            this.txtEncabezado = new System.Windows.Forms.TextBox();
            this.grpPie = new System.Windows.Forms.GroupBox();
            this.txtPie = new System.Windows.Forms.TextBox();
            this.lblLogo = new System.Windows.Forms.Label();
            this.txtLogoPath = new System.Windows.Forms.TextBox();
            this.btnBuscarLogo = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.picLogo = new System.Windows.Forms.PictureBox();

            this.grpEncabezado.SuspendLayout();
            this.grpPie.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();

            // 
            // grpEncabezado
            // 
            this.grpEncabezado.Controls.Add(this.txtEncabezado);
            this.grpEncabezado.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpEncabezado.ForeColor = System.Drawing.Color.White;
            this.grpEncabezado.Location = new System.Drawing.Point(20, 20);
            this.grpEncabezado.Name = "grpEncabezado";
            this.grpEncabezado.Size = new System.Drawing.Size(370, 170);
            this.grpEncabezado.TabIndex = 0;
            this.grpEncabezado.TabStop = false;
            this.grpEncabezado.Text = "Encabezado del Ticket";

            // 
            // txtEncabezado
            // 
            this.txtEncabezado.BackColor = System.Drawing.Color.Black;
            this.txtEncabezado.ForeColor = System.Drawing.Color.Lime;
            this.txtEncabezado.Multiline = true;
            this.txtEncabezado.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEncabezado.Location = new System.Drawing.Point(10, 25);
            this.txtEncabezado.Name = "txtEncabezado";
            this.txtEncabezado.Size = new System.Drawing.Size(350, 130);
            this.txtEncabezado.TabIndex = 0;

            // 
            // grpPie
            // 
            this.grpPie.Controls.Add(this.txtPie);
            this.grpPie.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpPie.ForeColor = System.Drawing.Color.White;
            this.grpPie.Location = new System.Drawing.Point(20, 200);
            this.grpPie.Name = "grpPie";
            this.grpPie.Size = new System.Drawing.Size(370, 130);
            this.grpPie.TabIndex = 1;
            this.grpPie.TabStop = false;
            this.grpPie.Text = "Pie del Ticket";

            // 
            // txtPie
            // 
            this.txtPie.BackColor = System.Drawing.Color.Black;
            this.txtPie.ForeColor = System.Drawing.Color.Lime;
            this.txtPie.Multiline = true;
            this.txtPie.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPie.Location = new System.Drawing.Point(10, 25);
            this.txtPie.Name = "txtPie";
            this.txtPie.Size = new System.Drawing.Size(350, 90);
            this.txtPie.TabIndex = 0;

            // 
            // lblLogo
            // 
            this.lblLogo.AutoSize = true;
            this.lblLogo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblLogo.ForeColor = System.Drawing.Color.White;
            this.lblLogo.Location = new System.Drawing.Point(420, 25);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(46, 19);
            this.lblLogo.TabIndex = 2;
            this.lblLogo.Text = "Logo:";

            // 
            // txtLogoPath
            // 
            this.txtLogoPath.BackColor = System.Drawing.Color.Black;
            this.txtLogoPath.ForeColor = System.Drawing.Color.Lime;
            this.txtLogoPath.Location = new System.Drawing.Point(470, 25);
            this.txtLogoPath.Name = "txtLogoPath";
            this.txtLogoPath.Size = new System.Drawing.Size(250, 23);
            this.txtLogoPath.TabIndex = 3;

            // 
            // btnBuscarLogo
            // 
            this.btnBuscarLogo.BackColor = System.Drawing.Color.DimGray;
            this.btnBuscarLogo.ForeColor = System.Drawing.Color.White;
            this.btnBuscarLogo.Location = new System.Drawing.Point(730, 25);
            this.btnBuscarLogo.Name = "btnBuscarLogo";
            this.btnBuscarLogo.Size = new System.Drawing.Size(40, 23);
            this.btnBuscarLogo.TabIndex = 4;
            this.btnBuscarLogo.Text = "...";
            this.btnBuscarLogo.UseVisualStyleBackColor = false;
            this.btnBuscarLogo.Click += new System.EventHandler(this.btnBuscarLogo_Click);

            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.White;
            this.picLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLogo.Location = new System.Drawing.Point(470, 60);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(300, 200);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 5;
            this.picLogo.TabStop = false;

            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.Green;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(470, 280);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(120, 40);
            this.btnGuardar.TabIndex = 6;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.Maroon;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(650, 280);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(120, 40);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            // 
            // frmConfiguracionTicket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(800, 360);
            this.Controls.Add(this.grpEncabezado);
            this.Controls.Add(this.grpPie);
            this.Controls.Add(this.lblLogo);
            this.Controls.Add(this.txtLogoPath);
            this.Controls.Add(this.btnBuscarLogo);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "frmConfiguracionTicket";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración de Ticket";
            this.Load += new System.EventHandler(this.FrmConfiguracion_Load);

            this.grpEncabezado.ResumeLayout(false);
            this.grpEncabezado.PerformLayout();
            this.grpPie.ResumeLayout(false);
            this.grpPie.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
