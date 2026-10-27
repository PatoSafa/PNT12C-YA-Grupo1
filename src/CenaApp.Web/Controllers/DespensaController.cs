// Mi Despensa: ingredientes que el usuario tiene en casa y qué puede cocinar con ellos.
// Agregar, quitar y vaciar se llaman con fetch (js/despensa.js) y responden la vista parcial _Contenido,
// que el JavaScript reemplaza en la página (D-07, Parte 15 del enunciado).
using System.Globalization;
using CenaApp.Web.Services;
using CenaApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenaApp.Web.Controllers;

[Authorize]
public class DespensaController : Controller
{
    // Accesos rápidos de la pantalla (RF17): se muestran los que el usuario todavía no tiene.
    private static readonly string[] IngredientesComunes =
        ["Huevo", "Papa", "Cebolla", "Tomate", "Arroz", "Zanahoria", "Ajo", "Limón", "Leche", "Garbanzos", "Lentejas", "Palta"];

    private readonly IDespensaService _despensa;

    public DespensaController(IDespensaService despensa)
    {
        _despensa = despensa;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var catalogo = await _despensa.CatalogoAsync();
        var modelo = new DespensaViewModel
        {
            Catalogo = catalogo.Select(i => (i.Nombre, i.Sinonimos)).ToList(),
            Contenido = await ArmarContenidoAsync(null),
        };
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar(string? texto)
    {
        var error = await _despensa.AgregarAsync(UsuarioActual.Id(User), texto ?? "");
        return await ResponderAsync(error);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Quitar(int id)
    {
        await _despensa.QuitarAsync(UsuarioActual.Id(User), id);
        return await ResponderAsync(null);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Vaciar()
    {
        await _despensa.VaciarAsync(UsuarioActual.Id(User));
        return await ResponderAsync(null);
    }

    // Si la petición viene de fetch, devuelve solo el bloque a reemplazar.
    // Si vino de un formulario común (JavaScript desactivado), vuelve a la página completa.
    private async Task<IActionResult> ResponderAsync(string? error)
    {
        if (Request.Headers["X-Requested-With"] == "fetch")
        {
            return PartialView("_Contenido", await ArmarContenidoAsync(error));
        }
        TempData["MensajeDespensa"] = error;
        return RedirectToAction(nameof(Index));
    }

    private async Task<DespensaContenidoViewModel> ArmarContenidoAsync(string? error)
    {
        var usuarioId = UsuarioActual.Id(User);
        var ingredientes = await _despensa.ObtenerAsync(usuarioId);
        var catalogo = await _despensa.CatalogoAsync();
        var sugerencias = await _despensa.SugerenciasAsync(usuarioId);

        return new DespensaContenidoViewModel
        {
            Mensaje = error ?? TempData["MensajeDespensa"] as string,
            Ingredientes = ingredientes,
            Rapidos = IngredientesComunes
                .Select(n => catalogo.FirstOrDefault(i => i.Nombre == n))
                .Where(i => i is not null && ingredientes.All(t => t.Id != i.Id))
                .Take(6)
                .ToList()!,
            PodesCocinar = sugerencias.PodesCocinar.Select(AVista).ToList(),
            CasiListas = sugerencias.CasiListas.Select(AVista).ToList(),
        };
    }

    private static RecetaDespensaViewModel AVista(AnalisisReceta a) => new()
    {
        Id = a.Receta.Id,
        Nombre = a.Receta.Nombre,
        Emoji = a.Receta.Emoji,
        TiempoMin = a.Receta.TiempoMin,
        Dificultad = a.Receta.Dificultad,
        Porcentaje = a.Porcentaje,
        Faltantes = a.Faltantes.Select(TextoFaltante).ToList(),
    };

    // "Queso fresco (30 g)", o solo el nombre si es "a gusto". Coma decimal siempre (es-AR), sin depender del servidor.
    private static string TextoFaltante(Models.RecetaIngrediente f)
    {
        if (f.Cantidad is null)
        {
            return f.Ingrediente.Nombre;
        }
        var cantidad = f.Cantidad.Value.ToString("0.##", CultureInfo.GetCultureInfo("es-AR"));
        return $"{f.Ingrediente.Nombre} ({cantidad} {f.Unidad})".Replace(" )", ")");
    }
}
