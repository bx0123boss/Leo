using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmAsignarPermisos : frmBase
    {
        public string IdUsuarioSeleccionado { get; set; } = "4";
        public string NombreUsuarioSeleccionado { get; set; }

        // Variable de control para evitar bucles infinitos al marcar casillas por código
        private bool _isUpdating = false;

        // Lista de todos los módulos disponibles en el sistema
        private Dictionary<string, string> ModulosDisponibles = new Dictionary<string, string>
        {
            { "ADMIN_TODO", "★ ACCESO TOTAL (Administrador) ★" },
            { "MOD_VENTAS", "Módulo de Ventas / Cobro" },
            { "MOD_VENTAS_CREDITO", "Módulo de Ventas Credito" },
            { "MOD_INVENTARIO", "Módulo de Inventario" },
            { "EXISTENCIAS", "Ver existencias actuales" },
            { "MOD_DEVOLUCIONES", "Módulo de Devoluciones" },
            { "MOD_CLIENTES", "Gestión de Clientes" },
            { "MOD_ENTRADAS", "Entradas / Depósitos" },
            { "MOD_SALIDAS", "Salidas / Retiros" },
            { "MOD_PROVEEDORES", "Proveedores / Cotizaciones" },
            { "MOD_USUARIOS", "Gestión de Usuarios" },
            { "MOD_APARTADOS", "Módulo de Apartados" },
            { "MOD_CORTES", "Cortes de Caja" },
            { "HISTORIAL_CORTES", "Historial de Cortes de Caja" },
            { "IMPRESION_CORTE", "Imprimir corte detallado"},
            { "MOD_REPORTES", "Reportes y Estadísticas" },
            { "MOD_CONFIGURACION", "Configuración General y Ticket" },
            { "MOD_WEB", "Módulo Web (Cotizador)" },
        };

        public frmAsignarPermisos()
        {
            InitializeComponent();

            // Suscribimos el evento que detecta cuando marcan/desmarcan una casilla
            chkPermisos.ItemCheck += ChkPermisos_ItemCheck;
        }

        private void frmAsignarPermisos_Load(object sender, EventArgs e)
        {
            this.Text = "Permisos de: " + NombreUsuarioSeleccionado;
            EstilizarBotonPrimario(this.btnGuardar);
            CargarListaDeModulos();
            MarcarPermisosActuales();
        }

        private void CargarListaDeModulos()
        {
            chkPermisos.Items.Clear();
            foreach (var modulo in ModulosDisponibles)
            {
                chkPermisos.Items.Add(modulo.Value);
            }
        }

        private void MarcarPermisosActuales()
        {
            try
            {
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();
                    string query = "SELECT Permiso FROM PermisosUsuario WHERE IdUsuario = '" + IdUsuarioSeleccionado + "'";
                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        // Pausamos el evento ItemCheck temporalmente mientras leemos de la BD
                        _isUpdating = true;

                        while (reader.Read())
                        {
                            string permisoBD = reader["Permiso"].ToString();

                            if (ModulosDisponibles.ContainsKey(permisoBD))
                            {
                                string descripcionVisible = ModulosDisponibles[permisoBD];
                                int index = chkPermisos.Items.IndexOf(descripcionVisible);
                                if (index != -1)
                                {
                                    chkPermisos.SetItemChecked(index, true);
                                }
                            }
                        }

                        _isUpdating = false; // Reactivamos el evento
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando permisos: " + ex.Message);
            }
        }

        // ===================================================================================
        // EVENTO MÁGICO: Si marcan "ACCESO TOTAL", marcamos todo. Si lo desmarcan, limpiamos.
        // ===================================================================================
        private void ChkPermisos_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Si el sistema está actualizando las casillas por código, ignoramos el evento
            if (_isUpdating) return;

            string itemSeleccionado = chkPermisos.Items[e.Index].ToString();
            string nombreAccesoTotal = ModulosDisponibles["ADMIN_TODO"];

            // Verificamos si el check que el usuario acaba de tocar es el de Administrador
            if (itemSeleccionado == nombreAccesoTotal)
            {
                _isUpdating = true; // Bloqueamos el evento para que no se cicle

                bool marcarTodo = (e.NewValue == CheckState.Checked);

                // Recorremos todos los elementos de la lista y los ponemos igual que el "ACCESO TOTAL"
                for (int i = 0; i < chkPermisos.Items.Count; i++)
                {
                    if (i != e.Index) // Para no volver a marcar el de Admin que ya se está marcando
                    {
                        chkPermisos.SetItemChecked(i, marcarTodo);
                    }
                }

                _isUpdating = false; // Liberamos el evento
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();
                    OleDbTransaction trx = con.BeginTransaction();

                    try
                    {
                        // 1. Borramos todos los permisos viejos de este usuario
                        using (OleDbCommand cmdDel = new OleDbCommand("DELETE FROM PermisosUsuario WHERE IdUsuario = '" + IdUsuarioSeleccionado + "'", con, trx))
                        {
                            cmdDel.ExecuteNonQuery();
                        }

                        // 2. Verificamos si marcó la casilla de "ACCESO TOTAL"
                        bool esAdminTotal = false;
                        string nombreAccesoTotal = ModulosDisponibles["ADMIN_TODO"];

                        foreach (var item in chkPermisos.CheckedItems)
                        {
                            if (item.ToString() == nombreAccesoTotal)
                            {
                                esAdminTotal = true;
                                break;
                            }
                        }

                        // 3. Lógica de inserción inteligente
                        if (esAdminTotal)
                        {
                            // Si es Admin, guardamos SOLO un registro y nos ahorramos el resto
                            string queryInsert = "INSERT INTO PermisosUsuario (IdUsuario, Permiso) VALUES ('" + IdUsuarioSeleccionado + "', 'ADMIN_TODO')";
                            using (OleDbCommand cmdIns = new OleDbCommand(queryInsert, con, trx))
                            {
                                cmdIns.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Si no es admin, guardamos los módulos que haya palomeado individualmente
                            foreach (var item in chkPermisos.CheckedItems)
                            {
                                string clavePermiso = "";
                                foreach (var kvp in ModulosDisponibles)
                                {
                                    if (kvp.Value == item.ToString())
                                    {
                                        clavePermiso = kvp.Key;
                                        break;
                                    }
                                }

                                if (!string.IsNullOrEmpty(clavePermiso))
                                {
                                    string queryInsert = "INSERT INTO PermisosUsuario (IdUsuario, Permiso) VALUES ('" + IdUsuarioSeleccionado + "', '" + clavePermiso + "')";
                                    using (OleDbCommand cmdIns = new OleDbCommand(queryInsert, con, trx))
                                    {
                                        cmdIns.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        trx.Commit();
                        MessageBox.Show("Permisos actualizados con éxito.", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception)
                    {
                        trx.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error guardando permisos: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}