namespace JaegerSoft
{
    partial class frmClientesCredito
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmClientesCredito));
            this.lblAdeudo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.flowLayoutPanelRight = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanelMain.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.flowLayoutPanelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 24);
            this.label1.TabIndex = 36;
            this.label1.Text = "Buscar";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(89, 8);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(819, 29);
            this.textBox1.TabIndex = 35;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.Black;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(733, 558);
            this.dataGridView1.TabIndex = 30;
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.button5.Location = new System.Drawing.Point(3, 3);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(156, 59);
            this.button5.TabIndex = 37;
            this.button5.Text = "Historial de ventas";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.button7.Location = new System.Drawing.Point(3, 68);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(156, 59);
            this.button7.TabIndex = 38;
            this.button7.Text = "Historial de Abonos";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.button6.Location = new System.Drawing.Point(3, 133);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(156, 59);
            this.button6.TabIndex = 39;
            this.button6.Text = "Impresión Adeudo Clientes";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 205);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 24);
            this.label2.TabIndex = 40;
            this.label2.Text = "Adeudo Total:";
            // 
            // lblAdeudo
            // 
            this.lblAdeudo.AutoSize = true;
            this.lblAdeudo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAdeudo.ForeColor = System.Drawing.Color.White;
            this.lblAdeudo.Location = new System.Drawing.Point(3, 239);
            this.lblAdeudo.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.lblAdeudo.Name = "lblAdeudo";
            this.lblAdeudo.Size = new System.Drawing.Size(21, 24);
            this.lblAdeudo.TabIndex = 41;
            this.lblAdeudo.Text = "0";
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.button4.Location = new System.Drawing.Point(3, 276);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(156, 59);
            this.button4.TabIndex = 34;
            this.button4.Text = "Abono a cuenta de cliente";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(3, 341);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(156, 74);
            this.button3.TabIndex = 33;
            this.button3.Text = "Eliminar";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelMain.ColumnCount = 2;
            // Reemplaza las líneas erróneas por estas:
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.tableLayoutPanelMain.Controls.Add(this.dataGridView1, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.flowLayoutPanelRight, 1, 0);
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(12, 51);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 1;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(915, 564);
            this.tableLayoutPanelMain.TabIndex = 42;
            // 
            // panelTop
            // 
            this.panelTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.textBox1);
            this.panelTop.Location = new System.Drawing.Point(12, 2);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(915, 43);
            this.panelTop.TabIndex = 43;
            // 
            // flowLayoutPanelRight
            // 
            this.flowLayoutPanelRight.Controls.Add(this.button5);
            this.flowLayoutPanelRight.Controls.Add(this.button7);
            this.flowLayoutPanelRight.Controls.Add(this.button6);
            this.flowLayoutPanelRight.Controls.Add(this.label2);
            this.flowLayoutPanelRight.Controls.Add(this.lblAdeudo);
            this.flowLayoutPanelRight.Controls.Add(this.button4);
            this.flowLayoutPanelRight.Controls.Add(this.button3);
            this.flowLayoutPanelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelRight.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelRight.Location = new System.Drawing.Point(742, 3);
            this.flowLayoutPanelRight.Name = "flowLayoutPanelRight";
            this.flowLayoutPanelRight.Size = new System.Drawing.Size(170, 558);
            this.flowLayoutPanelRight.TabIndex = 31;
            this.flowLayoutPanelRight.WrapContents = false;
            // 
            // frmClientesCredito
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(939, 627);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "frmClientesCredito";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Clientes con deuda a credito";
            this.Load += new System.EventHandler(this.frmClientesCredito_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.flowLayoutPanelRight.ResumeLayout(false);
            this.flowLayoutPanelRight.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblAdeudo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelRight;
    }
}