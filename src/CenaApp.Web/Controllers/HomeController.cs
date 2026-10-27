// Punto de entrada "/": manda a cada uno a donde corresponde según su sesión. También muestra la página de error.
using System.Diagnostics;
using CenaApp.Web.Data;
using CenaApp.Web.Models;
using CenaApp.Web.Services;
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

        var usuario = await _db.Usuarios.FindAsync(UsuarioActual.Id(User));
        if (usuario is null)
        {
            return RedirectToAction("Ingresar", "Cuenta");
        }

        // Cuenta recién creada, sin perfil → onboarding. Si no, al Inicio de recetas.
        if (usuario.Rol == Rol.Usuario && string.IsNullOrEmpty(usuario.Nombre))
        {
            return RedirectToAction("Index", "Perfil");
        }
        return RedirectToAction("Index", "Recetas");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
