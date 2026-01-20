namespace PuntoVentaWeb.Models;
public class Cotizacion
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; }
    public string Observaciones { get; set; }
    private decimal _totalDb; 

    public decimal Total
    {
        get
        {
            if (Detalles != null && Detalles.Count > 0)
                return Detalles.Sum(d => d.Importe);

            return _totalDb;
        }
        set
        {
            _totalDb = value; 
        }
    }
    // -------------------------
    public List<DetalleCotizacion> Detalles { get; set; } = new List<DetalleCotizacion>();
}

public class DetalleCotizacion
{
    public string ProductoCodigo { get; set; }
    public string Descripcion { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Importe => Cantidad * PrecioUnitario;
} 