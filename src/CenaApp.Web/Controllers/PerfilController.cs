// Perfil del usuario: verlo, completarlo por primera vez (onboarding) y editarlo.
using CenaApp.Web.Services;
using CenaApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenaApp.Web.Controllers;

[Authorize]
public class PerfilController : Controller
{
    private readonly IPerfilService _perfiles;

    public PerfilController(IPerfilService perfiles)
    {
        _perfiles = perfiles;
    }

    // GET /Perfil → perfil en modo lectura; si todavía no lo completó, al onboarding.
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var usuario = await _perfiles.ObtenerAsync(UsuarioActual.Id(User));
        if (usuario is null)
        {
            return RedirectToAction("Ingresar", "Cuenta");
        }
        if (string.IsNullOrEmpty(usuario.Nombre))
        {
            return RedirectToAction(nameof(Editar));
        }

        var modelo = new PerfilViewModel
        {
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Nivel = usuario.Nivel,
            Restricciones = usuario.Restricciones.Select(r => r.Nombre).Order().ToList(),
            Alergenos = usuario.Alergenos.Select(a => a.Nombre).Order().ToList(),
            Gustos = usuario.Gustos.Select(g => g.Nombre).Order().ToList(),
        };
        return View(modelo);
    }

    // GET /Perfil/Editar → formulario con lo que el usuario ya tiene marcado.
    [HttpGet]
    public async Task<IActionResult> Editar()
    {
        var usuario = await _perfiles.ObtenerAsync(UsuarioActual.Id(User));
        if (usuario is null)
        {
            return RedirectToAction("Ingresar", "Cuenta");
        }

        var formulario = new PerfilFormularioViewModel
        {
            Nombre = usuario.Nombre,
            Nivel = usuario.Nivel,
            RestriccionesIds = usuario.Restricciones.Select(r => r.Id).ToList(),
            AlergenosIds = usuario.Alergenos.Select(a => a.Id).ToList(),
            GustosIds = usuario.Gustos.Select(g => g.Id).ToList(),
            EsOnboarding = string.IsNullOrEmpty(usuario.Nombre),
        };
        await _perfiles.CargarOpcionesAsync(formulario);
        return View(formulario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(PerfilFormularioViewModel formulario)
    {
        if (!ModelState.IsValid)
        {
            // Las opciones no viajan en el POST: se vuelven a cargar para redibujar el formulario con los errores.
            await _perfiles.CargarOpcionesAsync(formulario);
            return View(formulario);
        }

        await _perfiles.GuardarAsync(UsuarioActual.Id(User), formulario);

        // Después del onboarding va directo a las recetas; después de editar, vuelve al perfil.
        return formulario.EsOnboarding
            ? RedirectToAction("Index", "Home")
            : RedirectToAction(nameof(Index));
    }
}
