using System.Collections.Generic;

namespace BRUNO
{
    public static class Sesion
    {
        public static string IdUsuario { get; set; } = "";
        public static string NombreUsuario { get; set; } = "";

        // AQUÍ ESTÁ LA MAGIA: Una lista que guardará los permisos traídos de la BD
        public static List<string> PermisosActuales { get; set; } = new List<string>();

        // Método centralizado para preguntar por permisos
        public static bool TienePermiso(string accion)
        {
            // 1. Si en su lista de permisos tiene el de Administrador Total, entra directo.
            if (PermisosActuales.Contains("ADMIN_TODO")) return true;

            // 2. Si no es admin, verificamos si la lista contiene el permiso exacto que estamos consultando.
            return PermisosActuales.Contains(accion.ToUpper());
        }

        // Limpiar la sesión al cerrar el programa o cambiar de usuario
        public static void CerrarSesion()
        {
            IdUsuario = "";
            NombreUsuario = "";
            PermisosActuales.Clear();
        }
    }
}