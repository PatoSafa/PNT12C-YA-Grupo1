// Registro, inicio y cierre de sesión. La sesión es una cookie firmada por el servidor con el id, el nombre y el rol.
using System.Security.Claims;
using CenaApp.Web.Models;
using CenaApp.Web.Services;
using CenaApp.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenaApp.Web.Controllers;

public class CuentaController : Controller
{
    private readonly ICuentaService _cuentas;

    public CuentaController(ICuentaService cuentas)
    {
        _cuentas = cuentas;
    }

    [HttpGet]
    public IActionResult Registrarse()
    {
        return View(new RegistroViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrarse(RegistroViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var usuario = await _cuentas.RegistrarAsync(modelo.Email, modelo.Password);
        if (usuario is null)
        {
            ModelState.AddModelError(nameof(modelo.Email), "Ya existe una cuenta con ese email.");
            return View(modelo);
        }

        await IniciarSesionAsync(usuario);
        // Cuenta nueva: primero completa su perfil (onboarding).
        return RedirectToAction("Index", "Perfil");
    }

    [HttpGet]
    public IActionResult Ingresar(string? returnUrl = null)
    {
        return View(new IngresoViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ingresar(IngresoViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var usuario = await _cuentas.ValidarCredencialesAsync(modelo.Email, modelo.Password);
        if (usuario is null)
        {
            // Mismo mensaje si el email no existe o si la contraseña está mal: no revela qué cuentas existen (RNF16).
            ModelState.AddModelError("", "Email o contraseña incorrectos.");
            return View(modelo);
        }

        await IniciarSesionAsync(usuario);

        // Solo se vuelve a direcciones de esta misma app (evita redirigir a otro sitio).
        if (!string.IsNullOrEmpty(modelo.ReturnUrl) && Url.IsLocalUrl(modelo.ReturnUrl))
        {
            return LocalRedirect(modelo.ReturnUrl);
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Salir()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Ingresar));
    }

    [HttpGet]
    public IActionResult AccesoDenegado()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }

    // Arma la cookie de sesión con los datos que se necesitan en cada petición (claims).
    private async Task IniciarSesionAsync(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, string.IsNullOrEmpty(usuario.Nombre) ? usuario.Email : usuario.Nombre),
            new(ClaimTypes.Role, usuario.Rol.ToString()),
        };
        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));
    }
}
