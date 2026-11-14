using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaegerSoft
{
    public class Conexion
    {
        public static string CadSQL = @"Data Source=.\SQLEXPRESS;Initial Catalog=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;";
        public static string CadCon = CadSQL; // Redirigido a SQL Server al 100%
        
        public static string lugar;
        public static string[] datosTicket;
        public static string[] pieDeTicket;
        public static string logoPath;
        public static string Font;
        public static string impresora;
        public static int MaxChar;
        public static int FontSize;
        public static int MaxCharDescription;
        public static bool ConIva;
        public static bool impresionMediaCarta;
        public static string Whatsapp;
        public static bool Bascula;
        public static bool Ticket;
        public static bool PuntoB;
        public static bool AbrirTeclado { get; internal set; } = false;

        public static void CargarConfiguracion(int id)
        {
            using (var conexion = new SqlConnection(CadSQL))
            {
                conexion.Open();
                string query = "SELECT * FROM Configuracion WHERE Id = @id";
                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lugar = reader["Lugar"].ToString();
                            Font = reader["Font"].ToString();
                            impresora = reader["Impresora"].ToString();
                            MaxChar = Convert.ToInt32(reader["MaxChar"]);
                            FontSize = Convert.ToInt32(reader["FontSize"]);
                            MaxCharDescription = Convert.ToInt32(reader["MaxCharDescription"]);
                            ConIva = Convert.ToBoolean(reader["ConIva"]);
                            logoPath = reader["logoPath"].ToString();
                            datosTicket = reader["DatosTicket"].ToString().Split('|');
                            pieDeTicket = reader["PieDeTicket"].ToString().Split('|');
                            impresionMediaCarta = Convert.ToBoolean(reader["MediaCarta"]);
                            Whatsapp = reader["Whatsapp"].ToString();
                            Bascula = Convert.ToBoolean(reader["Bascula"].ToString());
                            PuntoB = Convert.ToBoolean(reader["PuntoB"].ToString());
                            Ticket = Convert.ToBoolean(reader["Ticket"].ToString());
                            AbrirTeclado = Convert.ToBoolean(reader["AbrirTeclado"].ToString());
                        }
                    }
                }
            }
        }
    }
}
