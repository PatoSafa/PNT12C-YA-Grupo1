// Configura la aplicación: servicios (inyección de dependencias), base de datos y rutas MVC.
using CenaApp.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// La cadena de conexión sale de appsettings.Development.json en local
// y de la variable de entorno ConnectionStrings__CenaApp en producción.
var cadenaConexion = builder.Configuration.GetConnectionString("CenaApp")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'CenaApp'.");
builder.Services.AddDbContext<AppDbContext>(opciones => opciones.UseNpgsql(cadenaConexion));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
