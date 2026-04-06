using PuntoVentaWeb.Data;
using PuntoVentaWeb.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(opciones =>
{
    opciones.ListenAnyIP(5000); // Escucha HTTP normal en toda la red
    opciones.ListenAnyIP(5001, opcionesEscucha =>
    {
        // Escucha HTTPS y le decimos exactamente dónde está el certificado
        opcionesEscucha.UseHttps("certificado_pv.pfx", "MiClave123");
    });
});
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<PuntoVentaWeb.Services.CotizacionService>();
builder.Services.AddScoped<PuntoVentaWeb.Services.EmailService>();
builder.Services.AddScoped<PuntoVentaWeb.Services.ClienteService>();
builder.Services.AddScoped<PuntoVentaWeb.Services.ConfiguracionService>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped(sp => new HttpClient());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// COMENTADO:
// app.UseHttpsRedirection(); 

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();