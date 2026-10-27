// Punto de entrada "/": manda a cada uno a donde corresponde según su sesión. También muestra la página de error.
using System.Diagnostics;
using CenaApp.Web.Data;
using CenaApp.Web.Models;
using CenaApp.Web.Services;
using CenaApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CenaApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        // Sin sesión → login.
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction("Ingresar", "Cuenta");
        }

        // Cuenta recién creada, sin perfil → onboarding.
        var usuario = await _db.Usuarios.FindAsync(UsuarioActual.Id(User));
        if (usuario is null)
        {
            return RedirectToAction("Ingresar", "Cuenta");
        }
        if (usuario.Rol == Rol.Usuario && string.IsNullOrEmpty(usuario.Nombre))
        {
            return RedirectToAction("Index", "Perfil");
        }

        // Provisorio hasta que exista el Inicio de recetas (T12).
        var modelo = new InicioViewModel
        {
            Nombre = usuario.Nombre,
            EsAdmin = usuario.Rol == Rol.Admin,
            BaseConectada = await _db.Database.CanConnectAsync()
        };
        return View(modelo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
