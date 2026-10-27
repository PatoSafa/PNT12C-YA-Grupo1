// Configura la aplicación: servicios (inyección de dependencias), base de datos y rutas MVC.
using System.Text.Json.Serialization;
using CenaApp.Web.Data;
using CenaApp.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Los enum viajan en el JSON como texto ("Facil") en vez de número (0).
builder.Services.AddControllersWithViews()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Services: una instancia por petición HTTP (igual que el AppDbContext que usan).
builder.Services.AddScoped<IRecetaService, RecetaService>();

// La cadena de conexión sale de appsettings.Development.json en local
// y de la variable de entorno ConnectionStrings__CenaApp en producción.
var cadenaConexion = builder.Configuration.GetConnectionString("CenaApp")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'CenaApp'.");
builder.Services.AddDbContext<AppDbContext>(opciones => opciones.UseNpgsql(cadenaConexion));

var app = builder.Build();

// Al arrancar: aplica las migraciones pendientes y carga los datos iniciales si la base está vacía.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DatosIniciales.Cargar(db, app.Configuration);
}

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
