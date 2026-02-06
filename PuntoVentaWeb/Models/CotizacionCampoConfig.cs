namespace PuntoVentaWeb.Models
{
    public class CotizacionCampoConfig
    {
        public int Id { get; set; }
        public string NombreEtiqueta { get; set; }
        public string TipoDato { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; } = true;
    }
}