using System;
using System.Windows.Forms;

namespace JaegerSoft
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Conexion.CargarConfiguracion(1);
            Application.Run(new frmLogin());
            
        }
    }
}
