using System;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmOpcionesImpresionCorte : Form
    {
        public bool ImprimirResumen { get; private set; }
        public bool ImprimirFoliosDetallados { get; private set; }
        public bool ImprimirCategorias { get; private set; }
        public bool ImprimirGrupos { get; private set; }
        public bool ImprimirSalidas { get; private set; }

        public frmOpcionesImpresionCorte(bool tienePermisoDetallado, bool haySalidas)
        {
            InitializeComponent();

            // Mostrar opción de folios detallados solo si tiene el permiso
            chkFoliosDetallados.Visible = tienePermisoDetallado;
            chkFoliosDetallados.Checked = false;

            // Si no hubo salidas en el turno, desmarcar por defecto
            chkSalidas.Checked = haySalidas;
            if (!haySalidas)
            {
                chkSalidas.Text += " (Sin movimientos)";
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            ImprimirResumen = chkResumen.Checked;
            ImprimirFoliosDetallados = chkFoliosDetallados.Visible && chkFoliosDetallados.Checked;
            ImprimirCategorias = chkCategorias.Checked;
            ImprimirGrupos = chkGrupos.Checked;
            ImprimirSalidas = chkSalidas.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnNoImprimir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}