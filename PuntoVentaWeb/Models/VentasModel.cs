namespace PuntoVentaWeb.Models;

public class CategoriaVenta
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Color { get; set; } = "0d6efd";
    public string Letra { get; set; } = "#ffffff";
}

public class ProductoVenta
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public double Precio { get; set; }
    public double CostoTotal { get; set; }
    public double Existencia { get; set; }
    public string ImagenUrl { get; set; } = "";
    public int IdCategoria { get; set; }
    public string Categoria { get; set; } = "";
    public bool Comanda { get; set; }
    public string Origen { get; set; } = "";
    public double Iva { get; set; }
}

public class ItemCarritoVenta
{
    public string ProductoId { get; set; } = "";
    public string Nombre { get; set; } = "";
    public double Precio { get; set; }
    public double Cantidad { get; set; } = 1;
    public double Costo { get; set; }
    public string Comentario { get; set; } = "";
    public string Categoria { get; set; } = "";
    public double Existencia { get; set; }
    public double Iva { get; set; }
    public double Subtotal => Math.Round(Precio * Cantidad, 2);
}

public class VentaGuardarRequest
{
    public string TipoVenta { get; set; } = "CONTADO"; // CONTADO, CREDITO, APARTADO
    public string Folio { get; set; } = "";
    public string IdCliente { get; set; } = "0";
    public string NombreCliente { get; set; } = "PUBLICO EN GENERAL";
    public string DireccionCliente { get; set; } = "";
    public string TelefonoCliente { get; set; } = "";
    public double Subtotal { get; set; }
    public double Descuento { get; set; }
    public double Total { get; set; }
    public string MetodoPago { get; set; } = "Efectivo";
    public double MontoRecibido { get; set; }
    public double Cambio { get; set; }
    public double AbonoInicial { get; set; }
    public double Restante { get; set; }
    public string Observaciones { get; set; } = "";
    public string Usuario { get; set; } = "Admin";
    public string IdUsuario { get; set; } = "1";
    public List<ItemCarritoVenta> Items { get; set; } = new();
    public Dictionary<string, double> PagosDetalle { get; set; } = new();
}

public class ClienteVentaDto
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Direccion { get; set; } = "";
    public string Correo { get; set; } = "";
    public double Adeudo { get; set; }
    public double Limite { get; set; }
}
