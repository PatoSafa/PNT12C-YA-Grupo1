// Registro y login: guarda el hash de la contraseña (nunca la contraseña) y lo verifica al ingresar.
using CenaApp.Web.Data;
using CenaApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CenaApp.Web.Services;

public class CuentaService : ICuentaService
{
    private readonly AppDbContext _db;

    // PasswordHasher viene con ASP.NET Core (no es Identity completo): genera un hash con sal (PBKDF2)
    // y sabe verificar una contraseña contra ese hash.
    private readonly PasswordHasher<Usuario> _hasher = new();

    public CuentaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Usuario?> RegistrarAsync(string email, string password)
    {
        var emailNormalizado = Normalizar(email);
        if (await _db.Usuarios.AnyAsync(u => u.Email == emailNormalizado))
        {
            return null;
        }

        // El nombre y el resto del perfil se completan en el onboarding.
        var usuario = new Usuario { Email = emailNormalizado, Rol = Rol.Usuario };
        usuario.PasswordHash = _hasher.HashPassword(usuario, password);

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario?> ValidarCredencialesAsync(string email, string password)
    {
        var emailNormalizado = Normalizar(email);
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == emailNormalizado);
        if (usuario is null)
        {
            return null;
        }

        var resultado = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, password);
        return resultado == PasswordVerificationResult.Failed ? null : usuario;
    }

    // El email se guarda y se busca en minúsculas: "Ana@Mail.com" y "ana@mail.com" son la misma cuenta (VN-01).
    private static string Normalizar(string email) => email.Trim().ToLowerInvariant();
}
