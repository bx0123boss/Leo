using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmPrecio : Form
    {
        public string tipo { get; set; }
        public frmPrecio()
        {
            InitializeComponent();
        }

        private void btnMayoreo_Click(object sender, EventArgs e)
        {
            tipo = "MAY";
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnGeneral_Click(object sender, EventArgs e)
        {
            tipo = "GEN";
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tipo = "ESP";
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void frmPrecio_Load(object sender, EventArgs e)
        {
            //PRECIO PUBLICO GENERAL
            if (Conexion.lugar == "LEO" || Conexion.lugar == "SANJUAN")
            {
                tipo = "GEN";
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            
        }
    }
}
