// Configura la aplicación: servicios (inyección de dependencias), base de datos y rutas MVC.
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using CenaApp.Web.Data;
using CenaApp.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.WebEncoders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Los enum viajan en el JSON como texto ("Facil") en vez de número (0).
builder.Services.AddControllersWithViews()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Las vistas escriben los acentos y la ñ tal cual en el HTML (por defecto los codifica como &#xF1;).
builder.Services.Configure<WebEncoderOptions>(o => o.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

// Services: una instancia por petición HTTP (igual que el AppDbContext que usan).
builder.Services.AddScoped<IRecetaService, RecetaService>();
builder.Services.AddScoped<ICuentaService, CuentaService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IDespensaService, DespensaService>();

// Login por cookie (sin Identity completo, ver CLAUDE.md §3). Si alguien sin sesión entra a una
// página con [Authorize], lo manda a /Cuenta/Ingresar; si no tiene el rol, a /Cuenta/AccesoDenegado.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/Cuenta/Ingresar";
        opciones.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        // La sesión vence tras 60 minutos sin uso; cada visita la renueva (RNF15).
        opciones.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        opciones.SlidingExpiration = true;
    });

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

// Primero se lee la cookie (quién es), después se controlan los permisos (qué puede hacer).
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
