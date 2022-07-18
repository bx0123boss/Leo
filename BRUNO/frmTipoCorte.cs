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
    public partial class frmTipoCorte : Form
    {
        
        public frmTipoCorte()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmProveedores))
                {
                    MessageBox.Show("Ya existe un modulo de corte abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmCorte cort = new frmCorte();
                cort.Show();
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmProveedores))
                {
                    MessageBox.Show("Ya existe un modulo de historial de cortes abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmHistorialCortes histo = new frmHistorialCortes();
                histo.Show();
                this.Close();
            }
        }

        private void frmTipoCorte_Load(object sender, EventArgs e)
        {

        }
    }
}
