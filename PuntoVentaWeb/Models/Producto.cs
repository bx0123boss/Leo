namespace PuntoVentaWeb.Models;
public class Producto
{
    public string Id { get; set; } // Asumo que es numérico en Access
    public string Nombre { get; set; }
    public decimal PrecioventaMayoreo { get; set; }
    public decimal Precioventa { get; set; } // Ojo con mayúsculas/minúsculas, en C# usamos PascalCase
    public double Existencia { get; set; } // Double por si vendes a granel (kilos, metros)
    public double Limite { get; set; }     // "Límite"
    public string Categoria { get; set; }  // "Categoría"
    public decimal Especial { get; set; } 
    public string Iva { get; set; }
    public string Unidad { get; set; }
    public string Uni { get; set; }
}