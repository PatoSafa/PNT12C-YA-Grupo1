// Configura la aplicación: servicios (inyección de dependencias), base de datos y rutas MVC.
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using CenaApp.Web.Data;
using CenaApp.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.WebEncoders;
using Microsoft.EntityFrameworkCore;

// Números con punto decimal en formularios y JSON, igual en cualquier máquina: un <input type="number">
// manda "0.5", y con la configuración regional en español se leería como 5. Los textos que se muestran
// con coma ("0,5 ud") se formatean explícitamente con es-AR.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Render indica en la variable PORT en qué puerto escuchar. Es lo mismo que server.port: ${PORT:8080} en Spring (Clase 6).
// En local no existe, y se usa el de launchSettings.json (5224).
var puerto = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(puerto))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{puerto}");
}

// En Render la app está detrás de un proxy que atiende el HTTPS. Estos encabezados le avisan a la app que
// la petición original era https (si no, creería que es http y la redirección a HTTPS no terminaría nunca).
builder.Services.Configure<ForwardedHeadersOptions>(opciones =>
{
    opciones.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    opciones.KnownIPNetworks.Clear();
    opciones.KnownProxies.Clear();
});

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
builder.Services.AddScoped<IIngredienteService, IngredienteService>();
// Sin estado ni base de datos: una sola instancia para toda la app.
builder.Services.AddSingleton<IComercioService, ComercioService>();

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

app.UseForwardedHeaders();

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
