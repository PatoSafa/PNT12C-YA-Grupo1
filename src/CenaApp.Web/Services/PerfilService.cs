// Perfil del usuario: nombre, nivel y preferencias (restricciones, alergias y gustos) que filtran y ordenan las recetas.
using CenaApp.Web.Data;
using CenaApp.Web.Models;
using CenaApp.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CenaApp.Web.Services;

public class PerfilService : IPerfilService
{
    private readonly AppDbContext _db;

    public PerfilService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Usuario?> ObtenerAsync(int usuarioId)
    {
        return await _db.Usuarios
            .Include(u => u.Restricciones)
            .Include(u => u.Alergenos)
            .Include(u => u.Gustos)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Id == usuarioId);
    }

    public async Task CargarOpcionesAsync(PerfilFormularioViewModel formulario)
    {
        formulario.Restricciones = await _db.Restricciones.OrderBy(r => r.Nombre).ToListAsync();
        formulario.Alergenos = await _db.Alergenos.OrderBy(a => a.Nombre).ToListAsync();
        formulario.Gustos = await _db.Gustos.OrderBy(g => g.Tipo).ThenBy(g => g.Nombre).ToListAsync();
    }

    public async Task GuardarAsync(int usuarioId, PerfilFormularioViewModel formulario)
    {
        var usuario = await ObtenerAsync(usuarioId)
            ?? throw new InvalidOperationException($"No existe el usuario {usuarioId}.");

        usuario.Nombre = formulario.Nombre.Trim();
        usuario.Nivel = formulario.Nivel;

        // Se reemplazan las listas completas por lo que quedó marcado. EF Core calcula qué filas
        // de las tablas intermedias agregar y cuáles borrar.
        usuario.Restricciones = await _db.Restricciones.Where(r => formulario.RestriccionesIds.Contains(r.Id)).ToListAsync();
        usuario.Alergenos = await _db.Alergenos.Where(a => formulario.AlergenosIds.Contains(a.Id)).ToListAsync();
        usuario.Gustos = await _db.Gustos.Where(g => formulario.GustosIds.Contains(g.Id)).ToListAsync();

        await _db.SaveChangesAsync();
    }
}
