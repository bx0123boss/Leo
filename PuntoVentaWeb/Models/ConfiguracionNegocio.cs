namespace PuntoVentaWeb.Models;
public class ConfiguracionNegocio
{
    public string NombreLugar { get; set; } = "DEPORTES LEO";
    public string[] DatosTicket { get; set; } = Array.Empty<string>(); // Array para las líneas de dirección/RFC
    public string[] PieDeTicket { get; set; } = Array.Empty<string>(); // Array para despedida
    public bool ConIva { get; set; }
    public string LogoPath { get; set; } // Ruta local ej: C:\Jaeger Soft\logo.jpg
    public string CorreoEmisor { get; set; }
    public string PasswordEmisor { get; set; }
}