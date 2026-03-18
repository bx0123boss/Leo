using PuntoVentaWeb.Data;
using PuntoVentaWeb.Services;

var builder = WebApplication.CreateBuilder(args);

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