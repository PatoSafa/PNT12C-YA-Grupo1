// Pantallas de recetas del usuario: Inicio (recetas aptas para su perfil).
using CenaApp.Web.Data;
using CenaApp.Web.Services;
using CenaApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CenaApp.Web.Controllers;

[Authorize]
public class RecetasController : Controller
{
    private readonly IRecetaService _recetas;
    private readonly AppDbContext _db;

    public RecetasController(IRecetaService recetas, AppDbContext db)
    {
        _recetas = recetas;
        _db = db;
    }

    // GET /Recetas?buscar=pollo&gusto=3
    [HttpGet]
    public async Task<IActionResult> Index(string? buscar, int? gusto)
    {
        var usuarioId = UsuarioActual.Id(User);
        var usuario = await _db.Usuarios.FindAsync(usuarioId);
        if (usuario is null)
        {
            return RedirectToAction("Ingresar", "Cuenta");
        }

        var gustos = await _db.Gustos.OrderBy(g => g.Tipo).ThenBy(g => g.Nombre).ToListAsync();
        var recetas = await _recetas.ListarParaUsuarioAsync(usuarioId, buscar, gusto);

        var modelo = new InicioRecetasViewModel
        {
            NombreUsuario = usuario.Nombre,
            Buscar = buscar,
            GustoId = gusto,
            GustoElegido = gustos.FirstOrDefault(g => g.Id == gusto)?.Nombre,
            Gustos = gustos,
            Recetas = recetas.Select(r => new TarjetaRecetaViewModel
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                Emoji = r.Emoji,
                TiempoMin = r.TiempoMin,
                Porciones = r.Porciones,
                Dificultad = r.Dificultad,
            }).ToList(),
        };
        return View(modelo);
    }
}
