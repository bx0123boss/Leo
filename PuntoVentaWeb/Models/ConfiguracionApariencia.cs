namespace PuntoVentaWeb.Models
{
    public class ConfiguracionApariencia
    {
        public int Id { get; set; }
        public string ColorPrimario { get; set; }
        public string ColorSecundario { get; set; }
        public int DiasValidez { get; set; } = 15;
    }
}
