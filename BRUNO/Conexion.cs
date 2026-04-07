using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRUNO
{
    class Conexion
    {
        public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Jaeger Soft\Jaeger.accdb; Jet OLEDB:Database Password=75941232";
        public static string CadSQL = @"Data Source=.\SQLEXPRESS;Initial Catalog=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;";

        //63public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.1.1\Jaeger Soft\Jaeger.accdb; Jet OLEDB:Database Password=75941232";
        //public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Respaldo\Joyeria.accdb";
        //public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb";
        //public static string lugar = "LOCAL";

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
        public static bool PuntoB;
        public static void CargarConfiguracion(int id)
        {
            using (var conexion = new OleDbConnection(CadCon))
            {
                conexion.Open();
                string query = "SELECT * FROM Configuracion WHERE Id = @id";
                using (var cmd = new OleDbCommand(query, conexion))
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
                            Whatsapp = reader["PieDeTicket"].ToString();
                            Bascula = Convert.ToBoolean(reader["Bascula"].ToString());
                            PuntoB = Convert.ToBoolean(reader["PuntoB"].ToString());
                        }
                    }
                }
            }
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          