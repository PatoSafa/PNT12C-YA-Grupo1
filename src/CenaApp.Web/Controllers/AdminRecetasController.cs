// Administración de recetas: listado, alta, edición y baja con confirmación. Solo para el rol Admin (RNF12).
// Usa las mismas validaciones y el mismo alta que la API (RecetaService + RecetaCrearDto).
using CenaApp.Web.Data;
using CenaApp.Web.Services;
using CenaApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CenaApp.Web.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Recetas/[action]/{id?}")]
public class AdminRecetasController : Controller
{
    private readonly IRecetaService _recetas;
    private readonly AppDbContext _db;

    public AdminRecetasController(IRecetaService recetas, AppDbContext db)
    {
        _recetas = recetas;
        _db = db;
    }

    [HttpGet("/Admin/Recetas")]
    [HttpGet]
    public async Task<IActionResult> Index(string? buscar)
    {
        ViewData["Buscar"] = buscar;
        var recetas = await _recetas.ListarAsync(buscar);
        return View(recetas.Select(r => new RecetaFilaViewModel
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Emoji = r.Emoji,
            Categoria = r.Categoria.Nombre,
            TiempoMin = r.TiempoMin,
        }).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var formulario = new RecetaFormularioViewModel();
        formulario.Datos.Porciones = 1;
        await CargarOpcionesAsync(formulario);
        return View("Formulario", formulario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(RecetaFormularioViewModel formulario)
    {
        formulario.Id = null;
        if (!await PrepararYValidarAsync(formulario))
        {
            await CargarOpcionesAsync(formulario);
            return View("Formulario", formulario);
        }

        var creada = await _recetas.CrearAsync(formulario.Datos);
        TempData["Exito"] = $"Se creó la receta {creada.Nombre}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var receta = await _recetas.ObtenerAsync(id);
        if (receta is null)
        {
            return NotFound();
        }

        var formulario = new RecetaFormularioViewModel
        {
            Id = receta.Id,
            Datos = new RecetaCrearDto
            {
                Nombre = receta.Nombre,
                Descripcion = receta.Descripcion,
                Emoji = receta.Emoji,
                TiempoMin = receta.TiempoMin,
                Porciones = receta.Porciones,
                Calorias = receta.Calorias,
                Dificultad = receta.Dificultad,
                CategoriaId = receta.CategoriaId,
                GustosIds = receta.Gustos.Select(g => g.Id).ToList(),
                Ingredientes = receta.Ingredientes
                    .OrderBy(ri => ri.Ingrediente.Nombre)
                    .Select(ri => new IngredienteCrearDto { IngredienteId = ri.IngredienteId, Cantidad = ri.Cantidad, Unidad = ri.Unidad })
                    .ToList(),
            },
            PasosTexto = string.Join("\n", receta.Pasos.OrderBy(p => p.Orden).Select(p => p.Texto)),
            AlergenosCalculados = RecetaService.CalcularAlergenos(receta),
        };
        await CargarOpcionesAsync(formulario);
        return View("Formulario", formulario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, RecetaFormularioViewModel formulario)
    {
        formulario.Id = id;
        if (!await PrepararYValidarAsync(formulario))
        {
            await CargarOpcionesAsync(formulario);
            return View("Formulario", formulario);
        }

        if (!await _recetas.ActualizarAsync(id, formulario.Datos))
        {
            return NotFound();
        }
        TempData["Exito"] = $"Se guardaron los cambios de {formulario.Datos.Nombre.Trim()}.";
        return RedirectToAction(nameof(Index));
    }

    // La confirmación la pide js/confirmar.js antes de enviar (VN-22). Pasos, ingredientes y favoritos se borran en cascada.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (await _recetas.EliminarAsync(id))
        {
            TempData["Exito"] = "Se eliminó la receta.";
        }
        else
        {
            TempData["Error"] = $"No existe una receta con id {id}.";
        }
        return RedirectToAction(nameof(Index));
    }

    // Arma el DTO a partir de lo que llegó del formulario y lo valida igual que la API.
    private async Task<bool> PrepararYValidarAsync(RecetaFormularioViewModel formulario)
    {
        // Las filas de ingrediente que quedaron vacías no cuentan.
        formulario.Datos.Ingredientes = formulario.Datos.Ingredientes.Where(i => i.IngredienteId > 0).ToList();
        // Un paso por línea, sin líneas vacías. (Si el textarea llega vacío, el model binder lo deja en null: ver E-04.)
        formulario.Datos.Pasos = (formulario.PasosTexto ?? "")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        // Se valida de nuevo, ya con las listas armadas (Data Annotations del DTO)...
        ModelState.Clear();
        if (!TryValidateModel(formulario))
        {
            return false;
        }
        // ...y después las reglas del servicio (nombre único, ingredientes repetidos, etc.).
        foreach (var (campo, mensaje) in await _recetas.ValidarAsync(formulario.Datos, formulario.Id))
        {
            ModelState.AddModelError($"{nameof(formulario.Datos)}.{campo}", mensaje);
        }
        return ModelState.IsValid;
    }

    private async Task CargarOpcionesAsync(RecetaFormularioViewModel formulario)
    {
        formulario.Categorias = await _db.Categorias.OrderBy(c => c.Id).ToListAsync();
        formulario.Gustos = await _db.Gustos.OrderBy(g => g.Tipo).ThenBy(g => g.Nombre).ToListAsync();
        formulario.Ingredientes = await _db.Ingredientes.OrderBy(i => i.Nombre).ToListAsync();
    }
}
