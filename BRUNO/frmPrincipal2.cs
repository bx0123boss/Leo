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
    public partial class frmPrincipal2 : Form
    {
        public frmPrincipal2()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmConfiguracion confi = new frmConfiguracion();
            confi.Show();
        }

        private void BtnInventario_Click(object sender, EventArgs e)
        {
            frmTipoInventario invent = new frmTipoInventario();
            invent.usuario = "Admin";
            invent.Show();
        }

        private void frmPrincipal2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
