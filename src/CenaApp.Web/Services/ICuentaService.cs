// Contrato del servicio de cuentas: registro y verificación de credenciales.
using CenaApp.Web.Models;

namespace CenaApp.Web.Services;

public interface ICuentaService
{
    // Crea el usuario con la contraseña hasheada. Devuelve null si el email ya está registrado (VN-01).
    Task<Usuario?> RegistrarAsync(string email, string password);

    // Devuelve el usuario si el email y la contraseña son correctos; null si no.
    Task<Usuario?> ValidarCredencialesAsync(string email, string password);
}
