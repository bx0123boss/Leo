namespace PuntoVentaWeb.Models;
public class Cotizacion
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string ClienteNombre { get; set; }
    public List<DetalleCotizacion> Detalles { get; set; } = new List<DetalleCotizacion>();
    public decimal Total => Detalles.Sum(d => d.Importe);
}

public class DetalleCotizacion
{
    public string ProductoCodigo { get; set; }
    public string Descripcion { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Importe => Cantidad * PrecioUnitario;
} 