// Pantallas de recetas del usuario: Inicio (recetas aptas para su perfil) y Detalle.
using System.Globalization;
using CenaApp.Web.Data;
using CenaApp.Web.Models;
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

    // GET /Recetas/Detalle/5 → la receta, o la vista "no encontrada" con código 404 (D-17).
    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var receta = await _recetas.ObtenerAsync(id);
        if (receta is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View("NoEncontrada", id);
        }

        var usuario = await _db.Usuarios
            .Include(u => u.Alergenos)
            .FirstAsync(u => u.Id == UsuarioActual.Id(User));

        var alergenos = RecetaService.CalcularAlergenos(receta);
        var modelo = new DetalleRecetaViewModel
        {
            Id = receta.Id,
            Nombre = receta.Nombre,
            Descripcion = receta.Descripcion,
            Emoji = receta.Emoji,
            Categoria = receta.Categoria.Nombre,
            TiempoMin = receta.TiempoMin,
            Porciones = receta.Porciones,
            Calorias = receta.Calorias,
            Dificultad = receta.Dificultad,
            Alergenos = alergenos,
            AlergenosDelUsuario = alergenos.Intersect(usuario.Alergenos.Select(a => a.Nombre)).ToList(),
            AptaPara = RecetaService.RestriccionesAptas(receta, await _db.Restricciones.ToListAsync()),
            Ingredientes = receta.Ingredientes
                .OrderBy(ri => ri.Ingrediente.Nombre)
                .Select(ri => (ri.Ingrediente.Nombre, TextoCantidad(ri)))
                .ToList(),
            Pasos = receta.Pasos.OrderBy(p => p.Orden).Select(p => p.Texto).ToList(),
        };
        return View(modelo);
    }

    // "200 g", "0,5 ud" o "a gusto".
    private static string TextoCantidad(RecetaIngrediente ri)
    {
        if (ri.Cantidad is null)
        {
            return "a gusto";
        }
        var numero = ri.Cantidad.Value.ToString("0.##", CultureInfo.GetCultureInfo("es-AR"));
        return string.IsNullOrEmpty(ri.Unidad) ? numero : $"{numero} {ri.Unidad}";
    }
}
