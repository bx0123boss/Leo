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
    public partial class frmTipoInventario : Form
    {
        public String usuario = "Admin";
        public frmTipoInventario()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmInventario invent = new frmInventario();
            invent.usuario = usuario;
            invent.Show();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmAccesorios acces = new frmAccesorios();
            acces.usuario = usuario;
            acces.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmPerfumeria invent = new frmPerfumeria();
            invent.usuario = usuario;
            invent.Show();
            this.Close();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmRelojeria invent = new frmRelojeria();
            invent.usuario = usuario;
            invent.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmJoyeria invent = new frmJoyeria();
            invent.usuario = usuario;
            invent.Show();
            this.Close();
        }
    }
}
