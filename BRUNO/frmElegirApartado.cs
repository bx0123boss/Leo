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
    public partial class frmElegirApartado : Form
    {
        public String usuario = "";
        public frmElegirApartado()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmApartado))
                {
                    MessageBox.Show("Ya existe un modulo de apartado abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmApartado apart = new frmApartado();
                apart.usuario = usuario;
                apart.Show();
                this.Close();
            } 
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmApartados))
                {
                    MessageBox.Show("Ya existe un modulo de apartados abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmApartados apart = new frmApartados();
                apart.Show();
                this.Close();
            } 
        }
    }
}
