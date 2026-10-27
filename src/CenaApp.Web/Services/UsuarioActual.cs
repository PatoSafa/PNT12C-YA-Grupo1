// Lee de la cookie de sesión quién es el usuario que hizo la petición.
using System.Security.Claims;

namespace CenaApp.Web.Services;

public static class UsuarioActual
{
    // Id del usuario logueado (se guardó como claim NameIdentifier al iniciar sesión, ver CuentaController).
    public static int Id(ClaimsPrincipal usuario) =>
        int.Parse(usuario.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
