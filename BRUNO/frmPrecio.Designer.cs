namespace BRUNO
{
    partial class frmPrecio
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
            this.btnMayoreo = new System.Windows.Forms.Button();
            this.btnGeneral = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMayoreo
            // 
            this.btnMayoreo.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMayoreo.Location = new System.Drawing.Point(26, 96);
            this.btnMayoreo.Name = "btnMayoreo";
            this.btnMayoreo.Size = new System.Drawing.Size(229, 46);
            this.btnMayoreo.TabIndex = 7;
            this.btnMayoreo.Text = "Mayoreo";
            this.btnMayoreo.UseVisualStyleBackColor = true;
            this.btnMayoreo.Click += new System.EventHandler(this.btnMayoreo_Click);
            // 
            // btnGeneral
            // 
            this.btnGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGeneral.Location = new System.Drawing.Point(26, 12);
            this.btnGeneral.Name = "btnGeneral";
            this.btnGeneral.Size = new System.Drawing.Size(229, 46);
            this.btnGeneral.TabIndex = 6;
            this.btnGeneral.Text = "Publico General";
            this.btnGeneral.UseVisualStyleBackColor = true;
            this.btnGeneral.Click += new System.EventHandler(this.btnGeneral_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(26, 175);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(229, 46);
            this.button1.TabIndex = 8;
            this.button1.Text = "Especial";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmPrecio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(284, 167);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnMayoreo);
            this.Controls.Add(this.btnGeneral);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmPrecio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tipo de Precio";
            this.Load += new System.EventHandler(this.frmPrecio_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMayoreo;
        private System.Windows.Forms.Button btnGeneral;
        private System.Windows.Forms.Button button1;
    }
}