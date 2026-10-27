// Administración de ingredientes: listado, alta, edición y baja. Solo para el rol Admin (RNF12).
using CenaApp.Web.Services;
using CenaApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenaApp.Web.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Ingredientes/[action]/{id?}")]
public class AdminIngredientesController : Controller
{
    private readonly IIngredienteService _ingredientes;

    public AdminIngredientesController(IIngredienteService ingredientes)
    {
        _ingredientes = ingredientes;
    }

    // GET /Admin/Ingredientes (también /Admin/Ingredientes/Index?buscar=pal)
    [HttpGet("/Admin/Ingredientes")]
    [HttpGet]
    public async Task<IActionResult> Index(string? buscar)
    {
        ViewData["Buscar"] = buscar;
        return View(await _ingredientes.ListarAsync(buscar));
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var formulario = new IngredienteFormularioViewModel();
        await _ingredientes.CargarOpcionesAsync(formulario);
        return View("Formulario", formulario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(IngredienteFormularioViewModel formulario)
    {
        formulario.Id = null;
        if (!await EsValidoAsync(formulario))
        {
            await _ingredientes.CargarOpcionesAsync(formulario);
            return View("Formulario", formulario);
        }

        var creado = await _ingredientes.CrearAsync(formulario);
        TempData["Exito"] = $"Se creó el ingrediente {creado.Nombre}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var ingrediente = await _ingredientes.ObtenerAsync(id);
        if (ingrediente is null)
        {
            return NotFound();
        }

        var formulario = new IngredienteFormularioViewModel
        {
            Id = ingrediente.Id,
            Nombre = ingrediente.Nombre,
            Grupo = ingrediente.Grupo,
            Sinonimos = ingrediente.Sinonimos,
            CuentaEnDespensa = ingrediente.CuentaEnDespensa,
            AlergenosIds = ingrediente.Alergenos.Select(a => a.Id).ToList(),
            RestriccionesIncompatiblesIds = ingrediente.RestriccionesIncompatibles.Select(r => r.Id).ToList(),
        };
        await _ingredientes.CargarOpcionesAsync(formulario);
        return View("Formulario", formulario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, IngredienteFormularioViewModel formulario)
    {
        formulario.Id = id;
        if (!await EsValidoAsync(formulario))
        {
            await _ingredientes.CargarOpcionesAsync(formulario);
            return View("Formulario", formulario);
        }

        if (!await _ingredientes.ActualizarAsync(id, formulario))
        {
            return NotFound();
        }
        TempData["Exito"] = $"Se guardaron los cambios de {formulario.Nombre.Trim()}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var error = await _ingredientes.EliminarAsync(id);
        if (error is null)
        {
            TempData["Exito"] = "Se eliminó el ingrediente.";
        }
        else
        {
            TempData["Error"] = error;
        }
        return RedirectToAction(nameof(Index));
    }

    // Data Annotations (ModelState) + reglas del servicio, todo en el mismo ModelState para mostrarlo en el formulario.
    private async Task<bool> EsValidoAsync(IngredienteFormularioViewModel formulario)
    {
        if (ModelState.IsValid)
        {
            foreach (var (campo, mensaje) in await _ingredientes.ValidarAsync(formulario))
            {
                ModelState.AddModelError(campo, mensaje);
            }
        }
        return ModelState.IsValid;
    }
}
